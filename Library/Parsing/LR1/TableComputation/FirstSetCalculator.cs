using System.Runtime.CompilerServices;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;

namespace Aidan.TextAnalysis.Parsing.LR1.TableComputation;

/// <summary>
/// Represents the first set of a grammar.
/// </summary>
public class FirstSet
{
    /// <summary>
    /// Gets the terminals in the first set.
    /// </summary>
    public ITerminal[] Terminals { get; }

    /// <summary>
    /// Gets a value indicating whether the first set contains epsilon.
    /// </summary>
    public bool ContainsEpsilon { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirstSet"/> class with the specified terminals and epsilon flag.
    /// </summary>
    /// <param name="terminals">The terminals in the first set.</param>
    /// <param name="containsEpsilon">A value indicating whether the first set contains epsilon.</param>
    public FirstSet(ITerminal[] terminals, bool containsEpsilon)
    {
        Terminals = terminals;
        ContainsEpsilon = containsEpsilon;
    }
}

/// <summary>
/// Calculates the FIRST set for a given grammar.
/// </summary>
public class FirstSetCalculator
{
    /// <summary>
    /// Gets the grammar used for the FIRST set calculation.
    /// </summary>
    private IGrammar Grammar { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirstSetCalculator"/> class with the specified grammar.
    /// </summary>
    /// <param name="grammar">The grammar to use for the FIRST set calculation.</param>
    public FirstSetCalculator(IGrammar grammar)
    {
        Grammar = grammar;
    }

    /// <summary>
    /// Computes the FIRST set for the specified sentence.
    /// </summary>
    /// <param name="sentence">The sentence to compute the FIRST set for.</param>
    /// <param name="callStack">The call stack to use for recursion detection (optional).</param>
    /// <returns>The computed FIRST set.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FirstSet ComputeFirstSet(
        ISentence sentence,
        List<INonTerminal>? callStack = null)
    {
        /* if the sentence is empty, then it contains epsilon */
        if (sentence.Length == 0)
        {
            return new FirstSet(
                terminals: Array.Empty<ITerminal>(),
                containsEpsilon: true);
        }

        var firstSymbol = sentence[0];

        /* if the first symbol is a terminal, then it is the only terminal in the set */
        if (firstSymbol.Type == SymbolType.Terminal)
        {
            return new FirstSet(
                terminals: new[] { firstSymbol.AsTerminal() },
                containsEpsilon: false);
        }

        /* if the first symbol is a non-terminal, then compute the first set of the non-terminal */

        var terminals = new List<ITerminal>();
        var containsEpsilon = false;

        /* if the call stack is null, then create a new list */
        callStack ??= new List<INonTerminal>();

        foreach (var symbol in sentence)
        {
            var isLastSymbol = symbol == sentence[^1];

            /* if the symbol is a terminal, then add it to the set and break */
            if (symbol is ITerminal terminal)
            {
                terminals.Add(terminal);
                break;
            }

            if (symbol.Type == SymbolType.Epsilon)
            {
                if (isLastSymbol)
                {
                    containsEpsilon = true;
                }

                continue;
            }

            /* if the symbol is not a terminal, then it must be a non-terminal */
            if (symbol is not INonTerminal nonTerminal)
            {
                throw new InvalidOperationException("The symbol is neither a terminal nor a non-terminal.");
            }

            /* compute the first set of the non-terminal */
            var firstSet = ComputeFirstSet(nonTerminal, callStack);

            /* add the terminals of the first set to the set */
            terminals.AddRange(firstSet.Terminals);

            /* if the first set does not contain epsilon, then break */
            if (!firstSet.ContainsEpsilon)
            {
                break;
            }

            /* if the first set contains epsilon and the symbol is the last symbol, then the set contains epsilon */
            if (isLastSymbol)
            {
                containsEpsilon = true;
            }
        }

        var uniqueTerminals = terminals
            .Distinct(new TerminalEqualityComparer())
            .ToArray();

        return new FirstSet(
            terminals: uniqueTerminals,
            containsEpsilon: containsEpsilon);
    }

    /// <summary>
    /// Computes the FIRST set for the specified non-terminal symbol.
    /// </summary>
    /// <param name="nonTerminal">The non-terminal symbol to compute the FIRST set for.</param>
    /// <param name="callStack">The call stack to use for recursion detection.</param>
    /// <returns>The computed FIRST set.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private FirstSet ComputeFirstSet(
        INonTerminal nonTerminal,
        List<INonTerminal> callStack)
    {
        /* im not sure if this is correct, becausethe absence of epsilon causes the calling loop to break */
        if (callStack.Contains(nonTerminal, new NonTerminalEqualityComparer()))
        {
            return new FirstSet(
                terminals: Array.Empty<ITerminal>(),
                containsEpsilon: false);
        }

        var productions = Grammar.GetProductions(nonTerminal);
        var terminals = new List<ITerminal>();
        var containsEpsilon = false;

        callStack.Add(nonTerminal);

        foreach (var production in productions)
        {
            var firstSet = ComputeFirstSet(production.Body, callStack);
            var sentence = production.Body;

            terminals.AddRange(firstSet.Terminals);

            if (firstSet.ContainsEpsilon)
            {
                containsEpsilon = true;
            }
        }

        callStack.Remove(nonTerminal);

        var uniqueTerminals = terminals
            .Distinct(new TerminalEqualityComparer())
            .ToArray();

        return new FirstSet(
            terminals: uniqueTerminals,
            containsEpsilon: containsEpsilon);
    }
}
