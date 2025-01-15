using Aidan.Core.Patterns;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.RegularExpressions.Ast.ClassChildren;
using Aidan.TextAnalysis.RegularExpressions.Automata;
using Aidan.TextAnalysis.RegularExpressions.Automata.AutomatonComputation;
using Aidan.TextAnalysis.Tokenization;
using Aidan.TextAnalysis.Tokenization.StateMachine.Builders;

namespace Aidan.TextAnalysis.GDef.Tokenization;

public class GDefTokenizerBuilder : IBuilder<Tokenizer>
{
    public static string Comment { get; } = "comment";
    public static string Identifier { get; } = "identifier";
    public static string String { get; } = "string";

    /* keywords */
    public static string LexemeKeyword { get; } = "lexeme";
    public static string FragmentKeyword { get; } = "fragment";
    public static string CharsetKeyword { get; } = "charset";
    public static string IgnoreKeyword { get; } = "ignore";
    public static string TrueKeyword { get; } = "true";
    public static string FalseKeyword { get; } = "false";
    public static string IgnoredCharsKeyword { get; } = "ignored-chars";

    /* special characters */
    public static char[] SpecialChars { get; } = new char[]
    {
        ':',
        ';',
        ',',
        '$',
        '*',
        '+',
        '?',
        '(', ')',
        '[', ']',
        '|',
        '='
    };

    private Charset Charset { get; set; }

    public GDefTokenizerBuilder(Charset? charset = null)
    {
        Charset = charset ?? Charset.Compute(CharsetType.ExtendedAscii);
    }

    public static Lexeme[] GetLexemes()
    {
        /* 
         * The order of lexemes matters. 
         * If a lexeme is a subset of another lexeme, the subset lexeme should be defined first 
         */
        var lexemes = new List<Lexeme>();

        lexemes.AddRange(GetKeywordLexemes());
        lexemes.Add(GetIdentifierLexeme());
        lexemes.Add(GetStringLexeme());
        lexemes.AddRange(GetSpecialCharsLexemes());

        return lexemes.ToArray();
    }

    private static IEnumerable<Lexeme> GetKeywordLexemes()
    {
        yield return new Lexeme(name: LexemeKeyword, pattern: RegExpr.FromString(LexemeKeyword));
        yield return new Lexeme(name: CharsetKeyword, pattern: RegExpr.FromString(CharsetKeyword));
        yield return new Lexeme(name: IgnoreKeyword, pattern: RegExpr.FromString(IgnoreKeyword));
        yield return new Lexeme(name: TrueKeyword, pattern: RegExpr.FromString(TrueKeyword));
        yield return new Lexeme(name: FalseKeyword, pattern: RegExpr.FromString(FalseKeyword));
        yield return new Lexeme(name: IgnoredCharsKeyword, pattern: RegExpr.FromString(IgnoredCharsKeyword));
    }

    private static Lexeme GetStringLexeme()
    {
        /* /'([^'] | \')*'/ */

        var singleQuote = '\'';
        var doubleQuote = '\"';
        var escapeBar = '\\';
        var charset = Charset.Compute(CharsetType.ExtendedAscii);

        var singleQuotePattern = new ConcatenationNode(
            new LiteralNode(singleQuote),
            new ConcatenationNode(
                new StarNode(
                    new UnionNode(
                        new ConcatenationNode(
                            new LiteralNode(escapeBar),
                            new LiteralNode(singleQuote)
                        ),
                        new ClassNode(
                            charset: charset,
                            isNegated: true,
                            children: new ClassChild[]
                            {
                                new ClassLiteral(singleQuote)
                            }
                        )
                    )
                ),
                new LiteralNode(singleQuote)
            )
        );

        var doubleQuotePattern = new ConcatenationNode(
            new LiteralNode(doubleQuote),
            new ConcatenationNode(
                new StarNode(
                    new UnionNode(
                        new ConcatenationNode(
                            new LiteralNode(escapeBar),
                            new LiteralNode(doubleQuote)
                        ),
                        new ClassNode(
                            charset: charset,
                            isNegated: true,
                            children: new ClassChild[]
                            {
                                new ClassLiteral(doubleQuote)
                            }
                        )
                    )
                ),
                new LiteralNode(doubleQuote)
            )
        );

        var pattern = RegExpr.Union(singleQuotePattern, doubleQuotePattern);

        return new Lexeme("string", pattern: pattern);
    }

