using Aidan.Core.Extensions;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Components.Symbols;

namespace Aidan.TextAnalysis.Language.Extensions;

public static class IGrammarMacroExtensions
{
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

    public static IGrammar AugmentStart(this IGrammar grammar)
    {
        const string augmentStartName = "<augmented_start>";

        var startSymbol = grammar.StartSymbol;

        if (startSymbol.Name == augmentStartName)
        {
            return grammar;
        }

        var augmentedStartSymbol = new NonTerminal(augmentStartName);
        var augmentedStartProduction = new ProductionRule(
            head: augmentedStartSymbol,
            body: new Sentence(startSymbol));

        var productions = grammar.ProductionRules
            .ToList();

        productions.Add(augmentedStartProduction);

        return new Grammar(augmentedStartSymbol, productions);
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