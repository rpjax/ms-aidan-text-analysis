using Aidan.TextAnalysis.Helpers;

namespace Aidan.TextAnalysis.Language.Components.Symbols;

/// <summary>
/// Represents a terminal symbol in the language.
/// </summary>
public interface ITerminal : ISymbol, IComparable<ITerminal>
{

}

/// <summary>
/// Represents a concrete implementation of a terminal symbol.
/// </summary>
public class Terminal : ITerminal
{
    /// <summary>
    /// Gets the type of the symbol.
    /// </summary>
    public SymbolType Type { get; protected set; }

    /// <summary>
    /// Gets the name of the symbol.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Terminal"/> class with the specified value.
    /// </summary>
    /// <param name="name">The value of the terminal symbol.</param>
    public Terminal(string name)
    {
        Type = SymbolType.Terminal;
        Name = name;
    }

    /// <summary>
    /// Returns a string that represents the current terminal symbol.
    /// </summary>
    /// <returns>A string that represents the current terminal symbol.</returns>
    public override string ToString()
    {
        return $"\"{Name}\"";
    }

    /// <summary>
    /// Determines whether the specified symbol is equal to the current terminal symbol.
    /// </summary>
    /// <param name="other">The symbol to compare with the current terminal symbol.</param>
    /// <returns>true if the specified symbol is equal to the current terminal symbol; otherwise, false.</returns>
    public virtual bool Equals(ISymbol? other)
    {
        return other is not null
            && Type == other.Type
            && Name == other.Name;
    }

    /// <summary>
    /// Gets a value based hash for the terminal.
    /// </summary>
    /// <returns>A signed 32 bit integer hash.</returns>
    public override int GetHashCode()
    {
        return HashHelper.ComputeHash(Type, Name);
    }

    /// <summary>
    /// Compares the current terminal symbol with another terminal symbol.
    /// </summary>
    /// <param name="other">The terminal symbol to compare with the current terminal symbol.</param>
    /// <returns>A value that indicates the relative order of the terminal symbols being compared.</returns>
    public int CompareTo(ITerminal? other)
    {
        var thisName = Name;
        var otherName = other?.Name;

        return string.Compare(thisName, otherName, StringComparison.Ordinal);
    }
}
