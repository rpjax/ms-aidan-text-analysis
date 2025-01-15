using System.Text;
using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.GDef.Components;

/// <summary>
/// Represents a grammar for a generalized definition (GDef).
/// </summary>
public class GdefGrammar : IGrammar
{
    /// <summary>
    /// Gets the lexemes used in the grammar.
    /// </summary>
    public IReadOnlyList<GDefLexeme> Lexemes { get; }

    /// <summary>
    /// Gets the characters that should be ignored during parsing.
    /// </summary>
    public IReadOnlyList<char> IgnoredChars { get; }

    /// <summary>
    /// Gets the non-terminal symbols in the grammar.
    /// </summary>
    public IReadOnlyList<INonTerminal> NonTerminals { get; private set; }

    /// <summary>
    /// Gets the terminal symbols in the grammar.
    /// </summary>
    public IReadOnlyList<ITerminal> Terminals { get; private set; }

    /// <summary>
    /// Gets the production rules of the grammar.
    /// </summary>
    public IReadOnlyList<IProductionRule> ProductionRules { get; private set; }

    /// <summary>
    /// Gets the start symbol of the grammar.
    /// </summary>
    public INonTerminal StartSymbol { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GdefGrammar"/> class.
    /// </summary>
    /// <param name="lexemes">The collection of lexemes used in the grammar.</param>
    /// <param name="ignoredChars">The collection of characters to ignore during parsing.</param>
    /// <param name="productions">The collection of production rules defining the grammar.</param>
    /// <param name="start">The start symbol of the grammar.</param>
    public GdefGrammar(
        IEnumerable<GDefLexeme> lexemes,
        IEnumerable<char> ignoredChars,
        IEnumerable<IProductionRule> productions,
        INonTerminal start)
    {
        Lexemes = lexemes.ToArray();
        IgnoredChars = new Charset(ignoredChars);

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
    /// Returns a string representation of the current grammar.
    /// </summary>
    /// <returns>A string describing the grammar, including the start symbol and production rules.</returns>
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
