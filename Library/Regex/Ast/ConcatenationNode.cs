using Aidan.TextAnalysis.Regexes.Ast.Extensions;

namespace Aidan.TextAnalysis.Regexes.Ast;

/// <summary>
/// Represents a concatenation node in a regex, matching a sequence of patterns.
/// </summary>
public class ConcatenationNode : RegexNode
{
    /// <summary>
    /// Gets the left operand in the concatenation.
    /// </summary>
    public RegexNode Left { get; }

    /// <summary>
    /// Gets the right operand in the concatenation.
    /// </summary>
    public RegexNode Right { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcatenationNode"/> class.
    /// </summary>
    /// <param name="left">The left operand in the concatenation.</param>
    /// <param name="right">The right operand in the concatenation.</param>
    /// <param name="metadata">The metadata associated with the node.</param>
    public ConcatenationNode(
        RegexNode left,
        RegexNode right,
        Dictionary<string, object>? metadata = null)
        : base(
            type: RegexNodeType.Concatenation,
            containsEpsilon: left.ContainsEpsilon && right.ContainsEpsilon,
            children: new[] { left, right },
            metadata: metadata)
    {
        Left = left;
        Right = right;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Left}{Right}";
    }

    /// <inheritdoc />
    public override bool Equals(RegexNode? other)
    {
        return other is ConcatenationNode concatenation
            && concatenation.Left.Equals(Left)
            && concatenation.Right.Equals(Right);
    }

    /// <inheritdoc />
    public override IReadOnlyList<RegexNode> GetChildren()
    {
        return new RegexNode[] { Left, Right };
    }

}
