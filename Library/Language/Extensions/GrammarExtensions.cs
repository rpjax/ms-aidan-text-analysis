using Aidan.Core.Extensions;
using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Language.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="IGrammar"/> interface.
/// </summary>
public static class IGrammarExtensions
{
    /// <summary>
    /// Gets all symbols (terminals and non-terminals) in the grammar.
    /// </summary>
    /// <param name="grammar">The grammar to get the symbols from.</param>
    /// <returns>An array of all symbols in the grammar.</returns>
    public static ISymbol[] GetAllSymbols(
        this IGrammar grammar)
    {
        return grammar.NonTerminals
            .Cast<ISymbol>()
            .Concat(grammar.Terminals)
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Gets all production rules for a specified non-terminal symbol.
    /// </summary>
    /// <param name="grammar">The grammar to get the production rules from.</param>
    /// <param name="nonTerminal">The non-terminal symbol to get the production rules for.</param>
    /// <returns>An array of production rules for the specified non-terminal symbol.</returns>
    public static IProductionRule[] GetProductionRulesForNonTerminal(
        this IGrammar grammar,
        INonTerminal nonTerminal)
    {
        return grammar.ProductionRules
            .Where(x => x.Head.Equals(nonTerminal))
            .ToArray();
    }

    /// <summary>
    /// Ensures that the grammar does not contain any macros.
    /// </summary>
    /// <param name="grammar">The grammar to check for macros.</param>
    /// <exception cref="Exception">Thrown when the grammar contains macros.</exception>
    public static void EnsureNoMacros(
        this IGrammar grammar)
    {
        if (grammar.ProductionRules.Any(x => x.Body.Any(x => x.IsMacro())))
        {
            throw new Exception("Grammar contains macros.");
        }
    }

    public static bool ContainsMacro(this IGrammar grammar)
    {
        return grammar.ProductionRules
            .Any(x => x.Body.Any(x => x.IsMacro()));
    }

    public static NonTerminal CreateNonTerminalPrime(
        this IGrammar grammar,
        INonTerminal nonTerminal)
    {
        var name = nonTerminal.Name + "′";

        while (grammar.ProductionRules.Any(p => p.Head.Name == name))
        {
            name += "′";
        }

        return new NonTerminal(name);
    }

    public static IGrammar ExpandMacros(this IGrammar grammar)
    {
        var start = grammar.StartSymbol;
        var productions = new List<IProductionRule>(grammar.ProductionRules);

        /* Expand all other macros */
        while (true)
        {
            var macroProductions = productions
                .Where(x => x.Body.Any(x => x.IsMacro()))
                .ToArray();

            if (macroProductions.IsEmpty())
            {
                break;
            }

            foreach (var macroProduction in macroProductions)
            {
                var expandedProductions = macroProduction.ExpandMacros(productions)
                    .ToArray();

                productions.Remove(macroProduction);
                productions.AddRange(expandedProductions);
            }
        }

        return new Grammar(start, productions);
    }

    private static IGrammar ExpandPipeMacros(this IGrammar grammar)
    {
        var start = grammar.StartSymbol;
        var productions = new List<IProductionRule>(grammar.ProductionRules);

        /* Expand all other macros */
        while (true)
        {
            var macroProductions = productions
                .Where(x => x.Body.Any(x => x.IsPipeMacro()))
                .ToArray();

            if (macroProductions.IsEmpty())
            {
                break;
            }

            foreach (var macroProduction in macroProductions)
            {
                var expandedProductions = macroProduction.ExpandMacros(productions)
                    .ToArray();

                productions.Remove(macroProduction);
                productions.AddRange(expandedProductions);
            }
        }

        return new Grammar(start, productions);
    }

}
