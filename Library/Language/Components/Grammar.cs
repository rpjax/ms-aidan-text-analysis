using System.Text;
using Aidan.TextAnalysis.Language.Components.Symbols;

namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Definition of a context-free grammar. (Chomsky hierarchy type 2) <br/>
/// </summary>
/// <remarks>
/// The grammar is defined as a 4-tuple G = (N, T, P, S), where: <br/>
/// - N is a set of non-terminal symbols. <br/>
/// - T is a set of terminal symbols. <br/>
/// - P is a set of production rules. <br/>
/// - S is the start symbol. <br/>
/// </remarks>
public interface IGrammar
{
    /// <summary>
    /// Gets the list of non-terminal symbols.
    /// </summary>
    IReadOnlyList<INonTerminal> NonTerminals { get; }

    /// <summary>
    /// Gets the list of terminal symbols.
    /// </summary>
    IReadOnlyList<ITerminal> Terminals { get; }

    /// <summary>
    /// Gets the list of production rules.
    /// </summary>
    IReadOnlyList<IProductionRule> ProductionRules { get; }

    /// <summary>
    /// Gets the start symbol.
    /// </summary>
    INonTerminal StartSymbol { get; }
}

/// <summary>
/// Definition of a context-free grammar. (Chomsky hierarchy type 2) <br/>
/// </summary>
/// <remarks>
/// The grammar is defined as a 4-tuple G = (N, T, P, S), where: <br/>
/// - N is a set of non-terminal symbols. <br/>
/// - T is a set of terminal symbols. <br/>
/// - P is a set of production rules. <br/>
/// - S is the start symbol. <br/>
/// </remarks>
public class Grammar : IGrammar
{
    /// <summary>
    /// Gets the list of non-terminal symbols.
    /// </summary>
    public IReadOnlyList<INonTerminal> NonTerminals { get; private set; }

    /// <summary>
    /// Gets the list of terminal symbols.
    /// </summary>
    public IReadOnlyList<ITerminal> Terminals { get; private set; }

    /// <summary>
    /// Gets the list of production rules.
    /// </summary>
    public IReadOnlyList<IProductionRule> ProductionRules { get; private set; }

    /// <summary>
    /// Gets the start symbol.
    /// </summary>
    public INonTerminal StartSymbol { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Grammar"/> class.
    /// </summary>
    /// <param name="start">The start symbol.</param>
    /// <param name="productions">The collection of production rules.</param>
    public Grammar(INonTerminal start, IEnumerable<IProductionRule> productions)
    {
        NonTerminals = productions
            .Select(x => x.Head)
            .Distinct(new NonTerminalEqualityComparer())
            .ToArray();
        Terminals = productions
            .SelectMany(x => x.Body)
            .OfType<ITerminal>()
            .Distinct(new TerminalEqualityComparer())
            .ToArray();

        ProductionRules = productions.ToArray();
        StartSymbol = start;
    }

    /// <summary>
    /// Returns a string that represents the current grammar.
    /// </summary>
    /// <returns>A string that represents the current grammar.</returns>
    public override string ToString()
    {
        var builder = new StringBuilder();

        builder.AppendLine($"Start Symbol: {StartSymbol}");
        builder.AppendLine("Productions:");

        foreach (var production in ProductionRules)
        {
            builder.AppendLine(production.ToString());
        }

        return builder.ToString();
    }
}
