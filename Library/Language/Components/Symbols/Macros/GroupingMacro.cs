namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents a grouping macro which is a type of macro symbol.
/// </summary>
public class GroupingMacro : IMacroSymbol
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
    /// Gets the symbols that are grouped by this macro.
    /// </summary>
    public ISymbol[] GroupedSymbols { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupingMacro"/> class with the specified symbols.
    /// </summary>
    /// <param name="symbols">The symbols to be grouped by this macro.</param>
    public GroupingMacro(params ISymbol[] symbols)
    {
        Type = SymbolType.Macro;
        Name = "Grouping Macro";
        MacroType = MacroType.Grouping;
        GroupedSymbols = symbols;
    }

    /// <summary>
    /// Determines whether the specified symbol is equal to the current grouping macro.
    /// </summary>
    /// <param name="other">The symbol to compare with the current grouping macro.</param>
    /// <returns>true if the specified symbol is equal to the current grouping macro; otherwise, false.</returns>
    public bool Equals(ISymbol? other)
    {
        return other is GroupingMacro macro
            && GroupedSymbols.Equals(macro.GroupedSymbols);
    }

    /// <summary>
    /// Expands the grouping macro into a sequence of sentences.
    /// </summary>
    /// <param name="nonTerminal">The non-terminal symbol to expand.</param>
    /// <returns>An enumerable collection of sentences.</returns>
    public IEnumerable<ISentence> Expand(INonTerminal nonTerminal)
    {
        yield return new Sentence(GroupedSymbols);
    }

    /// <summary>
    /// Returns a string that represents the current grouping macro.
    /// </summary>
    /// <returns>A string that represents the current grouping macro.</returns>
    public override string ToString()
    {
        return $"( {new Sentence(GroupedSymbols)} )";
    }
}
