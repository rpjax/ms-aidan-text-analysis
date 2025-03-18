using Aidan.TextAnalysis.Helpers;

namespace Aidan.TextAnalysis.Language.Components.Symbols.Macros;

/// <summary>
/// Represents the alternative macro symbol.
/// </summary>
public class PipeMacro : IMacroSymbol
{
    /// <summary>
    /// Gets the type of the symbol.
    /// </summary>
    public SymbolType Type { get; }

    /// <summary>
    /// Gets the name of the macro.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the type of the macro.
    /// </summary>
    public MacroType MacroType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PipeMacro"/> class.
    /// </summary>
    public PipeMacro()
    {
        Type = SymbolType.Macro;
        Name = "Pipe Macro";
        MacroType = MacroType.Pipe;
    }

    /// <summary>
    /// Returns a string that represents the current pipe macro.
    /// </summary>
    /// <returns>A string that represents the current pipe macro.</returns>
    public override string ToString()
    {
        return "|";
    }

    /// <summary>
    /// Determines whether the specified symbol is equal to the current pipe macro.
    /// </summary>
    /// <param name="other">The symbol to compare with the current pipe macro.</param>
    /// <returns>true if the specified symbol is equal to the current pipe macro; otherwise, false.</returns>
    public bool Equals(ISymbol? other)
    {
        return other is IMacroSymbol macro
            && macro.MacroType == MacroType.Pipe;
    }

    /// <summary>
    /// Gets a value based hash for the pipe macro.
    /// </summary>
    /// <returns>A signed 32 bit integer hash.</returns>
    public override int GetHashCode()
    {
        return HashHelper.ComputeHash(Type, Name, MacroType);
    }

    /// <summary>
    /// Expands the pipe macro into a sequence of sentences.
    /// </summary>
    /// <param name="nonTerminal">The non-terminal symbol to expand.</param>
    /// <returns>An enumerable collection of sentences.</returns>
    /// <exception cref="InvalidOperationException">Thrown when attempting to expand a pipe macro.</exception>
    public IEnumerable<ISentence> Expand(INonTerminal nonTerminal)
    {
        throw new InvalidOperationException();
    }

}
