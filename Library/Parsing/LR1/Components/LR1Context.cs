using Aidan.Core;
using Aidan.Core.Exceptions;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Parsing.Components;
using Aidan.TextAnalysis.Tokenization;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents the context of a LR(1) parser execution. It contains the input stream, the stack and the concrete syntax tree (CST) builder.
/// </summary>
public class LR1Context
{
    public LR1ParsingTable ParsingTable { get; }
    public InputStream InputStream { get; }
    public LR1Stack Stack { get; }
    public CstBuilder CstBuilder { get; }

    /// <summary>
    /// Creates a new instance of <see cref="LR1Context"/>.
    /// </summary>
    /// <param name="inputStream"></param>
    /// <param name="stack"></param>
    /// <param name="cstBuilder"></param>
    public LR1Context(LR1ParsingTable parsingTable, InputStream inputStream, LR1Stack stack, CstBuilder cstBuilder)
    {
        ParsingTable = parsingTable;
        InputStream = inputStream;
        Stack = stack;
        CstBuilder = cstBuilder;
    }

    public Exception UnexpectedEndOfTokens()
    {
        var error = new ErrorBuilder()
            .SetTitle("Unexpected end of tokens.")
            .SetCode("SYNTAX_ERROR")
            .AddDetail("Stack Trace", InputStream.LookaheadToken?.ToStringVerbose())
            .Build()
            ;

        return new ErrorException(error);
    }

    public Error SyntaxError(int state, Token lookahead)
    {
        var expectedSymbols = ParsingTable[state].State.Items
            .Where(x => x.Symbol?.IsTerminal is true || true)
            .Select(x => new { Symbol = x.Symbol ?? Epsilon.Instance, Rule = x.GetSignature(useLookaheads: false) })
            .ToArray();
        ;

        var expectedSymbolsText = string.Join($",{Environment.NewLine}or ", expectedSymbols.Select(x => $"{x.Symbol} to form {x.Rule}"));

        return new ErrorBuilder()
            .SetTitle("Syntax error.")
            .SetCode("SYNTAX_ERROR")
            .AddDetail("At", InputStream.LookaheadToken?.ToStringVerbose())
            .AddDetail("Expected", expectedSymbolsText)
            .Build()
            ;
    }

    public Exception SyntaxException(int state, Token lookahead)
    {
        return new ErrorException(SyntaxError(state, lookahead));
    }

}
