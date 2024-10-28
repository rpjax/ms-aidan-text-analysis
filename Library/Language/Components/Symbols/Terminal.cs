namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents a terminal symbol in the language.
/// </summary>
public interface ITerminal : ISymbol, IComparable<ITerminal>
{
    /// <summary>
    /// Gets the value of the terminal symbol.
    /// </summary>
    ReadOnlyMemory<char> Value { get; }
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
    /// Gets the value of the terminal symbol.
    /// </summary>
    public ReadOnlyMemory<char> Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Terminal"/> class with the specified name and value.
    /// </summary>
    /// <param name="name">The name of the terminal symbol.</param>
    /// <param name="value">The value of the terminal symbol.</param>
    public Terminal(string name, ReadOnlyMemory<char> value)
    {
        Type = SymbolType.Terminal;
        Name = name;
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Terminal"/> class with the specified name and value.
    /// </summary>
    /// <param name="name">The name of the terminal symbol.</param>
    /// <param name="value">The value of the terminal symbol as a string.</param>
    public Terminal(string name, string value)
    {
        Type = SymbolType.Terminal;
        Name = name;
        Value = value.AsMemory();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Terminal"/> class with the specified value.
    /// </summary>
    /// <param name="name">The value of the terminal symbol.</param>
    public Terminal(string name)
    {
        Type = SymbolType.Terminal;
        Name = name;
        Value = string.Empty.AsMemory();
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
        return other is ITerminal terminal
            && Type == terminal.Type
            && Value.Equals(terminal.Value);
    }

    /// <summary>
    /// Compares the current terminal symbol with another terminal symbol.
    /// </summary>
    /// <param name="other">The terminal symbol to compare with the current terminal symbol.</param>
    /// <returns>A value that indicates the relative order of the terminal symbols being compared.</returns>
    public int CompareTo(ITerminal? other)
    {
        var thisStr = ToString();
        var otherStr = other?.ToString();

        return string.Compare(thisStr, otherStr, StringComparison.Ordinal);
    }
}
