using Aidan.Core.Patterns;
using Aidan.TextAnalysis.RegularExpressions.Automata;
using Aidan.TextAnalysis.Tokenization;
using Aidan.TextAnalysis.Tokenization.StateMachine;
using Aidan.TextAnalysis.Tokenization.StateMachine.Builders;

namespace Aidan.TextAnalysis.GDef.Tokenization;

public class GrammarTokenizerBuilder : IBuilder<Tokenizer>
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

    public Tokenizer Build()
    {
        var lexemes = GDefLexemes.GetLexemes();
        var ignoredChars = new char[] { ' ', '\t', '\n', '\r', '\0' };
        var calculator = new TokenizerCalculator(lexemes, ignoredChars);
        var table = calculator.ComputeTokenizerTable();
        var builder = new ManualTokenizerBuilder(table);

        builder.SetCharset(calculator.GetAlphabet());

        StringLexemes(builder);
        //CStyleComment(builder);

        builder.EnableDebugger();

        return builder.Build();
    }

    /* tokenization rules */

    private void StringLexemes(ManualTokenizerBuilder builder)
    {
        var delimiters = GDefLexemes.StringDelimiters;
        var escapeChar = '\\';

        foreach (var delimiter in delimiters)
        {
            var stringState = $"{delimiter}-string start";
            var stringEndState = $"{delimiter}-string end";
            var escapeState = $"{delimiter}-string escape char";
            var acceptName = GDefLexemes.String;
            
            builder.FromInitialState()
                .OnCharacter(delimiter)
                .GoTo(stringState);
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
            .Accept()
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
            .Accept()
            ;

    }

}

