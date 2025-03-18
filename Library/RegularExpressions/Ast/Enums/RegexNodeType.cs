namespace Aidan.TextAnalysis.RegularExpressions.Ast.Enums;

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

    /* new features */

    /// <summary>
    /// Represents an "anything" node, matching any character from a defined charset.
    /// <br/> This is typically equivalent to the `.` operator in regular expressions.
    /// </summary>
    Anything,

    /// <summary>
    /// Represents a character class node, matching a specific set or range of characters.
    /// <br/> This includes both positive classes (e.g., `[a-z]`) and negated classes (e.g., `[^a-z]`).
    /// </summary>
    Class
}
