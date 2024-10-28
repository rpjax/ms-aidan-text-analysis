namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents a macro symbol within a context-free grammar. <br/>
/// Macros are syntactic sugar EBNF operators such as repetition, optionality, and grouping. <br/>
/// Macros are expanded by replacing them with a non-terminal symbol that represents the macro's expansion.
/// </summary>
/// <remarks>
/// Macros are not terminal, non-terminal, or epsilon symbols. So they must be expanded before any analysis can be done. <br/>
/// Concrete implementations of macros are: 
/// <list type="bullet">
///    <item> <see cref="GroupingMacro"/> </item>
///    <item> <see cref="OptionMacro"/> </item>
///    <item> <see cref="RepetitionMacro"/> </item>
///    <item> <see cref="PipeMacro"/> </item>
/// </list>
/// </remarks>
public interface IMacroSymbol : ISymbol
{
    /// <summary>
    /// Gets the type of the macro.
    /// </summary>
    MacroType MacroType { get; }

    /// <summary>
    /// Expands the macro symbol into a list of possible derivations.
    /// </summary>
    /// <param name="nonTerminal"></param>
    /// <returns></returns>
    IEnumerable<ISentence> Expand(INonTerminal nonTerminal);
}
