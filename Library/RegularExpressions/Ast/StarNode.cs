namespace Aidan.TextAnalysis.RegularExpressions.Ast;

/// <summary>
/// Represents a star (Kleene star) node in a regex, matching zero or more repetitions of a pattern.
/// </summary>
public class StarNode : RegExpr
{
    /// <summary>
    /// Gets the child node of the star operation.
    /// </summary>
    public RegExpr Child { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StarNode"/> class.
    /// </summary>
    /// <param name="child">The child pattern to repeat zero or more times.</param>
    /// <param name="metadata">The metadata associated with the node.</param>
    public StarNode(
        RegExpr child,
        Dictionary<string, object>? metadata = null)
        : base(
            type: RegexNodeType.Star,
            containsEpsilon: true,
            children: new[] { child },
            metadata: metadata)
    {
        Child = child;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"({Child})*";
    }

    /// <inheritdoc />
    public override bool Equals(RegExpr? other)
    {
        return other is StarNode star
            && star.Child.Equals(Child);
    }

    /// <inheritdoc />
    public override IReadOnlyList<RegExpr> GetChildren()
    {
        return new List<RegExpr> { Child };
    }

}
