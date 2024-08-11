using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

public class LR1Grammar : Grammar
{
    public LR1Grammar(NonTerminal start, IEnumerable<ProductionRule> productions) : base(start, productions)
    {
    }
}
