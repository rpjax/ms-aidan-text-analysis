using Aidan.Core.Patterns;
using Aidan.TextAnalysis.Tokenization.StateMachine;

namespace Aidan.TextAnalysis.GDef.Tokenization;

public class RegexTokenizerBuilder : IBuilder<TokenizerMachine>
{
    public TokenizerMachine Build()
    {
        var builder = new TokenizerDfaBuilder(initialStateName: States.InitialState);

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

    private void SkipWhitespace(TokenizerDfaBuilder builder)
    {
        builder.FromInitialState()
            .OnWhitespace()
            .Recurse()
            ;
    }

    private void EscapeSequence(TokenizerDfaBuilder builder)
    {
        builder.SetCharset(CharsetType.Ascii);

        builder
            .FromInitialState()
            .OnCharacter('\\')
            .GoTo("escape_sequence_start")
            ;

        builder
            .FromState("escape_sequence_start")
            .OnAnyCharacter()
            .Except(' ')
            .GoTo("escape_sequence_char")
            ;

        builder
            .FromState("escape_sequence_char")
            .OnAnyCharacter()
            .Accept("escaped_char")
            ;
    }

    private void OperatorChars(TokenizerDfaBuilder builder)
    {
        builder.SetCharset(CharsetType.Ascii);

        foreach (var c in GetOperatorChars())
        {
            var stateName = $"operator '{c}'";
            var acceptName = $"{c}";

            builder
                .FromInitialState()
                .OnCharacter(c)
                .GoTo(stateName)
                ;

            builder
                .FromState(stateName)
                .OnAnyCharacter()
                .Accept(acceptName)
                ;
        }
    }

    private void Utf8Chars(TokenizerDfaBuilder builder)
    {
        var charset = TokenizerDfaBuilder.ComputeCharset(CharsetType.Ascii);

        builder.SetCharset(charset);

        foreach (var c in charset.Except(GetOperatorChars()))
        {
            if (c == ' ')
            {
                continue;
            }
            if (c == '\\')
            {
                continue;
            }

            var stateName = $"char '{c}'";
            var acceptName = $"char";

            builder
                .FromInitialState()
                .OnCharacter(c)
                .GoTo(stateName)
                ;

            builder
                .FromState(stateName)
                .OnAnyCharacter()
                .Accept(acceptName)
                ;
        }
    }
}

internal static class States
{
    public static string InitialState { get; } = "initial_state";
}
