using Aidan.Core.Patterns;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.RegularExpressions.Automata;
using Aidan.TextAnalysis.Tokenization;
using Aidan.TextAnalysis.Tokenization.Components;
using Aidan.TextAnalysis.Tokenization.StateMachine.Builders;

namespace Aidan.TextAnalysis.RegularExpressions.Parsing;

public class RegexTokenizerBuilder : IBuilder<Tokenizer>
{
    public static char[] SpecialChars { get; } = new char[]
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

    public Tokenizer Build()
    {
        var builder = new TokenizerBuilder();

        var calculator = new TokenizerCalculator(
            charset: null,
            lexemes: null,
            ignoredChars: null,
            useDebug: false);

        SkipWhitespace(builder);
        EscapeSequence(builder);
        OperatorChars(builder);
        Utf8Chars(builder);

        return builder.Build();
    }

    /* private methods */



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

