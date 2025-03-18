using Aidan.TextAnalysis.Helpers;
using Aidan.TextAnalysis.Language.Extensions;

namespace Aidan.TextAnalysis.Language.Components.Symbols.Macros;

/// <summary>
/// Represents a repetition macro. It is analoguous to EBNF's "{ }" operator.
/// </summary>
public class OneOrMoreMacro : IMacroSymbol
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
    public ISentence Symbols { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZeroOrMoreMacro"/> class with the specified symbols.
    /// </summary>
    /// <param name="symbols">The symbols to be included in the repeated sentence.</param>
    public OneOrMoreMacro(params ISymbol[] symbols)
    {
        var sentence = new Sentence(symbols);

        Type = SymbolType.Macro;
        Name = "One or More Macro";
        MacroType = MacroType.OneOrMore;
        Symbols = sentence;
    }

    /// <summary>
    /// Returns a string that represents the current repetition macro.
    /// </summary>
    /// <returns>A string that represents the current repetition macro.</returns>
    public override string ToString()
    {
        // sentential form
        return $"{Symbols}+";
    }

    /// <summary>
    /// Determines whether the specified symbol is equal to the current repetition macro.
    /// </summary>
    /// <param name="other">The symbol to compare with the current repetition macro.</param>
    /// <returns>true if the specified symbol is equal to the current repetition macro; otherwise, false.</returns>
    public bool Equals(ISymbol? other)
    {
        return other is OneOrMoreMacro macro
            && macro.MacroType == MacroType
            && macro.Symbols.SequenceEqual(Symbols);
    }

    /// <summary>
    /// Gets a value based hash for the zero or more macro.
    /// </summary>
    /// <returns>A signed 32 bit integer hash.</returns>
    public override int GetHashCode()
    {
        return HashHelper.ComputeHash(Type, Name, MacroType, Symbols);
    }

    /// <summary>
    /// Expands the repetition macro into a sequence of sentences.
    /// </summary>
    /// <param name="nonTerminal">The non-terminal symbol to expand.</param>
    /// <returns>An enumerable collection of sentences.</returns>
    public IEnumerable<ISentence> Expand(INonTerminal nonTerminal)
    {
        yield return Symbols.Add(new ZeroOrMoreMacro(Symbols.ToArray()));
    }

}
