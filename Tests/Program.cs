using Aidan.Core.Patterns;
using Aidan.TextAnalysis.GDef;
using Aidan.TextAnalysis.GDef.Tokenization;
using Aidan.TextAnalysis.Parsing.Extensions;
using Aidan.TextAnalysis.RegularExpressions;
using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.RegularExpressions.Automata;
using Aidan.TextAnalysis.Tokenization;
using Aidan.TextAnalysis.Tokenization.StateMachine;
using Aidan.TextAnalysis.Tokenization.StateMachine.Builders;
using System.Globalization;

namespace Aidan.TextAnalysis.Tests;

public class Program
{
    public static void Main(string[] args)
    {
        /* debug test */
        var testLexemes = new Lexeme[]
        {
            new Lexeme("lexeme 'a'", RegexNode.FromString("a")),
            new Lexeme("lexeme 'ab'", RegexNode.FromString("ab")),
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
        var lexemes = GDefLexemes.GetLexemes();
        var ignoredChars = new char[] { ' ', '\t', '\n', '\r', '\0' };

        /* tokenizer test */
        var calculator = new TokenizerCalculator(
            lexemes: lexemes,
            ignoredChars: ignoredChars,
            useDebug: true);

        var tokenizer = calculator.ComputeTokenizer();
        var input = ";;  $use charset utf8; foobar_id";

        var tokens = tokenizer.TokenizeToArray(input);

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }

    }

    private static string FormatTime(double seconds)
    {
        return seconds.ToString("F10", CultureInfo.InvariantCulture);
    }
}

public class DebugTokenizerBuilder : IBuilder<Tokenizer>
{
    public DebugTokenizerBuilder()
    {

    }

    public Tokenizer Build()
    {
        var ignoredChars = new char[] { ' ', '\t', '\n', '\r', '\0' };
        var builder = new TokenizerBuilder();

        builder.SetCharset(CharsetType.Ascii);

        StringLexemes(builder);
        //CStyleComment(builder);

        builder.EnableDebugger();

        return builder.Build();
    }

    /* tokenization rules */

    private void FooLexeme(TokenizerBuilder builder)
    {

    }

    /* strings*/
    private void StringLexemes(TokenizerBuilder builder)
    {
        var delimiters = GDefLexemes.StringDelimiters;
        var escapeChar = '\\';

        var originalCharset = builder.GetCharset();

        var workingCharset = originalCharset
            .Concat(delimiters)
            .Distinct()
            .ToArray();

        var stringCharset = TokenizerBuilder.ComputeCharset(CharsetType.Ascii);

        builder.SetCharset(workingCharset);

        foreach (var delimiter in delimiters)
        {
            var stringState = $"{delimiter}-string start";
            var stringEndState = $"{delimiter}-string end";
            var escapeState = $"{delimiter}-string escape char";
            var acceptName = GDefLexemes.String;

            var otherDelimiters = delimiters
                .Where(d => d != delimiter)
                .ToArray();
        }

    }

    /* comments */
    private void CStyleComment(TokenizerBuilder builder)
    {
        var cStyleCommentStart = "c_style_comment_start";

        var cStyleInlineComment = "c_style_inline_comment";
        var cStyleInlineCommentEnd = "c_style_inline_comment_end";

        var cStyleBlockComment = "c_style_block_comment";
        var cStyleBlockCommentEnd = "c_style_block_comment_end";
        var acceptName = GDefLexemes.Comment;

        var originalCharset = builder.GetCharset();

        var workingCharset = originalCharset
            .Concat(new char[] { '/', '*' })
            .Distinct()
            .ToArray();

        var commentCharset = TokenizerBuilder.ComputeCharset(CharsetType.Ascii);

    }
}

