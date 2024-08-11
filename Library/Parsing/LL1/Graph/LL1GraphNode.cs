using System.Collections;
using System.Text;
using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Language.Graph;

public class LL1GraphNode : IEnumerable<LL1GraphNode>
{
    public Symbol Symbol { get; }
    public ProductionRule? Production { get; }
    public LL1GraphNode? Parent { get; internal set; }
    public RecursionType RecursionType { get; internal set; }
    public int Position { get; }

    internal List<LL1GraphNode> Children { get; set; } 

    public LL1GraphNode(
        Symbol symbol, 
        ProductionRule? production = null, 
        LL1GraphNode? parent = null,
        RecursionType recursionType = RecursionType.None,
        IEnumerable<LL1GraphNode>? children = null)
    {
        Symbol = symbol;
        Production = production;
        Parent = parent;
        RecursionType = recursionType;
        Children = children?.ToList() ?? new();
        Position = GetPosition();
    }

    public bool IsRecursive => RecursionType != RecursionType.None;

    public LL1GraphNode this[int index]
    {
        get
        {
            if (index < 0 || index >= Children.Count)
            {
                throw new IndexOutOfRangeException();
            }

            return Children[index];
        }
        set
        {
            if (index < 0 || index >= Children.Count)
            {
                throw new IndexOutOfRangeException();
            }

            Children[index] = value;
            value.Parent = this;
        }
    }

    public bool IsRoot => Parent is null;
    public int ChildrenCount => Children.Count;

    public override string ToString()
    {
        if(Production is not null)
        {
            return $"({Symbol}) At {Position} From ({Production})";
        }

        return $"{Symbol}";
    }

    public IEnumerator<LL1GraphNode> GetEnumerator()
    {
        return Children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int IndexOf(Symbol symbol)
    {
        for (var i = 0; i < Children.Count; i++)
        {
            if (ReferenceEquals(Children[i].Symbol, symbol))
            {
                return i;
            }
        }

        return -1;
    }

    private int GetPosition()
    {
        if(Production is null)
        {
            return -1;
        }

        for (var i = 0; i < Production.Value.Body.Length; i++)
        {
            if (ReferenceEquals(Production.Value.Body[i], Symbol))
            {
                return i;
            }
        }

        return -1;
    }

    public bool IsLeftmostNonTerminal()
    {
        if (Production is null)
        {
            return false;
        }
        if (!Symbol.IsNonTerminal)
        {
            return false;
        }

        for (var i = 0; i < Production.Value.Body.Length; i++)
        {
            if (ReferenceEquals(Production.Value.Body[i], Symbol))
            {
                return true;
            }
            if (Production.Value.Body[i].IsNonTerminal)
            {
                return false;
            }
        }

        return false;
    }       

    public Sentence GetPrefix()
    {
        var prefix = new List<Symbol>();

        for (var i = 0; i < Position; i++)
        {
            prefix.Add(Production!.Value.Body[i]);
        }

        return new Sentence(prefix.ToArray());
    }

    public Sentence GetSuffix()
    {
        var suffix = new List<Symbol>();

        for (var i = Position + 1; i < Production!.Value.Body.Length; i++)
        {
            suffix.Add(Production.Value.Body[i]);
        }

        return new Sentence(suffix.ToArray());
    }

}
