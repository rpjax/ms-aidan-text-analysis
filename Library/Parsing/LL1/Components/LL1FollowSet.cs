using System.Collections;
using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Parsing.LL1.Components;

public class LL1FollowSet : IEnumerable<Symbol>
{
    public NonTerminal Symbol { get; }
    public Symbol[] Follows { get; }

    public LL1FollowSet(NonTerminal symbol, Symbol[] follows)
    {
        Symbol = symbol;
        Follows = follows;
    }

    public int Length => Follows.Length;

    public override string ToString()
    {
        var follows = string.Join(", ", Follows.Select(x => x.ToString()));
        return $"Follow({Symbol}): {{ {follows} }}";
    }

    public IEnumerator<Symbol> GetEnumerator()
    {
        return ((IEnumerable<Symbol>)Follows).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Follows.GetEnumerator();
    }
}

