using Aidan.TextAnalysis.Helpers;

namespace Aidan.TextAnalysis.Language.Components.Symbols.Macros;

/// <summary>
/// Represents an option macro. It is analoguous to EBNF's "[ ]" operator.
/// </summary>
public class NullableMacro : IMacroSymbol
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
    public ISentence NullableSentence { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableMacro"/> class with the specified symbols.
    /// </summary>
    /// <param name="symbols">The symbols to be included in the optional sentence.</param>
    public NullableMacro(params ISymbol[] symbols)
    {
        var sentence = new Sentence(symbols);

        Type = SymbolType.Macro;
        Name = sentence.ToString();
        MacroType = MacroType.Nullable;
        NullableSentence = sentence;
    }

    /// <summary>
    /// Returns a string that represents the current option macro.
    /// </summary>
    /// <returns>A string that represents the current option macro.</returns>
    public override string ToString()
    {
        return $"{NullableSentence.ToString()}?";
    }

    /// <summary>
    /// Determines whether the specified symbol is equal to the current option macro.
    /// </summary>
    /// <param name="other">The symbol to compare with the current option macro.</param>
    /// <returns>true if the specified symbol is equal to the current option macro; otherwise, false.</returns>
    public bool Equals(ISymbol? other)
    {
        return other is NullableMacro macro
            && macro.MacroType == MacroType
            && macro.NullableSentence.SequenceEqual(NullableSentence);
    }

    /// <summary>
    /// Gets a value based hash for the nullable macro.
    /// </summary>
    /// <returns>A signed 32 bit integer hash.</returns>
    public override int GetHashCode()
    {
        return HashHelper.ComputeHash(Type, Name, MacroType, NullableSentence);
    }

    /// <summary>
    /// Expands the option macro into a sequence of sentences.
    /// </summary>
    /// <param name="nonTerminal">The non-terminal symbol to expand.</param>
    /// <returns>An enumerable collection of sentences.</returns>
    public IEnumerable<ISentence> Expand(INonTerminal nonTerminal)
    {
        yield return NullableSentence;
        yield return new Sentence(new Epsilon());
    }
}
