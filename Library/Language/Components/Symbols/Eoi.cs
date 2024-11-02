namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents the End of Input (EOI) terminal symbol.
/// </summary>
public sealed class Eoi : Terminal
{
    /// <summary>
    /// The character representing the End of Input (EOI).
    /// </summary>
    public const char EoiChar = '\0';

    /// <summary>
    /// The string representing the End of Input (EOI).
    /// </summary>
    public const string EoiString = "\0";

    /// <summary>
    /// Gets the singleton instance of the <see cref="Eoi"/> class.
    /// </summary>
    public static Eoi Instance { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Eoi"/> class.
    /// </summary>
    public Eoi() : base(EoiString)
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
