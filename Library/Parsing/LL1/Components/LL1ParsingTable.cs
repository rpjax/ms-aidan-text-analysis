using Aidan.TextAnalysis.Tokenization;

namespace Aidan.TextAnalysis.Language.Components;

public class LL1ParsingTableEntry
{
    public NonTerminal State { get; }
    public Terminal Lookahead { get; }
    public ProductionRule Production { get; }

    public LL1ParsingTableEntry(NonTerminal state, Terminal lookahead, ProductionRule production)
    {
        State = state;
        Lookahead = lookahead;
        Production = production;
    }

    public string CreateKey()
    {
        var strategy = Lookahead.Value != null
            ? LL1ParsingTable.KeyStrategy.TypeAndValue
            : LL1ParsingTable.KeyStrategy.TypeOnly;

        return LL1ParsingTable.CreateKey(State, Lookahead, strategy);
    }
}

public class LL1ParsingTable
{
    public enum KeyStrategy
    {
        TypeAndValue,
        TypeOnly
    }

    private Dictionary<string, ProductionRule> InternalEntries = new();

    public LL1ParsingTable(IEnumerable<LL1ParsingTableEntry> entries)
    {
        InternalEntries = entries
            .ToDictionary(x => x.CreateKey(), x => x.Production);
    }

    public IReadOnlyDictionary<string, ProductionRule> Entries => InternalEntries;

    internal static string CreateKey(string state, TokenType type, string? value, KeyStrategy strategy)
    {
        switch (strategy)
        {
            case KeyStrategy.TypeAndValue:
                return $"{state} + {type}({value?.ToString()})";

            case KeyStrategy.TypeOnly:
                return $"{state} + {type}";

            default:
                throw new InvalidOperationException();
        }
    }

    internal static string CreateKey(NonTerminal state, Terminal lookahead, KeyStrategy strategy)
    {
        return CreateKey(state.Name, lookahead.Type, lookahead.Value, strategy);
    }

    public override string ToString()
    {
        var keys = InternalEntries.Keys
            .Select(x => x)
            ;
        var values = InternalEntries.Values
            .Select(x => x.ToString())
            ;

        return string.Join("\n", keys.Zip(values, (k, v) => $"[{k}] = ({v})"));
    }

    public bool Lookup(NonTerminal state, Terminal lookahead, out ProductionRule production)
    {
        if (InternalEntries.TryGetValue(CreateKey(state, lookahead, KeyStrategy.TypeAndValue), out var value1))
        {
            production = value1;
            return true;
        }

        if (InternalEntries.TryGetValue(CreateKey(state, lookahead, KeyStrategy.TypeOnly), out var value2))
        {
            production = value2;
            return true;
        }

        production = default;
        return false;
    }

}
