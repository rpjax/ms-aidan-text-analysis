using Aidan.TextAnalysis.Language.Extensions;

namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents a repetition macro. It is analoguous to EBNF's "{ }" operator.
/// </summary>
public class RepetitionMacro : IMacroSymbol
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
    /// Gets the sentence that is repeated by this macro.
    /// </summary>
    public ISentence RepeatedSentence { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepetitionMacro"/> class with the specified symbols.
    /// </summary>
    /// <param name="symbols">The symbols to be included in the repeated sentence.</param>
    public RepetitionMacro(params ISymbol[] symbols)
    {
        var sentence = new Sentence(symbols);

        Type = SymbolType.Macro;
        Name = "Repetition Macro";
        MacroType = MacroType.Repetition;
        RepeatedSentence = sentence;
    }

    /// <summary>
    /// Expands the repetition macro into a sequence of sentences.
    /// </summary>
    /// <param name="nonTerminal">The non-terminal symbol to expand.</param>
    /// <returns>An enumerable collection of sentences.</returns>
    public IEnumerable<ISentence> Expand(INonTerminal nonTerminal)
    {
        yield return RepeatedSentence.Add(nonTerminal);
        yield return new Sentence(new Epsilon());
    }

    /// <summary>
    /// Returns a string that represents the current repetition macro.
    /// </summary>
    /// <returns>A string that represents the current repetition macro.</returns>
    public override string ToString()
    {
        // sentential form
        return $"{{ {RepeatedSentence} }}";
    }

    /// <summary>
    /// Determines whether the specified symbol is equal to the current repetition macro.
    /// </summary>
    /// <param name="other">The symbol to compare with the current repetition macro.</param>
    /// <returns>true if the specified symbol is equal to the current repetition macro; otherwise, false.</returns>
    public bool Equals(ISymbol? other)
    {
        return other is RepetitionMacro macro
            && macro.MacroType == MacroType
            && macro.RepeatedSentence.SequenceEqual(RepeatedSentence);
    }
}
