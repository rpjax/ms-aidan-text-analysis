namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents the End of Input (EOI) terminal symbol.
/// </summary>
public sealed class Eoi : Terminal
{
    /// <summary>
    /// Gets the singleton instance of the <see cref="Eoi"/> class.
    /// </summary>
    public static Eoi Instance { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Eoi"/> class.
    /// </summary>
    public Eoi() : base("\0", "\0")
    {
        Type = SymbolType.Eoi;
    }

    /// <summary>
    /// Determines whether the specified symbol is equal to the current EOI symbol.
    /// </summary>
    /// <param name="other">The symbol to compare with the current EOI symbol.</param>
    /// <returns>true if the specified symbol is equal to the current EOI symbol; otherwise, false.</returns>
    public override bool Equals(ISymbol? other)
    {
        return other?.Type == SymbolType.Eoi;
    }

    /// <summary>
    /// Returns a string that represents the current EOI symbol.
    /// </summary>
    /// <returns>A string that represents the current EOI symbol.</returns>
    public override string ToString()
    {
        return "$";
    }
}
