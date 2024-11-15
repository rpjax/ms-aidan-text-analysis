using Aidan.Core.Patterns;
using Aidan.TextAnalysis.RegularExpressions.Automata;
using Aidan.TextAnalysis.Tokenization;
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

    public Tokenizer Build()
    {
        var lexemes = GDefLexemes.GetLexemes();
        var ignoredChars = new char[] { ' ', '\t', '\n', '\r', '\0' };

        var calculator = new TokenizerCalculator(lexemes, ignoredChars);
        var table = calculator.ComputeTokenizerTable();

        var builder = new ManualTokenizerBuilder(
            table: table, 
            charset: calculator.GetAlphabet());

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

        /* single quote string */
        builder.FromInitialState()
            .OnCharacter('\'')
            .GoTo("single-quote-string");

        builder.FromState("single-quote-string")
            .RecurseOnNoTransition();

        builder.FromState("single-quote-string")
            .OnCharacter(escapeChar)
            .GoTo("single-quote-string-escape");

        builder.FromState("single-quote-string-escape")
            .OnCharacter('\'')
            .OnCharacter(escapeChar)
            .GoTo("single-quote-string");

        builder.FromState("single-quote-string")
            .OnCharacter('\'')
            .GoTo(GDefLexemes.String);

        /* double quote string */
        builder.FromInitialState()
             .OnCharacter('\"')
             .GoTo("double-quote-string");

        builder.FromState("double-quote-string")
            .RecurseOnNoTransition();

        builder.FromState("double-quote-string")
            .OnCharacter(escapeChar)
            .GoTo("double-quote-string-escape");

        builder.FromState("double-quote-string-escape")
            .OnCharacter('\"')
            .OnCharacter(escapeChar)
            .GoTo("double-quote-string");

        builder.FromState("double-quote-string")
            .OnCharacter('\"')
            .GoTo(GDefLexemes.String);

        builder.FromState(GDefLexemes.String)
            .Accept();
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

    }

}

