using Aidan.TextAnalysis.Tokenization;

namespace Aidan.TextAnalysis.Parsing.Components;

/// <summary>
/// Represents the type of a node in the concrete syntax tree (CST).
/// </summary>
public enum CstNodeType
{
    Root,
    Internal,
    Leaf,
}

/// <summary>
/// Represents a node in the concrete syntax tree (CST).
/// </summary>
public abstract class CstNode
{
    /// <summary>
    /// Gets the type of the node.
    /// </summary>
    public abstract CstNodeType Type { get; }

    /// <summary>
    /// Gets the name associated with the node.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the metadata associated with the node.
    /// </summary>
    public abstract CstNodeMetadata Metadata { get; }

    /// <summary>
    /// Gets the properties of the node. It can be used to extend the node with additional information.
    /// </summary>
    public Dictionary<string, object> Properties { get; }

    /// <summary>
    /// Gets the parent node of the current node.
    /// </summary>
    public CstNode? Parent { get; internal set; }

    /// <summary>
    /// Gets the children of the node.
    /// </summary>
    protected CstNode[] InternalChildren { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CstNode"/> class.
    /// </summary>
    /// <param name="children">The children nodes of the current node.</param>
    public CstNode(CstNode[]? children)
    {
        Properties = new();
        InternalChildren = children ?? Array.Empty<CstNode>();

        foreach (var child in InternalChildren)
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
    /// Accepts a visitor.
    /// </summary>
    /// <param name="visitor">The visitor to accept.</param>
    /// <returns>The result of the visitor's visit operation.</returns>
    public abstract CstNode Accept(CstNodeVisitor visitor);

    /// <summary>
    /// Gets the children of the node.
    /// </summary>
    /// <returns>An enumerable collection of child nodes.</returns>
    public IEnumerable<CstNode> GetChildren()
    {
        return InternalChildren;
    }

    /// <summary>
    /// Gets the properties of the node.
    /// </summary>
    /// <returns>An enumerable collection of key-value pairs representing the properties of the node.</returns>
    public IEnumerable<KeyValuePair<string, object>> GetProperties()
    {
        return Properties;
    }

    /// <summary>
    /// Gets a value indicating whether the node has a property by key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns> A value indicating whether the node has a property by key.</returns>
    public bool HasProperty(string key)
    {
        return Properties.ContainsKey(key);
    }

    /// <summary>
    /// Gets the value of a property by key.
    /// </summary>
    /// <param name="key">The key of the property.</param>
    /// <returns>The value of the property.</returns>
    public object GetProperty(string key)
    {
        return Properties[key];
    }

    /// <summary>
    /// Tries to get the value of a property by key.
    /// </summary>
    /// <param name="key">The key of the property.</param>
    /// <returns>The value of the property, or null if the property does not exist.</returns>
    public object? TryGetProperty(string key)
    {
        return Properties.TryGetValue(key, out var value) ? value : null;
    }

    /// <summary>
    /// Tries to get the value of a property by key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public T? TryGetProperty<T>(string key)
    {
        var value = TryGetProperty(key);

        if(value is T result)
        {
            return result;
        }

        return default;
    }

    /// <summary>
    /// Sets the value of a property by key.
    /// </summary>
    /// <param name="key">The key of the property.</param>
    /// <param name="value">The value to set.</param>
    public void SetProperty(string key, object value)
    {
        Properties[key] = value;
    }
}

/// <summary>
/// Represents a root node in the concrete syntax tree (CST).
/// </summary>
public class CstRootNode : CstNode
{
    /// <inheritdoc/>
    public override CstNodeType Type => CstNodeType.Root;

    /// <inheritdoc/>
    public override string Name { get; }

    /// <inheritdoc/>
    public override CstNodeMetadata Metadata { get; }

    /// <summary>
    /// Gets the children of the root node.
    /// </summary>
    public CstNode[] Children { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="CstRootNode"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="children"></param>
    /// <param name="metadata"></param>
    public CstRootNode(
        string name, 
        CstNode[] children, 
        CstNodeMetadata metadata) : base(children)
    {
        Name = name;
        Children = children;
        Metadata = metadata;
    }

    /// <inheritdoc/>
    public override CstNode Accept(CstNodeVisitor visitor)
    {
        return visitor.VisitRoot(this);
    }

    ///<inheritdoc/>
    public override string ToString()
    {
        return $"{Name}: [`{string.Join(" ", Children.Select(x => x.ToString()))}`]";
    }
}

/// <summary>
/// Represents an internal node in the concrete syntax tree (CST).
/// </summary>
public class CstInternalNode : CstNode
{
    /// <inheritdoc/>
    public override CstNodeType Type => CstNodeType.Internal;

    /// <inheritdoc/>
    public override string Name { get; }

    /// <inheritdoc/>
    public override CstNodeMetadata Metadata { get; }

    /// <summary>
    /// Gets the children of the internal node.
    /// </summary>
    public CstNode[] Children { get; }

    /// <summary>
    /// Gets a value indicating whether the internal node is an epsilon node.
    /// </summary>
    public bool IsEpsilon { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="CstInternalNode"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="children"></param>
    /// <param name="metadata"></param>
    /// <param name="isEpsilon"></param>
    public CstInternalNode(
        string name, 
        CstNode[] children, 
        CstNodeMetadata metadata, 
        bool isEpsilon = false) : base(children)
    {
        Name = name;
        Children = children;
        Metadata = metadata;
        IsEpsilon = isEpsilon;
    }

    /// <inheritdoc/>
    public override CstNode Accept(CstNodeVisitor visitor)
    {
        return visitor.VisitNonTerminal(this);
    }

    ///<inheritdoc/>
    public override string ToString()
    {
        return $"{Name}: [`{string.Join(" ", Children.Select(x => x.ToString()))}`]";
    }
}

/// <summary>
/// Represents a leaf node in the concrete syntax tree (CST).
/// </summary>
public class CstLeafNode : CstNode
{
    /// <inheritdoc/>
    public override CstNodeType Type => CstNodeType.Leaf;

    /// <inheritdoc/>
    public override string Name { get => Token.Type.ToString(); }

    /// <inheritdoc/>
    public override CstNodeMetadata Metadata { get; }

    /// <summary>
    /// Gets the token associated with the leaf node.
    /// </summary>
    public Token Token { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="CstLeafNode"/> class.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="metadata"></param>
    public CstLeafNode(Token token, CstNodeMetadata metadata) : base(null)
    {
        Token = token;
        Metadata = metadata;
    }

    ///<inheritdoc/>
    public override CstNode Accept(CstNodeVisitor visitor)
    {
        return visitor.VisitTerminal(this);
    }

    ///<inheritdoc/>
    public override string ToString()
    {
        return Token.Value.ToString();
    }

}
