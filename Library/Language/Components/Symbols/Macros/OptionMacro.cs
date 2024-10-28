namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents an option macro. It is analoguous to EBNF's "[ ]" operator.
/// </summary>
public class OptionMacro : IMacroSymbol
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
    /// Gets the optional sentence represented by this macro.
    /// </summary>
    public ISentence OptionalSentence { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionMacro"/> class with the specified symbols.
    /// </summary>
    /// <param name="symbols">The symbols to be included in the optional sentence.</param>
    public OptionMacro(params ISymbol[] symbols)
    {
        var sentence = new Sentence(symbols);

        Type = SymbolType.Macro;
        Name = sentence.ToString();
        MacroType = MacroType.Option;
        OptionalSentence = sentence;
    }

    /// <summary>
    /// Expands the option macro into a sequence of sentences.
    /// </summary>
    /// <param name="nonTerminal">The non-terminal symbol to expand.</param>
    /// <returns>An enumerable collection of sentences.</returns>
    public IEnumerable<ISentence> Expand(INonTerminal nonTerminal)
    {
        yield return OptionalSentence;
        yield return new Sentence(new Epsilon());
    }

    /// <summary>
    /// Returns a string that represents the current option macro.
    /// </summary>
    /// <returns>A string that represents the current option macro.</returns>
    public override string ToString()
    {
        return $"[ {OptionalSentence.ToString()} ]";
    }

    /// <summary>
    /// Determines whether the specified symbol is equal to the current option macro.
    /// </summary>
    /// <param name="other">The symbol to compare with the current option macro.</param>
    /// <returns>true if the specified symbol is equal to the current option macro; otherwise, false.</returns>
    public bool Equals(ISymbol? other)
    {
        return other is OptionMacro macro
            && macro.MacroType == MacroType
            && macro.OptionalSentence.SequenceEqual(OptionalSentence);
    }
}
