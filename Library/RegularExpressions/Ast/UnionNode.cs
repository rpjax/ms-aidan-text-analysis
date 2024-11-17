namespace Aidan.TextAnalysis.RegularExpressions.Ast;

/// <summary>
/// Represents a union node in a regex, matching either of two patterns.
/// </summary>
public class UnionNode : RegExpr
{
    /// <summary>
    /// Gets the left operand of the union.
    /// </summary>
    public RegExpr Left { get; }

    /// <summary>
    /// Gets the right operand of the union.
    /// </summary>
    public RegExpr Right { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnionNode"/> class.
    /// </summary>
    /// <param name="left">The left operand of the union.</param>
    /// <param name="right">The right operand of the union.</param>
    /// <param name="metadata">The metadata associated with the node.</param>
    public UnionNode(
        RegExpr left,
        RegExpr right,
        Dictionary<string, object>? metadata = null)
        : base(
            type: RegexNodeType.Union,
            containsEpsilon: left.ContainsEpsilon || right.ContainsEpsilon,
            children: new[] { left, right },
            metadata: metadata)
    {
        Left = left;
        Right = right;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Left}|{Right}";
    }

    /// <inheritdoc />
    public override bool Equals(RegExpr? other)
    {
        return other is UnionNode union
            && union.Left.Equals(Left)
            && union.Right.Equals(Right);
    }

    /// <inheritdoc />
    public override IReadOnlyList<RegExpr> GetChildren()
    {
        return new RegExpr[] { Left, Right };
    }

}
