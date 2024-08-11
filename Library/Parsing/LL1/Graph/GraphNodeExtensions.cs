using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Language.Graph;

public static class GraphNodeExtensions
{
    public static LL1GraphNode GetParent(this LL1GraphNode node)
    {
        if (node.Parent is null)
        {
            throw new InvalidOperationException("The node has no parent.");
        }

        return node.Parent;
    }

    public static IEnumerable<LL1GraphNode> GetChildren(this LL1GraphNode node)
    {
        return node.Children;
    }

    public static IEnumerable<LL1GraphNode> GetSiblings(this LL1GraphNode node)
    {
        if (node.Parent is null)
        {
            throw new InvalidOperationException("The node has no parent.");
        }

        return node.Parent.Children;
    }

    public static IEnumerable<LL1GraphNode> GetAncestors(this LL1GraphNode node)
    {
        var currentNode = node;

        while (currentNode.Parent is not null)
        {
            currentNode = currentNode.Parent;
            yield return currentNode;
        }
    }

    public static IEnumerable<LL1GraphNode> GetDescendants(this LL1GraphNode node)
    {
        foreach (var child in node.Children)
        {
            yield return child;

            if (child.IsRecursive)
            {
                continue;
            }

            foreach (var descendant in child.GetDescendants())
            {
                yield return descendant;
            }
        }
    }

    public static IEnumerable<LL1GraphNode> GetLeafs(this LL1GraphNode node)
    {
        foreach (var child in node.Children)
        {
            if(child.IsRecursive || child.ChildrenCount == 0)
            {
                yield return child;
            }

            foreach (var leaf in child.GetLeafs())
            {
                yield return leaf;
            }
        }
    }

    public static void SetParent(this LL1GraphNode self, LL1GraphNode parent)
    {
        self.Parent = parent;
    }

    public static void AddChild(this LL1GraphNode self, LL1GraphNode child)
    {
        self.Children.Add(child);
        child.Parent = self;
    }

    public static void RemoveChild(this LL1GraphNode self, LL1GraphNode child)
    {
        self.Children.Remove(child);
        child.Parent = null;
    }

    public static void RemoveFromTree(this LL1GraphNode self)
    {
        self.Parent?.RemoveChild(self);
    }

    public static void RemoveAllChildren(this LL1GraphNode self)
    {
        foreach (var child in self)
        {
            child.Parent = null;
        }

        self.Children.Clear();
    }

    public static void ReplaceChild(this LL1GraphNode self, LL1GraphNode oldNode, LL1GraphNode newNode)
    {
        var index = self.Children.IndexOf(oldNode);

        if (index == -1)
        {
            throw new InvalidOperationException("The old node is not a child of this node.");
        }

        self[index] = newNode;
        newNode.Parent = self;
        oldNode.Parent = null;
    }

    public static void ReplaceWith(this LL1GraphNode self, LL1GraphNode newNode)
    {
        self.Parent?.ReplaceChild(self, newNode);
    }

    public static ProductionSet ToProductionSet(this LL1GraphNode node)
    {
        var builder = new ProductionSetBuilder();
        var productions = node.GetDescendants()
            .Select(x => x.Production)
            .Concat(new[] { node.Production })
            .ToArray();

        foreach (var production in productions)
        {
            if (production is not null)
            {
                builder.Add(production.Value);
            }
        }

        return builder.Build();
    }

    public static GraphBranch GetRecursiveBranch(this LL1GraphNode self, Symbol root)
    {
        if (!self.IsRecursive)
        {
            throw new InvalidOperationException("The node is not recursive.");
        }
        if(self.Parent is null)
        {
            throw new InvalidOperationException("The node has no parent.");
        }

        var currentNode = self.Parent;
        var rootFound = false;
        var nodes = new List<LL1GraphNode>();

        nodes.Add(self);

        while (true)
        {
            nodes.Add(currentNode);

            if (currentNode.Symbol == root)
            {
                rootFound = true;
                break;
            }

            if (currentNode.Parent is null)
            {
                break;
            }

            currentNode = currentNode.Parent;
        }

        if (!rootFound)
        {
            throw new InvalidOperationException("The stop symbol was not found in the branch.");
        }

        nodes.Reverse();

        return new GraphBranch(nodes);
    }

}
