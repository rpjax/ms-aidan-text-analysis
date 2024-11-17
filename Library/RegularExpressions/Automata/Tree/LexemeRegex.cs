using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;
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
    /// Gets the alphabet of the lexeme regex.
    /// </summary>
    public IReadOnlyList<char> Alphabet { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LexemeRegex"/> class.
    /// </summary>
    /// <param name="name">The name of the lexeme regex.</param>
    /// <param name="regex">The regex node of the lexeme regex.</param>
    /// <param name="alphabet">The alphabet of the lexeme regex.</param>
    public LexemeRegex(
        string name, 
        RegExpr regex, 
        IReadOnlyList<char>? alphabet = null)
    {
        var regexAlphabet = regex.ComputeAlphabet();

        Name = name;
        Regex = regex;
        Alphabet = alphabet ?? regexAlphabet;

        /* check if the alphabet contains all characters from the regex */
        foreach (var c in regexAlphabet)
        {
            if (!Alphabet.Contains(c))
            {
                throw new InvalidOperationException("The alphabet does not contain all characters from the regex.");
            }
        }
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

    public IReadOnlyList<char> GetAlphabet()
    {
        return Alphabet;
    }

    /// <summary>
    /// Computes the derivative of the lexeme regex with respect to the specified character.
    /// </summary>
    /// <param name="c">The character to derive with respect to.</param>
    /// <returns>A new <see cref="LexemeRegex"/> representing the derivative of the lexeme regex.</returns>
    public LexemeRegex Derive(char c)
    {
        var calculator = new RegexDerivativeCalculator();

        var derivative = calculator
            .Derive(Regex, c);

        return new LexemeRegex(
            name: Name,
            regex: derivative,
            alphabet: Alphabet);
    }

}
