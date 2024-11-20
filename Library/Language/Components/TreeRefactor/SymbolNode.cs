using Aidan.TextAnalysis.Parsing.LR1.Components;
using System.Xml.Linq;

namespace Aidan.TextAnalysis.Language.Components.TreeRefactor;

public enum SymbolNodeType
{
    Terminal,
    NonTerminal,
    Epsilon,
    Grouping,
    Union,
    Nullable,
    ZeroOrMore,
    OneOrMore,
}

public abstract class SymbolNode
{
    public SymbolNodeType Type { get; }
    public SymbolNode? Parent { get; }

    public SymbolNode(SymbolNodeType type)
    {
        Type = type;
    }

    public bool ProducesEpsilonGetter => ProducesEpsilon();

    public abstract override string ToString();
    public abstract bool ProducesEpsilon();
    

    //public abstract IReadOnlyList<SymbolNode> GetChildren();

}

public class TerminalNode : SymbolNode
{
    public string Name { get; }

    public TerminalNode(string name)
        : base(SymbolNodeType.Terminal)
    {
        Name = name;
    }

    public override string ToString()
    {
        return $"'{Name}'";
    }

    public override bool ProducesEpsilon()
    {
        return false;
    }
}

public class EoiNode : TerminalNode
{
    public EoiNode() : base("\0") { }
}

public class NonTerminalNode : SymbolNode
{
    public string Name { get; }
    public SymbolNode[] Children { get; private set; }

    public NonTerminalNode(string name)
        : base(SymbolNodeType.NonTerminal)
    {
        Name = name;
        Children = new SymbolNode[0];
    }

    public override string ToString()
    {
        return $"{Name}";
        return $"{string.Join(' ', Children.Select(x => x.ToString()))}";
    }

    public override bool ProducesEpsilon()
    {
        if (Children.Length == 0)
        {
            return true;
        }

        return Children[0].ProducesEpsilon();
    }

    public NonTerminalNode AddChildren(params SymbolNode[] children)
    {
        Children = Children
            .Concat(children)
            .ToArray();
        return this;
    }

}

public class EpsilonNode : SymbolNode
{
    public EpsilonNode()
    : base(SymbolNodeType.Epsilon)
    {

    }

    public override string ToString()
    {
        return "ε";
    }

    public override bool ProducesEpsilon()
    {
        return true;
    }

}

public class GroupingNode : SymbolNode
{
    public SymbolNode[] Children { get; private set; }
    public bool ShowParenteses { get; set; }

    public GroupingNode(params SymbolNode[] children)
        : base(SymbolNodeType.Grouping)
    {
        Children = children;
        ShowParenteses = true;
    }

    public override string ToString()
    {
        if (ShowParenteses)
        {
            return $"({string.Join(' ', Children.Select(x => x.ToString()))})";
        }

        return $"{string.Join(' ', Children.Select(x => x.ToString()))}";
    }

    public override bool ProducesEpsilon()
    {
        if (Children.Length == 0)
        {
            return true;
        }

        return Children[0].ProducesEpsilon();
    }

    public void AddChildren(params SymbolNode[] children)
    {
        Children = Children
            .Concat(children)
            .ToArray();
    }
}

public class UnionNode : SymbolNode
{
    public SymbolNode[] Children { get; private set; }

    public UnionNode(params SymbolNode[] children)
        : base(SymbolNodeType.Union)
    {
        Children = children;
    }

    public override string ToString()
    {
        return $"({string.Join(" | ", Children.Select(x => x.ToString()))})";
    }

    public override bool ProducesEpsilon()
    {
        if (Children.Length == 0)
        {
            return true;
        }

        return Children
            .Any(x => x.ProducesEpsilon());
    }

    public void AddChildren(params SymbolNode[] children)
    {
        Children = Children
            .Concat(children)
            .ToArray();
    }

}

public class NullableNode : SymbolNode
{
    public SymbolNode Child { get; private set; }

    public NullableNode(SymbolNode child)
        : base(SymbolNodeType.Nullable)
    {
        Child = child;
    }

    public override string ToString()
    {
        return $"{Child}?";
    }

    public override bool ProducesEpsilon()
    {
        return true;
    }

    public void SetChild(SymbolNode child)
    {
        Child = child;
    }

}

public class ZeroOrMoreNode : SymbolNode
{
    public SymbolNode Child { get; private set; }

    public ZeroOrMoreNode(SymbolNode child)
        : base(SymbolNodeType.ZeroOrMore)
    {
        Child = child;
    }

    public override string ToString()
    {
        return $"{Child}*";
    }

    public override bool ProducesEpsilon()
    {
        return true;
    }

    public void SetChild(SymbolNode child)
    {
        Child = child;
    }

}

public class OneOrMoreNode : SymbolNode
{
    public SymbolNode Child { get; private set; }

    public OneOrMoreNode(SymbolNode child)
        : base(SymbolNodeType.OneOrMore)
    {
        Child = child;
    }

    public override string ToString()
    {
        return $"{Child}+";
    }

    public override bool ProducesEpsilon()
    {
        return Child.ProducesEpsilon();
    }

    public void SetChild(SymbolNode child)
    {
        Child = child;
    }

}

public class LRItem
{
    public NonTerminalNode NonTerminal { get; }
    public uint Position { get; }
    public IReadOnlyList<TerminalNode> Lookaheads { get; }
    public SymbolNode SymbolAtCurrentPosition => GetSymbolAtPosition(Position);

    public LRItem(
        NonTerminalNode nonTerminal, 
        uint position,
        IEnumerable<TerminalNode> lookaheads)
    {
        NonTerminal = nonTerminal;
        Position = position;
        Lookaheads = lookaheads.ToArray();
    }

    public override string ToString()
    {
        var sentenceStrBuilder = new List<string>();

        for (int i = 0; i < NonTerminal.Children.Length; i++)
        {
            if (i == Position)
            {
                sentenceStrBuilder.Add("•");
            }

            sentenceStrBuilder.Add(NonTerminal.Children[i].ToString());
        }

        if (Position == NonTerminal.Children.Length)
        {
            sentenceStrBuilder.Add("•");
        }

        var sentenceStr = string.Join(" ", sentenceStrBuilder);
        var @base = $"{NonTerminal.Name} -> {sentenceStr}";

        var orderedLookaheads = Lookaheads.Count < 2
            ? Lookaheads
            : Lookaheads
                .ToArray();

        var lookaheadStrs = orderedLookaheads
            .Select(x => x.ToString())
            .ToArray();

        var lookaheads = string.Join(", ", lookaheadStrs);

        return $"{@base} - {lookaheads}";

        //var production = $"{NonTerminal.Name} -> {string.Join(" ", NonTerminal.Children.Select(x => x.ToString()))}";
        //var lookaheads = $"{string.Join(", ", Lookaheads.Select(x => x.ToString()))}";

        //return $"[({production}), {{{lookaheads}}}]";
    }

    public SymbolNode? GetSymbolAtPosition(uint position)
    {
        if (position >= NonTerminal.Children.Length)
        {
            return null;    
        }

        return NonTerminal.Children[position];
    }
}

public class LRKernel
{
    public IReadOnlyList<LRItem> Items { get; }

    public LRKernel(IEnumerable<LRItem> items)
    {
        Items = items.ToArray();
    }
}

public class LRClosure
{
    public IReadOnlyList<LRItem> Items { get; }

    public LRClosure(IEnumerable<LRItem> items)
    {
        Items = items.ToArray();
    }
}

