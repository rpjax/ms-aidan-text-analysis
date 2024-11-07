using Aidan.TextAnalysis.Regexes.Ast.Extensions;

namespace Aidan.TextAnalysis.Regexes.Ast;

/// <summary>
/// Represents an epsilon node in a regex, which matches the empty string ε.
/// </summary>
public class EpsilonNode : RegexNode
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
            children: Array.Empty<RegexNode>(),
            metadata: metadata)
    {

    }

    /// <inheritdoc />
    public override string ToString()
    {
        return "ε";
    }

    /// <inheritdoc />
    public override bool Equals(RegexNode? other)
    {
        return other?.IsEpsilon() == true;
    }

    /// <inheritdoc />
    public override IReadOnlyList<RegexNode> GetChildren()
    {
        return new RegexNode[0];
    }

}
