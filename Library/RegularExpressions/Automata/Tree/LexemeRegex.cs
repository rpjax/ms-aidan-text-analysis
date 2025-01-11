using Aidan.TextAnalysis.RegularExpressions.Tree;
using Aidan.TextAnalysis.RegularExpressions.Tree.Extensions;
using Aidan.TextAnalysis.RegularExpressions.Derivative;

namespace Aidan.TextAnalysis.RegularExpressions.Automata;

/// <summary>
/// Represents a lexeme regular expression with a name and a regex node.
/// </summary>
public class LexemeRegex : IEquatable<LexemeRegex>
{
    /// <summary>
    /// Gets the name of the lexeme regex.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the regex node of the lexeme regex.
    /// </summary>
    public RegExpr Regex { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LexemeRegex"/> class.
    /// </summary>
    /// <param name="name">The name of the lexeme regex.</param>
    /// <param name="regex">The regex node of the lexeme regex.</param>
    public LexemeRegex(
        string name, 
        RegExpr regex)
    {
        Name = name;
        Regex = regex;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"{Name} = /{Regex}/";
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;

            hash = hash * 23 + Name.GetHashCode();
            hash = hash * 23 + Regex.GetHashCode();

            return hash;
        }
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns><c>true</c> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.</returns>
    public bool Equals(LexemeRegex? other)
    {
        if (other is null)
        {
            return false;
        }

        return Name == other.Name
            && Regex.Equals(other.Regex);
    }

    public IReadOnlyList<char> ComputeAlphabet()
    {
        return Regex.ComputeAlphabet();
    }

    /// <summary>
    /// Computes the derivative of the lexeme regex with respect to the specified character.
    /// </summary>
    /// <param name="c">The character to derive with respect to.</param>
    /// <returns>A new <see cref="LexemeRegex"/> representing the derivative of the lexeme regex.</returns>
    public LexemeRegex ComputeDerivative(char c)
    {
        var calculator = new RegexDerivativeCalculator();

        var derivative = calculator
            .Derive(Regex, c);

        return new LexemeRegex(
            name: Name,
            regex: derivative);
    }

}
