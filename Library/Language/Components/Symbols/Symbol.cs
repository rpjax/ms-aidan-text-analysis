namespace Aidan.TextAnalysis.Language.Components.Symbols;

/// <summary>
/// Represents a symbol in the language.
/// </summary>
public interface ISymbol : IEquatable<ISymbol>
{
    /// <summary>
    /// Gets the type of the symbol.
    /// </summary>
    SymbolType Type { get; }

    /// <summary>
    /// Gets the name of the symbol.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Returns a string that represents the current symbol.
    /// </summary>
    /// <returns>A string that represents the current symbol.</returns>
    string ToString();

    /// <summary>
    /// Gets a value based hash for the symbol.
    /// </summary>
    /// <returns>A signed 32 bit integer hash.</returns>
    int GetHashCode();
}

/// <summary>
/// Represents an abstract base class for symbols.
/// </summary>
public abstract class Symbol : ISymbol
{
    /// <summary>
    /// Gets the type of the symbol.
    /// </summary>
    public abstract SymbolType Type { get; }

    /// <summary>
    /// Gets the name of the symbol.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Determines whether the specified symbol is equal to the current symbol.
    /// </summary>
    /// <param name="other">The symbol to compare with the current symbol.</param>
    /// <returns>true if the specified symbol is equal to the current symbol; otherwise, false.</returns>
    public abstract bool Equals(ISymbol? other);

    /// <summary>
    /// Gets a value based hash for the symbol.
    /// </summary>
    /// <returns>A signed 32 bit integer hash.</returns>
    public abstract override int GetHashCode();

    /// <summary>
    /// Returns a string that represents the current symbol.
    /// </summary>
    /// <returns>A string that represents the current symbol.</returns>
    public abstract override string ToString();
}
