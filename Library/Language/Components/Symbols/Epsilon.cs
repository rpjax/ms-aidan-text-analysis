using Aidan.TextAnalysis.Helpers;

namespace Aidan.TextAnalysis.Language.Components.Symbols;

/// <summary>
/// Represents an epsilon(ε) symbol in a context-free grammar.
/// </summary>
public class Epsilon : ISymbol
{
    /// <summary>
    /// Gets the singleton instance of the <see cref="Epsilon"/> class.
    /// </summary>
    public static Epsilon Instance { get; } = new Epsilon();

    /// <summary>
    /// Gets the type of the symbol.
    /// </summary>
    public SymbolType Type { get; protected set; }

    /// <summary>
    /// Gets the name of the symbol.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Epsilon"/> class.
    /// </summary>
    public Epsilon()
    {
        Type = SymbolType.Epsilon;
        Name = GreekLetters.Epsilon.ToString();
    }

    /// <summary>
    /// Returns a string that represents the current symbol.
    /// </summary>
    /// <returns>A string that represents the current symbol.</returns>
    public override string ToString()
    {
        return GreekLetters.Epsilon.ToString();
    }

    /// <summary>
    /// Determines whether the specified symbol is equal to the current symbol.
    /// </summary>
    /// <param name="other">The symbol to compare with the current symbol.</param>
    /// <returns><c>true</c> if the specified symbol is equal to the current symbol; otherwise, <c>false</c>.</returns>
    public bool Equals(ISymbol? other)
    {
        return other?.Type == SymbolType.Epsilon;
    }

    /// <summary>
    /// Gets a value based hash for the epsilon.
    /// </summary>
    /// <returns>A signed 32 bit integer hash.</returns>
    public override int GetHashCode()
    {
        return HashHelper.ComputeHash(Type);
    }

}
