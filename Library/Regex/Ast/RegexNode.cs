using Aidan.TextAnalysis.Regexes.Ast.Extensions;

namespace Aidan.TextAnalysis.Regexes.Ast;

/// <summary>
/// Represents an abstract base class for regex nodes in the abstract syntax tree of a regular expression.
/// </summary>
public abstract class RegexNode : IEquatable<RegexNode>
{
    /// <summary>
    /// Gets the type of the regex node.
    /// </summary>
    public RegexNodeType Type { get; }

    /// <summary>
    /// Gets a value indicating whether the node can match the empty string.
    /// </summary>
    public bool ContainsEpsilon { get; }

    /// <summary>
    /// Gets the metadata associated with the node.
    /// </summary>
    public Dictionary<string, object> Metadata { get; internal set; }

    /// <summary>
    /// Gets the parent of the regex node.
    /// </summary>
    public RegexNode? Parent { get; internal set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexNode"/> class.
    /// </summary>
    /// <param name="metadata">The metadata associated with the node.</param>
    /// <param name="children">The children of the regex node.</param>
    /// <param name="type"> 
    protected RegexNode(
        RegexNodeType type,
        bool containsEpsilon,
        RegexNode[] children,
        Dictionary<string, object>? metadata)
    {
        Type = type;
        ContainsEpsilon = containsEpsilon;
        Parent = null;
        Metadata = metadata ?? new Dictionary<string, object>();

        foreach (var child in children)
        {
            child.SetParent(this);
        }
    }

    /// <summary>
    /// Determines whether the specified <see cref="RegexNode"/> is equal to the current <see cref="RegexNode"/>.
    /// </summary>
    /// <param name="other">The <see cref="RegexNode"/> to compare with the current <see cref="RegexNode"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="RegexNode"/> is equal to the current <see cref="RegexNode"/>; otherwise, <c>false</c>.</returns>
    public abstract bool Equals(RegexNode? other);

    /// <summary>
    /// Returns a string representation of the regex node.
    /// </summary>
    /// <returns>A string representation of the regex node.</returns>
    public abstract override string ToString();

    /// <summary>
    /// Gets the children of the regex node.
    /// </summary>
    /// <returns>A read-only list of child regex nodes.</returns>
    public abstract IReadOnlyList<RegexNode> GetChildren();
}
