using Aidan.TextAnalysis.RegularExpressions.Ast.Enums;
using Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;

namespace Aidan.TextAnalysis.RegularExpressions.Ast;

/// <summary>
/// Represents an empty set node in a regex, which matches nothing (∅).
/// </summary>
public class EmptySetNode : RegExpr
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
            children: Array.Empty<RegExpr>(),
            metadata: metadata)
    {

    }

    /// <inheritdoc />
    public override string ToString()
    {
        return "∅";
    }

    /// <inheritdoc />
    public override bool Equals(RegExpr? other)
    {
        return other?.IsEmptySet() == true;
    }

    /// <inheritdoc />
    public override IReadOnlyList<RegExpr> GetChildren()
    {
        return Array.Empty<RegExpr>();
    }

}
