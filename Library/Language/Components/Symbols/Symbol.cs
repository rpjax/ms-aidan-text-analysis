using System.Diagnostics.CodeAnalysis;

namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents a production symbol in a context-free grammar.
/// </summary>
public interface ISymbol : IEquatable<ISymbol>
{
    /// <summary>
    /// Gets a value indicating whether the production symbol is a terminal symbol.
    /// </summary>
    bool IsTerminal { get; }

    /// <summary>
    /// Gets a value indicating whether the production symbol is a non-terminal symbol.
    /// </summary>
    bool IsNonTerminal { get; }

    /// <summary>
    /// Gets a value indicating whether the production symbol is an epsilon symbol.
    /// </summary>
    bool IsEpsilon { get; }

    /// <summary>
    /// Gets a value indicating whether this production symbol is a macro.
    /// </summary>
    bool IsMacro { get; }

    /// <summary>
    /// Gets a value indicating whether the production symbol is an end-of-input symbol.
    /// </summary>
    bool IsEoi { get; }

    /// <summary>
    /// Returns a string representation of the production symbol in the specified notation.
    /// </summary>
    /// <param name="notation"></param>
    /// <returns></returns>
    string ToNotation(NotationType notation);
}

/// <summary>
/// Abstract base class for production symbols in a context-free grammar.
/// </summary>
public abstract class Symbol : ISymbol, IEquatable<Symbol>
{
    /// <summary>
    /// Gets a value indicating whether the production symbol is a terminal symbol.
    /// </summary>
    public abstract bool IsTerminal { get; }

    /// <summary>
    /// Gets a value indicating whether the production symbol is a non-terminal symbol.
    /// </summary>
    public abstract bool IsNonTerminal { get; }

    /// <summary>
    /// Gets a value indicating whether the production symbol is an epsilon symbol.
    /// </summary>
    public abstract bool IsEpsilon { get; }

    /// <summary>
    /// Gets a value indicating whether this production symbol is a macro.
    /// </summary>
    public abstract bool IsMacro { get; }

    /// <summary>
    /// Gets a value indicating whether the production symbol is an end-of-input symbol.
    /// </summary>
    public abstract bool IsEoi { get; }

    public static bool operator ==(Symbol left, Symbol right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Symbol left, Symbol right)
    {
        return !left.Equals(right);
    }

    public abstract override int GetHashCode();
    public abstract override bool Equals(object? obj);
    public abstract bool Equals(Symbol? other);
    public abstract bool Equals(ISymbol? other);

    /// <summary>
    /// Returns a string representation of the production symbol.
    /// </summary>
    /// <returns>A string representation of the production symbol.</returns>
    public abstract override string ToString();
    public abstract string ToNotation(NotationType notation);
}
