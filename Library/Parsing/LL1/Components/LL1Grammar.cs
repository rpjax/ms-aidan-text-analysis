using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Parsing.LL1.Tools;

namespace Aidan.TextAnalysis.Parsing.LL1.Components;

public class LL1Grammar : Grammar
{
    private LL1ParsingTable? ParsingTable { get; set; }

    public LL1Grammar(NonTerminal start, IEnumerable<ProductionRule> productions) : base(start, productions)
    {
    }

    public LL1Grammar(ProductionSet set) : base(set)
    {
    }

    public void ComputeParsingTable()
    {
        ParsingTable = LL1ParsingTableTool.ComputeLL1ParsingTable(Productions);
    }

    public LL1ParsingTable GetParsingTable()
    {
        if (ParsingTable == null)
        {
            ParsingTable = LL1ParsingTableTool.ComputeLL1ParsingTable(Productions);
        }

        return ParsingTable;
    }

}

