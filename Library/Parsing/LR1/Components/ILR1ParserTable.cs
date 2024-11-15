using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents a table used by an LR(1) parser to determine parsing actions and production rules.
/// </summary>
public interface ILR1ParserTable
{
    /// <summary>
    /// Looks up the action to be taken for a given state and symbol.
    /// </summary>
    /// <param name="state">The current state of the parser.</param>
    /// <param name="symbol">The current input symbol.</param>
    /// <returns>The action to be taken, or <c>null</c> if no action is defined for the given state and symbol.</returns>
    LR1Action? Lookup(uint state, ISymbol symbol);

    /// <summary>
    /// Looks up the production rule for a given index.
    /// </summary>
    /// <param name="index">The index of the production rule.</param>
    /// <returns>The production rule corresponding to the given index.</returns>
    IProductionRule LookupProduction(uint index);
}
