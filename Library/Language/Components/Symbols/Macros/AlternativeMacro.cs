namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents an expanded alternative macro. It is used to represent a set of alternatives in a grammar.
/// </summary>
public class AlternativeMacro : IMacroSymbol
{
    /// <summary>
    /// Gets the type of the symbol.
    /// </summary>
    public SymbolType Type { get; }

    /// <summary>
    /// Gets the name of the symbol.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the type of the macro.
    /// </summary>
    public MacroType MacroType { get; }

    /// <summary>
    /// Gets the alternatives represented by this macro.
    /// </summary>
    public ISentence[] Alternatives { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlternativeMacro"/> class.
    /// </summary>
    /// <param name="alternatives">The alternatives represented by this macro.</param>
    /// <exception cref="ArgumentException">Thrown when no alternatives are provided.</exception>
    public AlternativeMacro(params ISentence[] alternatives)
    {
        Type = SymbolType.Macro;
        Name = "Alternative Macro";
        MacroType = MacroType.Alternative;
        Alternatives = alternatives;

        if (alternatives.Length == 0)
        {
            throw new ArgumentException("The alternation macro must contain at least one alternative.");
        }
    }

    /// <summary>
    /// Determines whether the specified symbol is equal to the current symbol.
    /// </summary>
    /// <param name="other">The symbol to compare with the current symbol.</param>
    /// <returns><c>true</c> if the specified symbol is equal to the current symbol; otherwise, <c>false</c>.</returns>
    public bool Equals(ISymbol? other)
    {
        return other is AlternativeMacro macro
            && macro.Alternatives.SequenceEqual(Alternatives);
    }

    /// <summary>
    /// Expands the macro into its alternatives.
    /// </summary>
    /// <param name="nonTerminal">The non-terminal symbol to expand.</param>
    /// <returns>The alternatives represented by this macro.</returns>
    public IEnumerable<ISentence> Expand(INonTerminal nonTerminal)
    {
        return Alternatives;
    }

    /// <summary>
    /// Returns a string that represents the current symbol.
    /// </summary>
    /// <returns>A string that represents the current symbol.</returns>
    public override string ToString()
    {
        return string.Join(" | ", Alternatives.Select(x => x.ToString()));
    }
}
