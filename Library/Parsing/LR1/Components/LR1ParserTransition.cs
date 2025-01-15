using Aidan.TextAnalysis.Language.Components.Symbols;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/*
 * FROM 'StateId' ON 'Symbol' DO 'Action'
 */

/// <summary>
/// Represents a transition in the LR(1) parser table.
/// </summary>
public class LR1ParserTransition
{
    /// <summary>
    /// Gets the state ID from which the transition occurs.
    /// </summary>
    public uint StateId { get; }

    /// <summary>
    /// Gets the symbol on which the transition occurs.
    /// </summary>
    public ISymbol Symbol { get; }

    /// <summary>
    /// Gets the action to be performed on the transition.
    /// </summary>
    public LR1Action Action { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LR1ParserTransition"/> class.
    /// </summary>
    /// <param name="stateId">The state ID from which the transition occurs.</param>
    /// <param name="symbol">The symbol on which the transition occurs.</param>
    /// <param name="action">The action to be performed on the transition.</param>
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