    private static Lexeme GetIdentifierLexeme()
    {
        // /[a-Z_][a-Z0-9_]*/ 

        var charset = Language.Components.Charset.Compute(CharsetType.ExtendedAscii);

        var pattern = new ConcatenationNode(
            new ClassNode(
                charset: charset,
                isNegated: false,
                children: new ClassChild[]
                {
                    new ClassRange('a', 'z'),
                    new ClassRange('A', 'Z'),
                    new ClassLiteral('_')
                }
            ),
            new StarNode(
                new ClassNode(
                    charset: charset,
                    isNegated: false,
                    children: new ClassChild[]
                    {
                        new ClassRange('a', 'z'),
                        new ClassRange('A', 'Z'),
                        new ClassRange('0', '9'),
                        new ClassLiteral('_')
                    }
                )
            )
        );

        return new Lexeme(Identifier, pattern);
    }

    private static IEnumerable<Lexeme> GetSpecialCharsLexemes()
    {
        foreach (var c in SpecialChars)
        {
            yield return new Lexeme(name: c.ToString(), pattern: RegExpr.FromCharacter(c));
        }
    }

    private static Lexeme GetCommentLexeme()
    {
        // `//[^\n]*\n`
        var charset = Language.Components.Charset.Compute(CharsetType.ExtendedAscii);

        var inlineCommentPattern = new ConcatenationNode(
            new LiteralNode('/'),
            new ConcatenationNode(
                new LiteralNode('/'),
                new ConcatenationNode(
                    new StarNode(
                        new ClassNode(
                            charset: charset,
                            isNegated: true,
                            children: new ClassChild[]
                            {
                                new ClassLiteral('\n')
                            }
                        )
                    ),
                    new LiteralNode('\n')
                )
            )
        );

        // `/* ([^*]*|*/) */`
        // end path
        // `[^*]*|*/` ('/*' seen)
        // `∅*|/` ('*' seen)
        // `∅|/` 
        // `/` 
        // `ε` ('/' seen)
        // continuation path
        // `[^*]*|*/` ('/*' seen)
        // `[^*]*|∅` ('x' seen)
        // `[^*]*` 
        // what i want path
        // `[^*]*|*/` ('/*' seen)
        // `[^*]*|*/` ('*' seen)
        var blockCommentPattern = new ConcatenationNode(
            new LiteralNode('/'),
            new ConcatenationNode(
                new LiteralNode('*'),
                new UnionNode(
                    new StarNode(
                        new ClassNode(
                            charset: charset,
                            isNegated: true,
                            children: new ClassChild[]
                            {
                                //new ClassCharacter('\n')
                            }
                        )
                    ),
                    new ConcatenationNode(
                        new LiteralNode('*'),
                        new LiteralNode('/')
                    )
                )
            )
        );

        // `//.*\n|/*(.*)*/`
        var pattern = RegExpr.Union(inlineCommentPattern, blockCommentPattern);

        return new Lexeme(Comment, pattern);
    }

    public Tokenizer Build()
    {
        var lexemes = GetLexemes();
        var ignoredChars = new char[] { ' ', '\t', '\n', '\r', '\0' };
        var calculator = new TokenizerCalculator(lexemes, ignoredChars);
        var table = calculator.ComputeTokenizerTable();
        var builder = new ManualTokenizerBuilder(table);

        AddComments(builder);

        return builder.Build();
    }

    public void AddComments(ManualTokenizerBuilder builder)
    {
        var charset = Charset.Compute(CharsetType.ExtendedAscii);
        var barSeen = "/";

        builder.SetCharset(charset);

        builder.FromInitialState()
            .OnCharacter('/')
            .GoTo(barSeen);

        builder.FromState(barSeen)
            .OnCharacter('/')
            .GoTo("c_style_inline_comment");

        builder.FromState("c_style_inline_comment")
            .OnAnyCharacter()
            .Except('\n')
            .Recurse();

        builder.FromState("c_style_inline_comment")
            .OnCharacter('\n')
            .GoTo(Comment);

        builder.FromState(barSeen)
            .OnCharacter('*')
            .GoTo("c_style_block_comment");

        builder.FromState("c_style_block_comment")
            .OnAnyCharacter()
            .Except('*')
            .Recurse();

        builder.FromState("c_style_block_comment")
            .OnCharacter('*')
            .GoTo("c_style_block_comment_'*'_seen");

        builder.FromState("c_style_block_comment_'*'_seen")
            .OnAnyCharacter()
            .Except('/')
            .GoTo("c_style_block_comment");

        builder.FromState("c_style_block_comment_'*'_seen")
            .OnCharacter('/')
            .GoTo(Comment);

        builder.FromState(Comment)
            .Accept();
    }

}
