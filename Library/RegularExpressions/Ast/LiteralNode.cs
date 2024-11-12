﻿using System.Globalization;

namespace Aidan.TextAnalysis.RegularExpressions.Ast;

/// <summary>
/// Represents a literal node in a regex, matching a specific character.
/// </summary>
public class LiteralNode : RegexNode
{
    /// <summary>
    /// Gets the literal character to match.
    /// </summary>
    public char Character { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LiteralNode"/> class.
    /// </summary>
    /// <param name="literal">The literal character to match.</param>
    /// <param name="metadata">The metadata associated with the node.</param>
    public LiteralNode(
        char literal,
        Dictionary<string, object>? metadata = null)
        : base(
            type: RegexNodeType.Literal,
            containsEpsilon: false,
            children: Array.Empty<RegexNode>(),
            metadata: metadata)
    {
        Character = literal;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Character.ToString(CultureInfo.InvariantCulture);
    }

    /// <inheritdoc />
    public override bool Equals(RegexNode? other)
    {
        return other is LiteralNode literal
            && literal.Character == Character;
    }

    /// <inheritdoc />
    public override IReadOnlyList<RegexNode> GetChildren()
    {
        return Array.Empty<RegexNode>();
    }

}
