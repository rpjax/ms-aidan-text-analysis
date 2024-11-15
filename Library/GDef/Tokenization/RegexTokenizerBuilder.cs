using Aidan.Core.Patterns;
using Aidan.TextAnalysis.Tokenization;
using Aidan.TextAnalysis.Tokenization.StateMachine;
using Aidan.TextAnalysis.Tokenization.StateMachine.Builders;

namespace Aidan.TextAnalysis.GDef.Tokenization;

public class RegexTokenizerBuilder : IBuilder<Tokenizer>
{
    internal static class States
    {
        public static string InitialState { get; } = "initial_state";
    }

    public Tokenizer Build()
    {
        var builder = new TokenizerBuilder();

        SkipWhitespace(builder);
        EscapeSequence(builder);
        OperatorChars(builder);
        Utf8Chars(builder);

        return builder.Build();
    }

    /* private methods */

    private char[] GetOperatorChars()
    {
        return new char[]
        {
            '.', // Matches any single character (except newline)
            '*', // Zero or more occurrences
            '+', // One or more occurrences
            '?', // Zero or one occurrence
            '|', // Alternation (OR)
            '(', // Group start
            ')', // Group end
            '[', // Character class start
            ']', // Character class end
            '{', // Quantifier start
            '}', // Quantifier end
            '^', // Start of line/string
            '$', // End of line/string
        };
    }

    private void SkipWhitespace(TokenizerBuilder builder)
    {
        builder.FromInitialState()
            .OnWhitespace()
            .Recurse()
            ;
    }

    private void EscapeSequence(TokenizerBuilder builder)
    {
        builder.SetCharset(CharsetType.Ascii);
    }

    private void OperatorChars(TokenizerBuilder builder)
    {
        builder.SetCharset(CharsetType.Ascii);
    }

    private void Utf8Chars(TokenizerBuilder builder)
    {
        var charset = TokenizerBuilder.ComputeCharset(CharsetType.Ascii);

        builder.SetCharset(charset);

    }

}

