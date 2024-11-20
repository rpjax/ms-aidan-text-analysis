using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;
using Aidan.TextAnalysis.RegularExpressions.Automata;

namespace Aidan.TextAnalysis.GDef.Tokenization;

public static class GDefLexemes
{
    public static string Comment { get; } = "comment";
    public static string Identifier { get; } = "identifier";
    public static string String { get; } = "string";
    public static char[] StringDelimiters { get; } = new char[] { '"', '\'' };

    /* keywords */
    public static string Lexeme { get; } = "lexeme";
    public static string Use { get; } = "use";
    public static string Charset { get; } = "charset";

    /* special characters */
    public static char[] SpecialChars { get; } = new char[]
    {
        ':', 
        ';', 
        '$', 
        '*', 
        '+', 
        '?', 
        '(', ')', 
        '|', 
        '='
    };

    public static char[] RegexSpecialChars { get; } = new char[]
    {
        '\\', // Escape character
        '[', ']', // Character class
        '(', ')', // Grouping
        '|', // Alternation
        '*', // Zero or more
        '+', // One or more
        '?', // Optional
        '{', '}', // Quantifiers
        '^', // Start of line
        '$', // End of line
        '.', // Any character
        '-', // Range separator
        '@', // Fragment reference
    };

    public static Charset GlobalCharset { get; } = Language.Components.Charset.Compute(CharsetType.ExtendedAscii);

    public static Lexeme[] GetLexemes()
    {
        /* 
         * The order of lexemes matters. 
         * If a lexeme is a subset of another lexeme, the subset lexeme should be defined first 
         */
        var lexemes = new List<Lexeme>
        {
            GetLexemeLexeme(),
            GetUseLexeme(),
            GetCharsetLexeme(),
            GetIdentifierLexeme(),
            GetStringLexeme(),
            //GetCommentLexeme()
        };

        lexemes.AddRange(GetSpecialCharsLexemes());

        return lexemes.ToArray();
    }

    private static Lexeme GetStringLexeme()
    {
        /* /'([^'] | \')*'/ */

        var singleQuote = '\'';
        var doubleQuote = '\"';
        var escapeBar = '\\';
        var charset = Language.Components.Charset.Compute(CharsetType.ExtendedAscii);

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

    private static Lexeme GetLexemeLexeme()
    {
        return new Lexeme(name: Lexeme, pattern: RegExpr.FromString("lexeme"));
    }

    private static Lexeme GetUseLexeme()
    {
        return new Lexeme(name: Use, pattern: RegExpr.FromString("use"));
    }

    private static Lexeme GetCharsetLexeme()
    {
        return new Lexeme(name: Charset, pattern: RegExpr.FromString("charset"));
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

}
