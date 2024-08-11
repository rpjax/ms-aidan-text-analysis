using Aidan.TextAnalysis.Language.Extensions;

namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents a non-terminal symbol in a context-free grammar.
/// </summary>
public interface INonTerminal : ISymbol
{
    string Name { get; }
}

/// <summary>
/// Represents a non-terminal symbol in a context-free grammar.
/// </summary>
public class NonTerminal : Symbol, INonTerminal
{
    /// <inheritdoc/>
    public override bool IsTerminal => false;

    /// <inheritdoc/>
    public override bool IsNonTerminal => true;

    /// <inheritdoc/>
    public override bool IsEpsilon => false;

    /// <inheritdoc/>
    public override bool IsMacro => false;

    /// <inheritdoc/>
    public override bool IsEoi => false;

    /// <summary>
    /// Gets the name of the non-terminal symbol.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonTerminal"/> class with the specified name.
    /// </summary>
    /// <param name="name">The name of the non-terminal symbol.</param>
    public NonTerminal(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("The name of a non-terminal symbol cannot be empty.");
        }
        if (name.Contains(' '))
        {
            throw new ArgumentException("The name of a non-terminal symbol cannot contain spaces.");
        }
        if (name == GreekLetters.Epsilon.ToString())
        {
            throw new ArgumentException("The name of a non-terminal symbol cannot be epsilon.");
        }

        Name = name;
    }

    public static implicit operator NonTerminal(string name)
    {
        return new NonTerminal(name);
    }

    public static bool operator ==(NonTerminal left, NonTerminal right)
    {
        return left.Name == right.Name;
    }

    public static bool operator !=(NonTerminal left, NonTerminal right)
    {
        return left.Name != right.Name;
    }

    /*
     * instance methods.
     */

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ Name.GetHashCode();
            return hash;
        }
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Symbol);
    }

    public override bool Equals(Symbol? other)
    {
        return other is NonTerminal nonTerminal
            && nonTerminal.Name == Name;
    }

    public override bool Equals(ISymbol? other)
    {
        return other is INonTerminal nonTerminal
            && nonTerminal.Name == Name;
    }

    public override string ToNotation(NotationType notation)
    {
        switch (notation)
        {
            case NotationType.Sentential:
                return this.ToSententialNotation();

            case NotationType.Bnf:
                return this.ToBnfNotation();

            case NotationType.Ebnf:
                return this.ToEbnfNotation();

            case NotationType.EbnfKleene:
                return this.ToEbnfKleeneNotation();
        }

        throw new InvalidOperationException("Invalid notation type.");
    }

    /// <summary>
    /// Returns a string representation of the non-terminal symbol.
    /// </summary>
    /// <returns>A string representation of the non-terminal symbol.</returns>
    public override string ToString()
    {
        return ToNotation(NotationType.Sentential);
    }

}
