using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Parsing.LL1.Components;

public class LL1FollowTable
{
    private Dictionary<NonTerminal, LL1FollowSet> Table { get; }

    public LL1FollowTable(LL1FollowSet[] sets)
    {
        Table = sets.ToDictionary(x => x.Symbol);
    }

    public LL1FollowSet Lookup(NonTerminal symbol)
    {
        if (!Table.ContainsKey(symbol))
        {
            throw new InvalidOperationException("The symbol is not in the table.");
        }

        return Table[symbol];
    }

    public override string ToString()
    {
        return string.Join("\n", Table.Values.Select(x => x.ToString()));
    }

}

