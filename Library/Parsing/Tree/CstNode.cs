using Aidan.Core.Patterns;
using Aidan.TextAnalysis.Tokenization;

namespace Aidan.TextAnalysis.Parsing.Tree;

/// <summary>
/// Represents the type of a node in the concrete syntax tree (CST).
/// </summary>
public enum CstNodeType
{
    /// <summary>
    /// The root node of the CST.
    /// </summary>
    Root,

    /// <summary>
    /// An internal node in the CST.
    /// </summary>
    Internal,

    /// <summary>
    /// A leaf node in the CST.
    /// </summary>
    Leaf,
}

/// <summary>
/// Represents a node in the concrete syntax tree (CST).
/// </summary>
public abstract class CstNode : ITree<CstNode>
{
    /// <summary>
    /// Gets the type of the node.
    /// </summary>
    public CstNodeType Type { get; }

    /// <summary>
    /// Gets the name associated with the node.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the metadata associated with the node.
    /// </summary>
    public CstNodeMetadata Metadata { get; }

    /// <summary>
    /// Gets the properties of the node. It can be used to extend the node with additional information.
    /// </summary>
    public Dictionary<string, object> Properties { get; }

    /// <summary>
    /// Gets the parent node of the current node.
    /// </summary>
    public CstNode? Parent { get; internal set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CstNode"/> class.
    /// </summary>
    /// <param name="type">The type of the node.</param>
    /// <param name="name">The name of the node.</param>
    /// <param name="metadata">The metadata associated with the node.</param>
    /// <param name="properties">Optional properties to associate with the node.</param>
    /// <param name="children">Optional child nodes to add to this node.</param>
    public CstNode(
        CstNodeType type,
        string name,
        CstNodeMetadata metadata,
        Dictionary<string, object>? properties,
        CstNode[]? children)
    {
        Type = type;
        Name = name;
        Metadata = metadata;
        Properties = properties ?? new Dictionary<string, object>();

        foreach (var child in children ?? Array.Empty<CstNode>())
        {
            child.Parent = this;
        }
    }

    /// <summary>
    /// Gets or sets a property by key.
    /// </summary>
    /// <param name="key">The key of the property.</param>
    /// <returns>The value of the property.</returns>
    public object this[string key]
    {
        get => Properties[key];
        set => Properties[key] = value;
    }

    /// <summary>
    /// Gets the children of the node.
    /// </summary>
    /// <returns>An enumerable collection of child nodes.</returns>
    public abstract IReadOnlyList<CstNode> GetChildren();

    /// <summary>
    /// Accepts a visitor.
    /// </summary>
    /// <param name="visitor">The visitor to accept.</param>
    /// <returns>The result of the visitor's visit operation.</returns>
    public abstract CstNode Accept(CstVisitor visitor);
}

/// <summary>
/// Represents a root node in the concrete syntax tree (CST).
/// </summary>
public class CstRootNode : CstNode
{
    /// <summary>
    /// Gets the children of the root node.
    /// </summary>
    public CstNode[] Children { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CstRootNode"/> class.
    /// </summary>
    /// <param name="name">The name of the root node.</param>
    /// <param name="children">The child nodes of the root node.</param>
    /// <param name="metadata">The metadata associated with the root node.</param>
    public CstRootNode(
        string name,
        CstNode[] children,
        CstNodeMetadata metadata)
        : base(
            type: CstNodeType.Root,
            name: name,
            metadata: metadata,
            properties: null,
            children: children)
    {
        Children = children;
    }

    /// <inheritdoc/>
    public override IReadOnlyList<CstNode> GetChildren()
    {
        return Children;
    }

    /// <inheritdoc/>
    public override CstNode Accept(CstVisitor visitor)
    {
        return visitor.VisitRoot(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Name}";
    }
}

/// <summary>
/// Represents an internal node in the concrete syntax tree (CST).
/// </summary>
public class CstInternalNode : CstNode
{
    /// <summary>
    /// Gets the children of the internal node.
    /// </summary>
    public CstNode[] Children { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CstInternalNode"/> class.
    /// </summary>
    /// <param name="name">The name of the internal node.</param>
    /// <param name="children">The child nodes of the internal node.</param>
    /// <param name="metadata">The metadata associated with the internal node.</param>
    public CstInternalNode(
        string name,
        CstNode[] children,
        CstNodeMetadata metadata)
        : base(
            type: CstNodeType.Internal,
            name: name,
            metadata: metadata,
            properties: null,
            children: children)
    {
        Children = children;
    }

    /// <inheritdoc/>
    public override IReadOnlyList<CstNode> GetChildren()
    {
        return Children;
    }

    /// <inheritdoc/>
    public override CstNode Accept(CstVisitor visitor)
    {
        return visitor.VisitInternal(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Name}";
    }
}

/// <summary>
/// Represents a leaf node in the concrete syntax tree (CST).
/// </summary>
public class CstLeafNode : CstNode
{
    /// <summary>
    /// Gets the token associated with the leaf node.
    /// </summary>
    public IToken Token { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CstLeafNode"/> class.
    /// </summary>
    /// <param name="name">The name of the leaf node.</param>
    /// <param name="metadata">The metadata associated with the leaf node.</param>
    /// <param name="token">The token associated with the leaf node.</param>
    public CstLeafNode(
        string name,
        CstNodeMetadata metadata,
        IToken token)
        : base(
            type: CstNodeType.Leaf,
            name: name,
            metadata: metadata,
            properties: null,
            children: null)
    {
        Token = token;
    }

    /// <inheritdoc/>
    public override IReadOnlyList<CstNode> GetChildren()
    {
        return Array.Empty<CstNode>();
    }

    /// <inheritdoc/>
    public override CstNode Accept(CstVisitor visitor)
    {
        return visitor.VisitLeaf(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return Token.Type;
    }

    /// <summary>
    /// Gets the value of the token associated with the leaf node.
    /// </summary>
    /// <returns>The token's value as a read-only memory of characters.</returns>
    public ReadOnlyMemory<char> GetValue()
    {
        return Token.Value;
    }
}
