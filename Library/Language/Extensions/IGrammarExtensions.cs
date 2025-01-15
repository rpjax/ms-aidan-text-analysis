using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Components.Symbols;

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
    public static IProductionRule[] GetProductions(
        this IGrammar grammar,
        INonTerminal nonTerminal)
    {
        return grammar.ProductionRules
            .Where(x => x.Head.Equals(nonTerminal))
            .ToArray();
    }

    /// <summary>
    /// Gets the start production rules for the grammar.
    /// </summary>
    /// <param name="grammar">The grammar to get the start production rules from.</param>
    /// <returns>An array of start production rules.</returns>
    public static IProductionRule[] GetStartProductionRules(
        this IGrammar grammar)
    {
        return GetProductions(grammar, grammar.StartSymbol);
    }

    /// <summary>
    /// Gets the augmented start production rule for the grammar.
    /// </summary>
    /// <param name="grammar">The grammar to get the augmented start production rule from.</param>
    /// <returns>The augmented start production rule.</returns>
    /// <exception cref="Exception">Thrown when the grammar does not have any production rules for the start symbol or has more than one production rule for the start symbol.</exception>
    public static IProductionRule GetAugmentedStartProduction(
        this IGrammar grammar)
    {
        var productions = GetStartProductionRules(grammar);

        if (productions.Length == 0)
        {
            throw new Exception("The grammar does not have any production rules for the start symbol.");
        }
        if (productions.Length > 1)
        {
            throw new Exception("The grammar has more than one production rule for the start symbol.");
        }

        var production = productions.First();

        if (production.Body.Length != 1)
        {
            throw new Exception("The start production rule is not in the correct form.");
        }
        if (production.Body.First().Type != SymbolType.NonTerminal)
        {
            throw new Exception("The start production rule is not in the correct form.");
        }

        return productions.First();
    }

    /// <summary>
    /// Creates a new non-terminal symbol with a prime (′) suffix.
    /// </summary>
    /// <param name="grammar">The grammar to create the non-terminal symbol for.</param>
    /// <param name="nonTerminal">The non-terminal symbol to create a prime for.</param>
    /// <returns>The new non-terminal symbol with a prime suffix.</returns>
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
}
