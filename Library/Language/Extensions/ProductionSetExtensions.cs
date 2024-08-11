using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Language.Extensions;

public static class ProductionSetExtensions
{
    /// <summary>
    /// Creates a new non-terminal symbol with a unique name based on the given non-terminal.
    /// </summary>
    /// <param name="set">The production set.</param>
    /// <param name="nonTerminal">The original non-terminal.</param>
    /// <returns>A new non-terminal symbol with a unique name.</returns>
    public static NonTerminal CreateNonTerminalPrime(this ProductionSet set, NonTerminal nonTerminal)
    {
        var name = nonTerminal.Name + "′";

        while (set.Productions.Any(p => p.Head.Name == name))
        {
            name += "′";
        }

        return new NonTerminal(name);
    }

    public static ProductionSubset CreateSubset(this ProductionSet set, IEnumerable<ProductionRule> productions)
    {
        var _productions = productions.ToArray();

        foreach (var production in _productions)
        {
            if (!set.Contains(production))
            {
                throw new InvalidOperationException("The production set does not contain the given production.");
            }
        }

        return new ProductionSubset(set, _productions);
    }
}

