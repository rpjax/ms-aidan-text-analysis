namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents a macro symbol within a context-free grammar. <br/>
/// Macros are syntactic sugar EBNF operators such as repetition, optionality, and grouping.
/// </summary>
public interface IMacroSymbol
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
    IEnumerable<Sentence> Expand(INonTerminal nonTerminal);
}

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
///    <item> <see cref="AlternativeMacro"/> </item>
/// </list>
/// </remarks>
public abstract class MacroSymbol : Symbol, IMacroSymbol
{
    /// <inheritdoc/>
    public override bool IsTerminal => false;

    /// <inheritdoc/>
    public override bool IsNonTerminal => false;

    /// <inheritdoc/>
    public override bool IsEpsilon => false;

    /// <inheritdoc/>
    public override bool IsMacro => true;

    /// <inheritdoc/>
    public override bool IsEoi => false;

    /// <inheritdoc/>
    public abstract MacroType MacroType { get; }

    /// <summary>
    /// Expands the macro symbol into a list of possible derivations.
    /// </summary>
    /// <param name="nonTerminal"></param>
    /// <returns></returns>
    public abstract IEnumerable<Sentence> Expand(NonTerminal nonTerminal);

    /// <inheritdoc/>
    public abstract IEnumerable<Sentence> Expand(INonTerminal nonTerminal);
}
