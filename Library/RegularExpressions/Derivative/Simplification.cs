using Aidan.TextAnalysis.RegularExpressions.Ast;

namespace Aidan.TextAnalysis.RegularExpressions.Derivative;

/// <summary>
/// Represents a simplification of a regular expression node.
/// </summary>
public class Simplification
{
    /// <summary>
    /// Gets the original regular expression node.
    /// </summary>
    public RegexNode Regex { get; }

    /// <summary>
    /// Gets the simplified regular expression node.
    /// </summary>
    public RegexNode SimplifiedRegex { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Simplification"/> class.
    /// </summary>
    /// <param name="regex">The original regular expression node.</param>
    /// <param name="simplifiedRegex">The simplified regular expression node.</param>
    public Simplification(
        RegexNode regex,
        RegexNode simplifiedRegex)
    {
        Regex = regex;
        SimplifiedRegex = simplifiedRegex;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"`{Regex}` simplified to `{SimplifiedRegex}`";
    }
}
