using Aidan.Core.Patterns;
using Aidan.TextAnalysis.RegularExpressions.Automata;
using Aidan.TextAnalysis.Tokenization.StateMachine;

namespace Aidan.TextAnalysis.GDef.Tokenization;

public class GrammarTokenizerBuilder : IBuilder<TokenizerMachine>
{
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

    private Lexeme[] Lexemes { get; }
    private char[] IgnoredChars { get; }

    public GrammarTokenizerBuilder()
    {
        Lexemes = GDefLexemes.GetLexemes();
        IgnoredChars = new char[] { ' ', '\t', '\n', '\r', '\0' };
    }

    public TokenizerMachine Build()
    {
        var lexemes = GDefLexemes.GetLexemes();
        var ignoredChars = new char[] { ' ', '\t', '\n', '\r', '\0' };
        var calculator = new DfaCalculator(lexemes, ignoredChars);
        var dfa = calculator.ComputeDfa();
        var builder = new TokenizerDfaBuilder();

        //builder.SetCharset(calculator.GetAlphabet());

        //StringLexemes(builder);
        //CStyleComment(builder);

        builder.EnableDebugger();

        return builder.Build();
    }

    /* tokenization rules */

    private void StringLexemes(TokenizerDfaBuilder builder)
    {
        var delimiters = GDefLexemes.StringDelimiters;
        var escapeChar = '\\';

        var originalCharset = builder.GetCharset();

        var workingCharset = originalCharset
            .Concat(delimiters)
            .Distinct()
            .ToArray();

        var stringCharset = TokenizerDfaBuilder.ComputeCharset(CharsetType.Ascii);

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

            builder
                .FromInitialState()
                .OnCharacter(delimiter)
                .GoTo(stringState)
                ;

            builder.SetCharset(stringCharset);

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

            builder.SetCharset(workingCharset);

            builder
                .FromState(stringEndState)
                .OnAnyCharacter()
                .Accept(acceptName)
                ;
        }

        builder.SetCharset(originalCharset);
    }

    /* comments */

    private void CStyleComment(TokenizerDfaBuilder builder)
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

        var commentCharset = TokenizerDfaBuilder.ComputeCharset(CharsetType.Ascii);

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
