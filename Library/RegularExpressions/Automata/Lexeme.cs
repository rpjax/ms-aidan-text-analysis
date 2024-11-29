using Aidan.TextAnalysis.RegularExpressions.Tree;

namespace Aidan.TextAnalysis.RegularExpressions.Automata;

/// <summary>
/// Represents a lexeme with a name and a pattern.
/// </summary>
public class Lexeme : IEquatable<Lexeme>
{
    /// <summary>
    /// Gets the name of the lexeme.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the pattern of the lexeme.
    /// </summary>
    public RegExpr Pattern { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Lexeme"/> class.
    /// </summary>
    /// <param name="name">The name of the lexeme.</param>
    /// <param name="pattern">The pattern of the lexeme.</param>
    public Lexeme(
        string name,
        RegExpr pattern)
    {
        Name = name;
        Pattern = pattern;
    }

    /// <summary>
    /// Returns a string that represents the current lexeme.
    /// </summary>
    /// <returns>A string that represents the current lexeme.</returns>
    public override string ToString()
    {
        return $"{Name} = /{Pattern}/";
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(Lexeme? other)
    {
        if (other is null)
        {
            return false;
        }

        return Name == other.Name
            && Pattern.Equals(other.Pattern);
    }
}
