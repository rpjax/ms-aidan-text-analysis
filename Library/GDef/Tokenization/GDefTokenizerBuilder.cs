using Aidan.Core.Patterns;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.RegularExpressions.Automata;
using Aidan.TextAnalysis.Tokenization;
using Aidan.TextAnalysis.Tokenization.StateMachine.Builders;

namespace Aidan.TextAnalysis.GDef.Tokenization;

public class GDefTokenizerBuilder : IBuilder<Tokenizer>
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

    private Charset Charset { get; set; }

    public GDefTokenizerBuilder(Charset? charset = null)
    {
        Charset = charset ?? Charset.Compute(CharsetType.ExtendedAscii);
    }

    public Tokenizer Build()
    {
        var lexemes = GDefLexemes.GetLexemes();
        var ignoredChars = new char[] { ' ', '\t', '\n', '\r', '\0' };
        var calculator = new TokenizerCalculator(lexemes, ignoredChars);
        var table = calculator.ComputeTokenizerTable();
        var builder = new ManualTokenizerBuilder(table);

        AddComments(builder);

        return builder.Build();
    }

    public void AddComments(ManualTokenizerBuilder builder)
    {
        var charset = Charset.Compute(CharsetType.ExtendedAscii);
        var barSeen = "/";

        builder.SetCharset(charset);

        builder.FromInitialState()
            .OnCharacter('/')
            .GoTo(barSeen);

        builder.FromState(barSeen)
            .OnCharacter('/')
            .GoTo("c_style_inline_comment");

        builder.FromState("c_style_inline_comment")
            .OnAnyCharacter()
            .Except('\n')
            .Recurse();

        builder.FromState("c_style_inline_comment")
            .OnCharacter('\n')
            .GoTo(GDefLexemes.Comment);
        
        builder.FromState(barSeen)
            .OnCharacter('*')
            .GoTo("c_style_block_comment");

        builder.FromState("c_style_block_comment")
            .OnAnyCharacter()
            .Except('*')
            .Recurse();

        builder.FromState("c_style_block_comment")
            .OnCharacter('*')
            .GoTo("c_style_block_comment_'*'_seen");

        builder.FromState("c_style_block_comment_'*'_seen")
            .OnAnyCharacter()
            .Except('/')
            .GoTo("c_style_block_comment");

        builder.FromState("c_style_block_comment_'*'_seen")
            .OnCharacter('/')
            .GoTo(GDefLexemes.Comment);

        builder.FromState(GDefLexemes.Comment)
            .Accept();
    }

}
