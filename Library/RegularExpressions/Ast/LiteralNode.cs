using System.Globalization;

namespace Aidan.TextAnalysis.RegularExpressions.Tree;

/// <summary>
/// Represents a literal node in a regex, matching a specific character.
/// </summary>
public class LiteralNode : RegExpr
{
    /// <summary>
    /// Gets the literal character to match.
    /// </summary>
    public char Character { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LiteralNode"/> class.
    /// </summary>
    /// <param name="character">The literal character to match.</param>
    /// <param name="metadata">The metadata associated with the node.</param>
    public LiteralNode(
        char character,
        Dictionary<string, object>? metadata = null)
        : base(
            type: RegexNodeType.Literal,
            containsEpsilon: false,
            children: Array.Empty<RegExpr>(),
            metadata: metadata)
    {
        Character = character;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Character.ToString(CultureInfo.InvariantCulture);
    }

    /// <inheritdoc />
    public override bool Equals(RegExpr? other)
    {
        return other is LiteralNode literal
            && literal.Character == Character;
    }

    /// <inheritdoc />
    public override IReadOnlyList<RegExpr> GetChildren()
    {
        return Array.Empty<RegExpr>();
    }

}
