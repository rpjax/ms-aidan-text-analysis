using Aidan.TextAnalysis.RegularExpressions.Ast.Enums;
using Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;

namespace Aidan.TextAnalysis.RegularExpressions.Ast;

/// <summary>
/// Represents an epsilon node in a regex, which matches the empty string ε.
/// </summary>
public class EpsilonNode : RegExpr
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EpsilonNode"/> class.
    /// </summary>
    /// <param name="metadata">The metadata associated with the node.</param>
    public EpsilonNode(
        Dictionary<string, object>? metadata = null)
        : base(
            type: RegexNodeType.Epsilon,
            containsEpsilon: true,
            children: Array.Empty<RegExpr>(),
            metadata: metadata)
    {

    }

    /// <inheritdoc />
    public override string ToString()
    {
        return "ε";
    }

    /// <inheritdoc />
    public override bool Equals(RegExpr? other)
    {
        return other?.IsEpsilon() == true;
    }

    /// <inheritdoc />
    public override IReadOnlyList<RegExpr> GetChildren()
    {
        return new RegExpr[0];
    }

}
