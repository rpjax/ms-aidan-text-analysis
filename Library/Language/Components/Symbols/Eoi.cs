using Aidan.TextAnalysis.Helpers;

namespace Aidan.TextAnalysis.Language.Components.Symbols;

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
    public Eoi() : base(name: EoiString)
    {
        Type = SymbolType.Eoi;
    }

    /// <summary>
    /// Returns a string that represents the current EOI symbol.
    /// </summary>
    /// <returns>A string that represents the current EOI symbol.</returns>
    public override string ToString()
    {
        return "$";
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
    /// Gets a value based hash for the EOI.
    /// </summary>
    /// <returns>A signed 32 bit integer hash.</returns>
    public override int GetHashCode()
    {
        return HashHelper.ComputeHash(Type);
    }

}
