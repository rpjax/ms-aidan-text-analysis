using Aidan.TextAnalysis.RegularExpressions.Tree;

namespace Aidan.TextAnalysis.RegularExpressions.Derivative;

/// <summary>
/// Represents a single derivative calculation for a regular expression with respect to a character.
/// </summary>
public class Derivation
{
    /// <summary>
    /// Gets the regular expression node.
    /// </summary>
    public RegExpr Regex { get; }

    /// <summary>
    /// Gets the character with respect to which the derivative is calculated.
    /// </summary>
    public char Character { get; }

    /// <summary>
    /// Gets the derivative of the regular expression node.
    /// </summary>
    public RegExpr Derivative { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Derivation"/> class.
    /// </summary>
    /// <param name="regex">The regular expression node.</param>
    /// <param name="character">The character with respect to which the derivative is calculated.</param>
    /// <param name="derivative">The derivative of the regular expression node.</param>
    public Derivation(
        RegExpr regex,
        char character,
        RegExpr derivative)
    {
        Regex = regex;
        Character = character;
        Derivative = derivative;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"`{Regex}` with respect to `{Character}` = `{Derivative}`";
    }
}
