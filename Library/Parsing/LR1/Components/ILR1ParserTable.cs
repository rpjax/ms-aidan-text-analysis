using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/* refactored classes */

public interface ILR1ParserTable
{
    LR1Action? Lookup(uint state, ISymbol symbol);
    IProductionRule LookupProduction(uint index);
}
