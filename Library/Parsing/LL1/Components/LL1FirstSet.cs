using System.Collections;
using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Parsing.LL1.Components;

public class LL1FirstSet : IEnumerable<Symbol>
{
    public NonTerminal Symbol { get; }
    public Symbol[] Firsts { get; }

    public LL1FirstSet(NonTerminal symbol, Symbol[] firsts)
    {
        Symbol = symbol;
        Firsts = firsts;
    }

    public IEnumerator<Symbol> GetEnumerator()
    {
        return ((IEnumerable<Symbol>)Firsts).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<Symbol>)Firsts).GetEnumerator();
    }

    public override string ToString()
    {
        var firsts = string.Join(", ", Firsts.Select(x => x.ToString()));
        return $"First({Symbol}): {{ {firsts} }}";
    }

    public bool Overlaps(LL1FirstSet other)
    {
        return Firsts.Any(x => x != Epsilon.Instance && other.Firsts.Contains(x));
    }

    public bool Overlaps(LL1FollowSet followSet)
    {
        return Firsts.Any(x => x != Epsilon.Instance && followSet.Contains(x));
    }

    public LL1FirstSet RemoveEpislon()
    {
        return new LL1FirstSet(Symbol, Firsts.Where(x => x != Epsilon.Instance).ToArray());
    }

}
