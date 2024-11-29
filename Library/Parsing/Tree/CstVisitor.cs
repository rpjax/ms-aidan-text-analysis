namespace Aidan.TextAnalysis.Parsing.Tree;

/// <summary>
/// Provides functionality to visit nodes in a concrete syntax tree (CST).
/// </summary>
public class CstVisitor
{
    /// <summary>
    /// Visits a generic <see cref="CstNode"/> and delegates the visit to the appropriate node type.
    /// </summary>
    /// <param name="node">The CST node to visit.</param>
    /// <returns>The visited node.</returns>
    public virtual CstNode Visit(CstNode node)
    {
        return node.Accept(this);
    }

    /// <summary>
    /// Visits a <see cref="CstRootNode"/> and recursively visits its children.
    /// </summary>
    /// <param name="node">The root node to visit.</param>
    /// <returns>The visited root node.</returns>
    public virtual CstNode VisitRoot(CstRootNode node)
    {
        foreach (var child in node.GetChildren())
        {
            Visit(child);
        }

        return node;
    }

    /// <summary>
    /// Visits a <see cref="CstInternalNode"/> and recursively visits its children.
    /// </summary>
    /// <param name="node">The internal node to visit.</param>
    /// <returns>The visited internal node.</returns>
    public virtual CstNode VisitInternal(CstInternalNode node)
    {
        foreach (var child in node.GetChildren())
        {
            Visit(child);
        }

        return node;
    }

    /// <summary>
    /// Visits a <see cref="CstLeafNode"/>. No further traversal is performed.
    /// </summary>
    /// <param name="node">The leaf node to visit.</param>
    /// <returns>The visited leaf node.</returns>
    public virtual CstNode VisitLeaf(CstLeafNode node)
    {
        return node;
    }
}

/// <summary>
/// Provides functionality to rewrite nodes in a concrete syntax tree (CST).
/// </summary>
public class CstRewriter
{
    // Documentation for commented-out methods is included in case they are restored in the future.

    /*
    /// <summary>
    /// Rewrites a <see cref="CstRootNode"/> by creating a new root node
    /// with rewritten children.
    /// </summary>
    /// <param name="node">The root node to rewrite.</param>
    /// <returns>The rewritten root node.</returns>
    public virtual CstNode VisitRoot(CstRootNode node)
    {
        return new CstRootNode(
            name: node.Name,
            children: node.Children.Select(x => Visit(x)).ToArray(),
            metadata: node.Metadata);
    }

    /// <summary>
    /// Rewrites a <see cref="CstInternalNode"/> by creating a new internal node
    /// with rewritten children.
    /// </summary>
    /// <param name="node">The internal node to rewrite.</param>
    /// <returns>The rewritten internal node.</returns>
    public virtual CstNode VisitNonTerminal(CstInternalNode node)
    {
        return new CstInternalNode(
            name: node.Name,
            children: node.Children.Select(x => Visit(x)).ToArray(),
            metadata: node.Metadata);
    }

    /// <summary>
    /// Rewrites a <see cref="CstLeafNode"/> by creating a new leaf node
    /// with the same token and metadata.
    /// </summary>
    /// <param name="node">The leaf node to rewrite.</param>
    /// <returns>The rewritten leaf node.</returns>
    public virtual CstNode VisitTerminal(CstLeafNode node)
    {
        return new CstLeafNode(
            token: node.Token,
            metadata: node.Metadata);
    }
    */
}
