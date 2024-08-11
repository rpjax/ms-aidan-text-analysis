using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Parsing.LL1.Components;

public class LL1FirstTable
{
    private Dictionary<NonTerminal, LL1FirstSet> Entries { get; }

    public LL1FirstTable(LL1FirstSet[] sets)
    {
        Entries = sets.ToDictionary(x => x.Symbol);
    }

    public LL1FirstSet Lookup(NonTerminal symbol)
    {
        if (!Entries.ContainsKey(symbol))
        {
            throw new InvalidOperationException("The symbol is not in the table.");
        }

        return Entries[symbol];
    }

    public override string ToString()
    {
        return string.Join("\n", Entries.Values.Select(x => x.ToString()));
    }

}
