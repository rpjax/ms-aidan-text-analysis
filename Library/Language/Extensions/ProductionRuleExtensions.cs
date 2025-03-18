using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Components.Symbols;

namespace Aidan.TextAnalysis.Language.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="IProductionRule"/> interface.
/// </summary>
public static class IProductionRuleExtensions
{
    /// <summary>
    /// Determines whether the specified production rule is an epsilon production.
    /// </summary>
    /// <param name="production">The production rule to check.</param>
    /// <returns>true if the production rule is an epsilon production; otherwise, false.</returns>
    public static bool IsEpsilonProduction(this IProductionRule production)
    {
        return production.Body.Length == 1
            && production.Body[0].IsEpsilon();
    }

    /// <summary>
    /// Gets the index of the specified symbol in the production rule's body.
    /// </summary>
    /// <param name="production">The production rule to search.</param>
    /// <param name="symbol">The symbol to find.</param>
    /// <returns>The zero-based index of the symbol if found; otherwise, -1.</returns>
    public static int IndexOfSymbol(
        this IProductionRule production,
        ISymbol symbol)
    {
        var index = -1;

        foreach (var item in production.Body)
        {
            index++;

            if (ReferenceEquals(item, symbol))
            {
                break;
            }
        }

        return index;
    }

    /// <summary>
    /// Expands macros in the production rule using the specified source production rules.
    /// </summary>
    /// <param name="production">The production rule to expand.</param>
    /// <param name="source">The source production rules to use for expansion.</param>
    /// <returns>An enumerable of expanded production rules.</returns>
    public static IEnumerable<IProductionRule> ExpandMacros(
         this IProductionRule production,
         IEnumerable<IProductionRule> source)
    {
        var transformedSentence = new List<ISymbol>();
        var generatedProductions = new List<IProductionRule>();

        foreach (var symbol in production.Body)
        {
            if (symbol is not IMacroSymbol macro)
            {
                transformedSentence.Add(symbol);
                continue;
            }

            var _source = source.ToList();
            _source.AddRange(generatedProductions);
            var set = new ProductionCollection(_source);

            var nonTerminal = set.CreateNonTerminalPrime(production.Head);
            var sentences = macro.Expand(nonTerminal);
            var productions = sentences
                .Select(x => new ProductionRule(
                    head: nonTerminal,
                    body: x.ToArray()
                ))
                .ToArray();

            transformedSentence.Add(nonTerminal);
            generatedProductions.AddRange(productions);
        }

        var transformedProduction = new ProductionRule(
            head: production.Head,
            body: transformedSentence
        );

        return new[] { transformedProduction }
            .Concat(generatedProductions);
    }

}
