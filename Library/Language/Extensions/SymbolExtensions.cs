using Aidan.TextAnalysis.Language.Components.Symbols;
using Aidan.TextAnalysis.Language.Components.Symbols.Macros;

namespace Aidan.TextAnalysis.Language.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="ISymbol"/> interface.
/// </summary>
public static class ISymbolExtensions
{
    /// <summary>
    /// Converts the symbol to a terminal symbol.
    /// </summary>
    /// <param name="symbol">The symbol to convert.</param>
    /// <returns>The terminal symbol.</returns>
    /// <exception cref="InvalidCastException">Thrown when the symbol is not a terminal symbol.</exception>
    public static ITerminal AsTerminal(this ISymbol symbol)
    {
        if (symbol is not ITerminal terminal)
        {
            throw new InvalidCastException("The production symbol is not a terminal symbol.");
        }

        return terminal;
    }

    /// <summary>
    /// Converts the symbol to a non-terminal symbol.
    /// </summary>
    /// <param name="symbol">The symbol to convert.</param>
    /// <returns>The non-terminal symbol.</returns>
    /// <exception cref="InvalidCastException">Thrown when the symbol is not a non-terminal symbol.</exception>
    public static INonTerminal AsNonTerminal(this ISymbol symbol)
    {
        if (symbol is not INonTerminal nonTerminal)
        {
            throw new InvalidCastException("The production symbol is not a non-terminal symbol.");
        }

        return nonTerminal;
    }

    /// <summary>
    /// Converts the symbol to a macro symbol.
    /// </summary>
    /// <param name="symbol">The symbol to convert.</param>
    /// <returns>The macro symbol.</returns>
    /// <exception cref="InvalidCastException">Thrown when the symbol is not a macro symbol.</exception>
    public static IMacroSymbol AsMacro(this ISymbol symbol)
    {
        if (symbol is not IMacroSymbol productionMacro)
        {
            throw new InvalidCastException("The production symbol is not a macro.");
        }

        return productionMacro;
    }

    /// <summary>
    /// Gets a value indicating whether the production symbol is a terminal symbol.
    /// </summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <returns><c>true</c> if the symbol is a terminal symbol; otherwise, <c>false</c>.</returns>
    public static bool IsTerminal(this ISymbol symbol) => symbol.Type == SymbolType.Terminal;

    /// <summary>
    /// Gets a value indicating whether the production symbol is a non-terminal symbol.
    /// </summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <returns><c>true</c> if the symbol is a non-terminal symbol; otherwise, <c>false</c>.</returns>
    public static bool IsNonTerminal(this ISymbol symbol) => symbol.Type == SymbolType.NonTerminal;

    /// <summary>
    /// Gets a value indicating whether the production symbol is an epsilon symbol.
    /// </summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <returns><c>true</c> if the symbol is an epsilon symbol; otherwise, <c>false</c>.</returns>
    public static bool IsEpsilon(this ISymbol symbol) => symbol.Type == SymbolType.Epsilon;

    /// <summary>
    /// Gets a value indicating whether this production symbol is a macro.
    /// </summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <returns><c>true</c> if the symbol is a macro; otherwise, <c>false</c>.</returns>
    public static bool IsMacro(this ISymbol symbol) => symbol.Type == SymbolType.Macro;

    /// <summary>
    /// Gets a value indicating whether the production symbol is a pipe macro.
    /// </summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <returns><c>true</c> if the symbol is a pipe macro; otherwise, <c>false</c>.</returns>
    public static bool IsPipeMacro(this ISymbol symbol) => true
        && symbol is IMacroSymbol macro
        && macro.MacroType == MacroType.Pipe;

    /// <summary>
    /// Gets a value indicating whether the production symbol is an end-of-input symbol.
    /// </summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <returns><c>true</c> if the symbol is an end-of-input symbol; otherwise, <c>false</c>.</returns>
    public static bool IsEoi(this ISymbol symbol) => symbol.Type == SymbolType.Eoi;
}
