namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Specifies the types of symbols used in text analysis.
/// </summary>
public enum SymbolType : byte
{
    /// <summary>
    /// Represents a terminal symbol.
    /// </summary>
    Terminal,

    /// <summary>
    /// Represents a non-terminal symbol.
    /// </summary>
    NonTerminal,

    /// <summary>
    /// Represents an epsilon symbol, which denotes an empty production.
    /// </summary>
    Epsilon,

    /// <summary>
    /// Represents an end-of-input symbol.
    /// </summary>
    Eoi,

    /// <summary>
    /// Represents a macro symbol.
    /// </summary>
    Macro,
}
