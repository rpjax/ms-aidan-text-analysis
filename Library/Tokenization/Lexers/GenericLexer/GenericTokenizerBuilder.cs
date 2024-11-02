using Aidan.Core;
using Aidan.Core.Patterns;
using Aidan.TextAnalysis.Tokenization.StateMachine;

namespace Aidan.TextAnalysis.Tokenization.GenericLexer;

public class LexemeDefinition
{
    public string Type { get; }
    public string Value { get; }
    public CharsetType Charset { get; }

    public LexemeDefinition(
        string type, 
        string value)
    {
        Type = type;
        Value = value;
    }
}

public class GenericTokenizerBuilder : IBuilder<TokenizerMachine>
{
    private bool IncludeIntegers { get; set; }
    private bool IncludeFloats { get; set; }
    private bool IncludeStrings { get; set; }
    private bool IncludePunctuation { get; set; }

    private LexemeDefinition[] Lexemes { get; } = new LexemeDefinition[]
    {
        new LexemeDefinition("int", "int"),
        new LexemeDefinition("float", "float"),
        new LexemeDefinition("string", "string"),
        new LexemeDefinition("identifier", "identifier"),
        new LexemeDefinition("punctuation", "punctuation"),
        new LexemeDefinition("symbol", "symbol"),
    };

    public TokenizerMachine Build()
    {
        var builder = new TokenizerDfaBuilder(initialStateName: States.InitialState);

        SkipWhitespace(builder);
 
        NumbersLexemes(builder);
        StringLexemes(builder);
        IdentifierLexeme(builder);
        PuntuactionLexemes(builder);
        SymbolLexemes(builder);

        return builder.Build();
    }

    /* private methods */

    private void SkipWhitespace(TokenizerDfaBuilder builder)
    {
        builder.FromInitialState()
            .OnWhitespace()
            .Recurse()
            ;
    }

    private void NumbersLexemes(TokenizerDfaBuilder builder)
    {
        builder
            .FromInitialState()
            .OnCharacter('-')
            .GoTo(States.MinusSign)

            .FromState(States.MinusSign)
            .OnCharacterRange('0', '9')
            .GoTo(States.IntDigitAccepted)
            ;

        builder
            .FromState(States.MinusSign)
            .OnAnyCharacterExceptRange('0', '9')
            .Accept(States.MinusSignAccepted)
            ;

        IntLexeme(builder);
        FloatLexeme(builder);
    }

    private void IntLexeme(TokenizerDfaBuilder builder)
    {
        builder.SetCharset(CharsetType.Ascii);

        // Handle Positive Integers
        builder.FromInitialState()
            .OnCharacterRange('0', '9')
            .GoTo(States.IntDigitAccepted)

            .FromState(States.IntDigitAccepted)
            .OnCharacterRange('0', '9')
            .Recurse()

            .FromState(States.IntDigitAccepted)
            .OnCharacter('.')
            .GoTo(States.FloatStart) // float transition

            .FromState(States.IntDigitAccepted)
            .OnAnyCharacter()
            .ExceptRange('0', '9')
            .Except('.')
            .Accept(States.IntAccepted)
            ;
    }

    private void FloatLexeme(TokenizerDfaBuilder builder)
    {
        builder.SetCharset(CharsetType.Ascii);

        // integer: 5, -5
        // float: 5.5, -5.5

        builder.FromState(States.FloatStart)
                .OnCharacterRange('0', '9')
                .GoTo(States.FloatDigitAccepted)

            .FromState(States.FloatDigitAccepted)
                .OnCharacterRange('0', '9')
                .Recurse()

            .FromState(States.FloatDigitAccepted)
                .OnAnyCharacter()
                .ExceptRange('0', '9')
                .Accept(States.FloatAccepted)
                ;
    }

    private void StringLexemes(TokenizerDfaBuilder builder)
    {
        builder.SetCharset(CharsetType.Ascii);

        builder
            .FromInitialState()
            .OnCharacter('"')
            .GoTo(States.StringStart)
            ;

        builder.SetCharset(CharsetType.Utf16);

        builder
            .FromState(States.StringStart)
            .OnAnyCharacterExcept('"')
            .Recurse()
            ;

        builder
            .FromState(States.StringStart)
            .OnCharacter('"')
            .GoTo(States.StringEnd)
            ;

        builder.SetCharset(CharsetType.Ascii);

        builder
            .FromState(States.StringEnd)
            .OnAnyCharacter()
            .Accept(States.StringAccepted)
            ;

        /**/

        //builder.SetCharset(CharsetType.Ascii);

        //builder
        //    .FromInitialState()
        //    .OnCharacter('\'')
        //    .GoTo(States.StringStart)
        //    ;

        //builder
        //    .FromState(States.StringStart)
        //    .OnCharacter('\'')
        //    .GoTo(States.StringEnd)
        //    ;
    }

