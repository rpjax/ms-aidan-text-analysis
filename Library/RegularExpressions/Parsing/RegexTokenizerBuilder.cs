using Aidan.Core.Patterns;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Tokenization;
using Aidan.TextAnalysis.Tokenization.StateMachine.Builders;

namespace Aidan.TextAnalysis.RegularExpressions.Parsing;

public class RegexTokenizerBuilder : IBuilder<Tokenizer>
{
    private char[] SpecialChars { get; }
    private char[] IgnoredChars { get; }
    private char[] EscapableChars { get; }
    private Charset Charset { get; }

    public RegexTokenizerBuilder()
    {
        SpecialChars = new char[]
        {
            //'\\', // Escape character
            '[', ']', // Character class
            '(', ')', // Grouping
            '|', // Alternation
            '*', // Zero or more
            '+', // One or more
            '?', // Optional
            '{', '}', // Quantifiers
            '^', // Class negation
            //'$', // End of line
            '.', // Any character
            '-', // Range separator
            '@', // Fragment reference        
        };
        EscapableChars = new char[]
        {
            '\\',
            'n',
            't',
            '0',
        };
        IgnoredChars = new char[] { ' ', '\t', '\n', '\r', '\0' };
        Charset = Charset.Compute(CharsetType.ExtendedAscii);
    }

    public Tokenizer Build()
    {
        var builder = new ManualTokenizerBuilder();

        AddSkipIgnoredChars(builder);
        AddEscapeSequence(builder);
        AddSpecialChars(builder);
        AddLiteralChars(builder);

        return builder.Build();
    }

    /* private methods */

    private void AddSkipIgnoredChars(ManualTokenizerBuilder builder)
    {
        builder.FromInitialState()
            .OnManyCharacters(IgnoredChars)
            .Recurse()
            ;
    }

    private void AddEscapeSequence(ManualTokenizerBuilder builder)
    {
        var escapedChars = SpecialChars
            .Concat(EscapableChars)
            .Distinct()
            .ToArray();

        builder.FromInitialState()
            .OnEscapeBar()
            .GoTo("escape_bar");

        foreach (var c in escapedChars)
        {
            builder.FromState("escape_bar")
                .OnCharacter(c)
                .GoTo("escaped_char");

            builder.FromState("escaped_char")
                .Accept();
        }
    }

    private void AddSpecialChars(ManualTokenizerBuilder builder)
    {
        foreach (var c in SpecialChars)
        {
            builder.FromInitialState()
                .OnCharacter(c)
                .GoTo(c.ToString());

            builder.FromState(c.ToString())
                .Accept();
        }
    }

    private void AddLiteralChars(ManualTokenizerBuilder builder)
    {
        var literalChars = Charset
            .Except(SpecialChars)
            .Except(IgnoredChars)
            .Except(new char[] { '\\' })
            .ToArray();

        foreach (var c in literalChars)
        {
            builder.FromInitialState()
                .OnCharacter(c)
                .GoTo("char");

            builder.FromState("char")
                .Accept();
        }

    }

}

