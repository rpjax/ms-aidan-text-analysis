using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/*
 * FROM 'StateId' ON 'Symbol' DO 'Action'
 */

public class LR1ParserTransition
{
    public uint StateId { get; }
    public ISymbol Symbol { get; }
    public LR1Action Action { get; }

    public LR1ParserTransition(
        uint stateId, 
        ISymbol symbol, 
        LR1Action action)
    {
        StateId = stateId;
        Symbol = symbol;
        Action = action;
    }
}
