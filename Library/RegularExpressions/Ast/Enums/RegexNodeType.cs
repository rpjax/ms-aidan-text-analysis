namespace Aidan.TextAnalysis.RegularExpressions.Ast;

/// <summary>
/// Represents the type of a regex node.
/// </summary>
public enum RegexNodeType
{
    /// <summary>
    /// Represents an epsilon node, matching an empty string.
    /// </summary>
    Epsilon,

    /// <summary>
    /// Represents an empty set node, matching nothing.
    /// </summary>
    EmptySet,

    /// <summary>
    /// Represents a literal node, matching a specific character.
    /// </summary>
    Literal,

    /// <summary>
    /// Represents a union node, matching one of two patterns.
    /// </summary>
    Union,

    /// <summary>
    /// Represents a concatenation node, matching a sequence of patterns.
    /// </summary>
    Concatenation,

    /// <summary>
    /// Represents a star node, matching zero or more repetitions of a pattern.
    /// </summary>
    Star,
}