    private void IdentifierLexeme(TokenizerDfaBuilder builder)
    {
        builder
            .FromInitialState()
            .OnCharacter('_')
            .OnCharacterRange('a', 'z')
            .OnCharacterRange('A', 'Z')
            .GoTo(States.IdentifierStart)
            ;

        builder
            .FromState(States.IdentifierStart)
            .OnCharacter('_')
            .OnCharacterRange('a', 'z')
            .OnCharacterRange('A', 'Z')
            .OnCharacterRange('0', '9')
            .Recurse()

            ;
        builder
            .FromState(States.IdentifierStart)
            .OnAnyCharacter()
            .ExceptRange('0', '9')
            .Except('_')
            .ExceptRange('a', 'z')
            .ExceptRange('A', 'Z')
            .Accept(States.IdentifierAccepted)
            ;
    }

    private void PuntuactionLexemes(TokenizerDfaBuilder builder)
    {
        var charset = TokenizerDfaBuilder.ComputeCharset(CharsetType.Ascii);

        builder.SetCharset(CharsetType.Ascii);

        foreach (var c in charset)
        {
            if (!char.IsPunctuation(c))
            {
                continue;
            }

            if (c == '"')
            {
                continue;
            }

            if (c == '\'')
            {
                continue;
            }

            if (c == '_')
            {
                continue;
            }

            if (c == '-')
            {
                continue;
            }

            builder
                .FromInitialState()
                .OnCharacter(c)
                .GoTo($"punctuation '{c}' found")
                ;

            builder
                .FromState($"punctuation '{c}' found")
                .OnAnyCharacter()
                .Accept($"{c}")
                ;
        }

    }

    private void SymbolLexemes(TokenizerDfaBuilder builder)
    {
        var charset = TokenizerDfaBuilder.ComputeCharset(CharsetType.Ascii);

        builder.SetCharset(CharsetType.Ascii);

        foreach (var c in charset)
        {
            if (!char.IsSymbol(c))
            {
                continue;
            }

            if (c == '"')
            {
                continue;
            }

            if (c == '\'')
            {
                continue;
            }

            if (c == '_')
            {
                continue;
            }

            if (c == '-')
            {
                continue;
            }

            builder
                .FromInitialState()
                .OnCharacter(c)
                .GoTo($"symbol '{c}' found")
                ;

            builder
                .FromState($"symbol '{c}' found")
                .OnAnyCharacter()
                .Accept($"{c}")
                ;
        }

    }

    /* comments */

    private void CStyleInlineComment(TokenizerDfaBuilder builder)
    {
        builder
            .FromInitialState()
            .OnCharacter('/')
            .GoTo("comment_start")
            ;

        builder
            .FromState("comment_start")
            .OnCharacter('/')
            .GoTo("comment_body")
            ;

        builder
            .FromState("comment_body")
            .OnAnyCharacterExcept('\n')
            .Recurse()
            ;

        builder
            .FromState("comment_body")
            .OnCharacter('\n')
            .Accept("comment")
            ;
    }

    private void CSyleBlockComment(TokenizerDfaBuilder builder)
    {
        builder
            .FromInitialState()
            .OnCharacter('/')
            .GoTo("block_comment_start")
            ;

        builder
            .FromState("block_comment_start")
            .OnCharacter('*')
            .GoTo("block_comment_body")
            ;

        builder
            .FromState("block_comment_body")
            .OnAnyCharacterExcept('*')
            .Recurse()
            ;

        builder
            .FromState("block_comment_body")
            .OnCharacter('*')
            .GoTo("block_comment_end_star")
            ;

        builder
            .FromState("block_comment_end_star")
            .OnAnyCharacterExcept('/')
            .GoTo("block_comment_body")
            ;

        builder
            .FromState("block_comment_end_star")
            .OnCharacter('/')
            .Accept("block_comment")
            ;
    }

}

internal static class States
{
    public static string InitialState { get; } = "initial_state";

    public static string MinusSign { get; } = "minus_sign";
    public static string MinusSignAccepted { get; } = "-";

    public static string IntDigitAccepted { get; } = "int_digit_accepted";
    public static string IntAccepted { get; } = "int";

    public static string FloatStart { get; } = "float_start";
    public static string FloatDigitAccepted { get; } = "float_digit_accepted";
    public static string FloatAccepted { get; } = "float";

    public static string StringStart { get; } = "string_start";
    public static string StringEnd { get; } = "string_end";
    public static string StringAccepted { get; } = "string";

    public static string IdentifierStart { get; } = "identifier_start";
    public static string IdentifierAccepted { get; } = "identifier";
}