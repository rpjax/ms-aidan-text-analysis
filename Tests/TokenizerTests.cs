using Aidan.Core.Patterns;
using Aidan.TextAnalysis.GDef;
using Aidan.TextAnalysis.GDef.Tokenization;
using Aidan.TextAnalysis.Language.Extensions;
using Aidan.TextAnalysis.RegularExpressions.Automata;
using Aidan.TextAnalysis.RegularExpressions.Tree;
using Aidan.TextAnalysis.Tokenization;
using Aidan.TextAnalysis.Tokenization.StateMachine.Builders;

namespace Aidan.TextAnalysis.Tests;

public class DebugTokenizerBuilder : IBuilder<Tokenizer>
{
    public DebugTokenizerBuilder()
    {

    }

    public Tokenizer Build()
    {
        var ignoredChars = new char[] { ' ', '\t', '\n', '\r', '\0' };
        var builder = new TokenizerBuilder();


        return builder.Build();
    }

    /* tokenization rules */

    private void FooLexeme(TokenizerBuilder builder)
    {

    }

    /* strings*/
    private void StringLexemes(TokenizerBuilder builder)
    {

    }

    /* comments */
    private void CStyleComment(TokenizerBuilder builder)
    {

    }
}

public class TokenizerTests
{
    private static void TestTokenizerBuild()
    {

        var grammarFile = @"
fragment digit = /[0-9]/;
fragment uint = /@digit+/;
fragment int = /(\+|\-)?@uint/;
fragment float = /@int\.@uint/;

lexeme int = /@int/;
lexeme float = /@float/;
lexeme number = '@int|@float';
lexeme string = /'[^']*'/;

fragment c_style_comment_start = '/*';
fragment c_style_comment_end = '*/';
fragment c_style_comment = /@c_style_comment_start((.)*)?@c_style_comment_end/;

lexeme comment = /@c_style_comment/;
lexeme id = /[a-zA-Z_][a-zA-Z0-9_]*/;

start
    : regex
    ;

regex
    : '/' regex_body '/'
    ;

symbol
    : terminal
    | non_terminal
    | macro
    ;

regex_body
    : char 
    | special_char
    | escape_char

char
    : 'a'
    |
    ;

regex_macro
    : '?' symbol // optional
    | '*' symbol // zero or more
    | '+' symbol // one or more
    | '.' // any character
    | '[' class_symbol { class_symbol } ']' // character class
    ;

class_symbol
    : letter
    | digit
    | macro
    ;

char_range
    : char '-' char
    ;
";

        var grammar = GDefParser.ParseToGrammar(grammarFile).ExpandMacros();
        //GDefService.CreateLR1Parser();
        /* debug test */
        var testLexemes = new Lexeme[]
        {
            new Lexeme("lexeme 'a'", RegExpr.FromString("a")),
            new Lexeme("lexeme 'ab'", RegExpr.FromString("ab")),
            //new Lexeme("lexeme 'ab'", new ConcatenationNode(
            //    new LiteralNode('a'),
            //    new StarNode(new LiteralNode('b'))
            //)),
            //new Lexeme("lexeme 'c'", RegexNode.FromString("c")),
        };

        var foo = new Lexeme("lexeme '(a|b)*.a.b.b#'",
            new ConcatenationNode(
                new ConcatenationNode(
                    new ConcatenationNode(
                        new ConcatenationNode(
                            new StarNode(
                                new UnionNode(
                                    new LiteralNode('a'),
                                    new LiteralNode('b')
                                )
                            ),
                            new LiteralNode('a')
                        ),
                        new LiteralNode('b')
                    ),
                    new LiteralNode('b')
                ),
                new LiteralNode('#')
            )
        );

        /* GDef test */
        var lexemes = GDefTokenizerBuilder.GetLexemes();
        var ignoredChars = new char[] { ' ', '\t', '\n', '\r', '\0' };

        ///* tokenizer test */
        //var calculator = new TokenizerCalculator(
        //    lexemes: lexemes,
        //    ignoredChars: ignoredChars,
        //    useDebug: true);

        //var grammarTokenizerBuilder = new GDefGrammarTokenizerBuilder();
        //var grammarTokenizer = grammarTokenizerBuilder.Build();
        ////var tokenizer = calculator.ComputeTokenizer();

        //var input = ";;  $use charset 'utf8 \\'foo\\' bar'; foobar_id";

        //var tokens = grammarTokenizer.TokenizeToArray(input);

        //foreach (var token in tokens)
        //{
        //    Console.WriteLine(token);
        //}

    }

}
