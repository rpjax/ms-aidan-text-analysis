using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Language.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="ProductionCollection"/> class.
/// </summary>
public static class ProductionCollectionExtensions
{
    /// <summary>
    /// Creates a new non-terminal symbol with a prime (′) appended to its name.
    /// </summary>
    /// <param name="productions">The collection of production rules.</param>
    /// <param name="nonTerminal">The non-terminal symbol to prime.</param>
    /// <returns>A new non-terminal symbol with a prime appended to its name.</returns>
    public static INonTerminal CreateNonTerminalPrime(
        this ProductionCollection productions,
        INonTerminal nonTerminal)
    {
        var name = nonTerminal.Name + "′";

        while (productions.Any(p => p.Head.Name == name))
        {
            name += "′";
        }

        return new NonTerminal(name);
    }
}
