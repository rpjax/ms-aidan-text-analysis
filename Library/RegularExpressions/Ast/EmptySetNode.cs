using Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;

namespace Aidan.TextAnalysis.RegularExpressions.Ast;

/// <summary>
/// Represents an empty set node in a regex, which matches nothing (∅).
/// </summary>
public class EmptySetNode : RegexNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmptySetNode"/> class.
    /// </summary>
    /// <param name="metadata">The metadata associated with the node.</param>
    public EmptySetNode(
        Dictionary<string, object>? metadata = null)
        : base(
            type: RegexNodeType.EmptySet,
            containsEpsilon: false,
            children: Array.Empty<RegexNode>(),
            metadata: metadata)
    {

    }

    /// <inheritdoc />
    public override string ToString()
    {
        return "∅";
    }

    /// <inheritdoc />
    public override bool Equals(RegexNode? other)
    {
        return other?.IsEmptySet() == true;
    }

    /// <inheritdoc />
    public override IReadOnlyList<RegexNode> GetChildren()
    {
        return Array.Empty<RegexNode>();
    }

}
