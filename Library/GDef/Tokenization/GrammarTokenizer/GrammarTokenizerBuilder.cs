using Aidan.Core.Patterns;
using Aidan.TextAnalysis.Tokenization.StateMachine;

namespace Aidan.TextAnalysis.GDef.Tokenization;

public class GrammarTokenizerBuilder : IBuilder<TokenizerMachine>
{
    internal static class States
    {
        public static string InitialState { get; } = "initial_state";
    }

    /*
        identifier
        colon
        semicolon
        strings for terminal symbols
        '$' for referencing lexemes
        '{' and '}' for repetition macros
        '[' and ']' for option macros
        '(' and ')' for group macros
        '|' for alternative macros
     */
    public TokenizerMachine Build()
    {
        var builder = new TokenizerDfaBuilder(initialStateName: States.InitialState);

        SkipWhitespace(builder);
        SkipControlChars(builder);
        Identifier(builder);
        SpecialSymbols(builder);
        StringLexemes(builder);
        CStyleComment(builder);

        builder.EnableDebugger();

        return builder.Build();
    }

    private char[] GetSpecialSymbols()
    {
        return new char[]
        {
            ':', ';', '$', '{', '}', '[', ']', '(', ')', '|'
        };
    }

    private char[] GetStringDelimiters()
    {
        return new char[]
        {
            '"', '\''
        };
    }

    private void SkipWhitespace(TokenizerDfaBuilder builder)
    {
        builder.FromInitialState()
            .OnWhitespace()
            .Recurse()
            ;
    }

    private void SkipControlChars(TokenizerDfaBuilder builder)
    {
        builder
            .FromInitialState()
            .OnCharacter('\r')
            .OnCharacter('\n')
            .OnCharacter('\t')
            .Recurse()
            ;
    }

    private void Identifier(TokenizerDfaBuilder builder)
    {
        var identifierStart = "identifier_start";
        var acceptName = "identifier";

        builder
            .FromInitialState()
            .OnCharacter('_')
            .OnCharacterRange('a', 'z')
            .OnCharacterRange('A', 'Z')
            .GoTo(identifierStart)
            ;

        builder
            .FromState(identifierStart)
            .OnCharacter('_')
            .OnCharacterRange('a', 'z')
            .OnCharacterRange('A', 'Z')
            .OnCharacterRange('0', '9')
            .Recurse()

            ;
        builder
            .FromState(identifierStart)
            .OnAnyCharacter()
            .ExceptRange('0', '9')
            .Except('_')
            .ExceptRange('a', 'z')
            .ExceptRange('A', 'Z')
            .Accept(acceptName)
            ;
    }

    private void SpecialSymbols(TokenizerDfaBuilder builder)
    {
        foreach (var c in GetSpecialSymbols())
        {
            var stateName = $"special char '{c}' found";
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

    private void StringLexemes(TokenizerDfaBuilder builder)
    {
        var delimiters = GetStringDelimiters();
        var escapeChar = '\\';

        builder.SetCharset(CharsetType.Ascii);

        foreach (var delimiter in delimiters)
        {
            var stringState = $"{delimiter}-string start";
            var stringEndState = $"{delimiter}-string end";
            var escapeState = $"{delimiter}-string escape char";
            var acceptName = "string";

            var otherDelimiters = delimiters
                .Where(d => d != delimiter)
                .ToArray();

            builder
                .FromInitialState()
                .OnCharacter(delimiter)
                .GoTo(stringState)
                ;

            builder.SetCharset(CharsetType.Utf16);

            builder
                .FromState(stringState)
                .OnAnyCharacterExcept(delimiter, escapeChar)
                .Recurse()
                ;

            builder
                .FromState(stringState)
                .OnCharacter(delimiter)
                .GoTo(stringEndState)
                ;

            /* escape character */
            builder
                .FromState(stringState)
                .OnCharacter(escapeChar)
                .GoTo(escapeState)
                ;

            builder
                .FromState(escapeState)
                .OnAnyCharacter()
                .GoTo(stringState)
                ;

            builder.SetCharset(CharsetType.Ascii);

            builder
                .FromState(stringEndState)
                .OnAnyCharacter()
                .Accept(acceptName)
                ;
        }
    }

    /* comments */

    private void CStyleComment(TokenizerDfaBuilder builder)
    {
        var cStyleCommentStart = "c_style_comment_start";

        var cStyleInlineComment = "c_style_inline_comment";
        var cStyleInlineCommentEnd = "c_style_inline_comment_end";

        var cStyleBlockComment = "c_style_block_comment";
        var cStyleBlockCommentEnd = "c_style_block_comment_end";
        var acceptName = "comment";

        //builder.SetCharset(CharsetType.Utf8);

        /* on '/' goto c style comment start */
        builder
            .FromInitialState()
            .OnCharacter('/')
            .GoTo(cStyleCommentStart)
            ;

        /* on the second '/' goto inline comment */
        builder
            .FromState(cStyleCommentStart)
            .OnCharacter('/')
            .GoTo(cStyleInlineComment)
            ;

        /* on '*' goto block comment */
        builder
            .FromState(cStyleCommentStart)
            .OnCharacter('*')
            .GoTo(cStyleBlockComment)
            ;

        /* from inline comment, recurse on any character except '\n' */
        builder
            .FromState(cStyleInlineComment)
            .OnAnyCharacterExcept('\n')
            .Recurse()
            ;

        /* from inline comment, on '\n' accept the comment */
        builder
            .FromState(cStyleInlineComment)
            .OnCharacter('\n')
            .GoTo(cStyleInlineCommentEnd)
            ;

        builder
            .FromState(cStyleInlineCommentEnd)
            .OnAnyCharacter()
            .Accept(acceptName)
            ;

        /* from block comment, recurse on any character except '*' */
        builder
            .FromState(cStyleBlockComment)
            .OnAnyCharacterExcept('*')
            .Recurse()
            ;

        /* from block comment, on '*' go to block comment end */
        builder
            .FromState(cStyleBlockComment)
            .OnCharacter('*')
            .GoTo(cStyleBlockCommentEnd)
            ;

        /* from block comment end, recurse on any character except '/' */
        builder
            .FromState(cStyleBlockCommentEnd)
            .OnAnyCharacterExcept('/')
            .GoTo(cStyleBlockComment)
            ;

        /* from block comment end, on '/' accept the comment */
        builder
            .FromState(cStyleBlockCommentEnd)
            .OnCharacter('/')
            .GoTo("c_style_block_comment_finilizer")
            ;

        builder
            .FromState("c_style_block_comment_finilizer")
            .OnAnyCharacter()
            .Accept(acceptName)
            ;

    }

}
