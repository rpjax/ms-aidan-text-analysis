using Aidan.TextAnalysis.Helpers;

namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents a non-terminal symbol in a context-free grammar.
/// </summary>
public interface INonTerminal : ISymbol
{

}

/// <summary>
/// Represents a non-terminal symbol in a context-free grammar.
/// </summary>
public class NonTerminal : INonTerminal
{
    /// <summary>
    /// Gets the type of the symbol.
    /// </summary>
    public SymbolType Type { get; protected set; }

    /// <summary>
    /// Gets the name of the non-terminal symbol.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonTerminal"/> class with the specified name.
    /// </summary>
    /// <param name="name">The name of the non-terminal symbol.</param>
    /// <exception cref="ArgumentException">Thrown when the name is empty, contains spaces, or is epsilon.</exception>
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

        Type = SymbolType.NonTerminal;
        Name = name;
    }

    /// <summary>
    /// Returns a string that represents the current non-terminal symbol.
    /// </summary>
    /// <returns>A string that represents the current non-terminal symbol.</returns>
    public override string ToString()
    {
        return Name;
    }

    /// <summary>
    /// Determines whether the specified symbol is equal to the current non-terminal symbol.
    /// </summary>
    /// <param name="other">The symbol to compare with the current non-terminal symbol.</param>
    /// <returns>true if the specified symbol is equal to the current non-terminal symbol; otherwise, false.</returns>
    public bool Equals(ISymbol? other)
    {
        return other is INonTerminal nonTerminal
            && nonTerminal.Name == Name;
    }

    /// <summary>
    /// Gets a value based hash for the terminal.
    /// </summary>
    /// <returns>A signed 32 bit integer hash.</returns>
    public override int GetHashCode()
    {
        return HashHelper.ComputeHash(Type, Name);
    }

}
