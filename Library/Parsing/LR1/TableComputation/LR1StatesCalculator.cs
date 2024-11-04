using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;
using Aidan.TextAnalysis.Parsing.LR1.Components;

namespace Aidan.TextAnalysis.Parsing.LR1.TableComputation;

/// <summary>
/// Represents a calculator for computing the states of an LR(1) parser.
/// </summary>
public class LR1StatesCalculator
{
    /// <summary>
    /// Gets the grammar used for computing the states.
    /// </summary>
    private IGrammar Grammar { get; }

    /// <summary>
    /// Gets the calculator for computing the first set of a grammar.
    /// </summary>
    private FirstSetCalculator FirstSetCalculator { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LR1StatesCalculator"/> class with the specified grammar.
    /// </summary>
    /// <param name="grammar">The grammar to use for computing the states.</param>
    /// <exception cref="ArgumentException">Thrown when the grammar contains macros.</exception>
    public LR1StatesCalculator(IGrammar grammar)
    {
        Grammar = grammar;
        FirstSetCalculator = new FirstSetCalculator(grammar);

        if (grammar.ContainsMacro())
        {
            throw new ArgumentException("The grammar contains macros.");
        }
    }

    /// <summary>
    /// Computes the states of an LR(1) parser for the specified grammar.
    /// </summary>
    /// <param name="grammar">The grammar to compute the states for.</param>
    /// <returns>An array of computed LR(1) states.</returns>
    public static LR1State[] ComputeStates(IGrammar grammar)
    {
        return new LR1StatesCalculator(grammar).ComputeStates();
    }

    /// <summary>
    /// Computes the states of an LR(1) parser.
    /// </summary>
    /// <returns>An array of computed LR(1) states.</returns>
    public LR1State[] ComputeStates()
    {
        var initialState = ComputeInitialState();
        var states = new List<LR1State> { initialState };
        var processedStates = new HashSet<LR1State>();

        while (true)
        {
            var newStatesCounter = 0;
            var statesToProcess = states
                .Except(processedStates)
                .ToArray();

            foreach (var state in statesToProcess)
            {
                var gotoStates = ComputeGotoStates(state);

                /* mark the state as processed */
                processedStates.Add(state);

                /* adds the goto states to the list of states */
                foreach (var gotoState in gotoStates)
                {
                    if (states.Any(s => s.Equals(gotoState)))
                    {
                        continue;
                    }

                    states.Add(gotoState);
                    newStatesCounter++;
                }
            }

            /* if no new states were added, then the computation is complete */
            if (newStatesCounter == 0)
            {
                break;
            }
        }

        return states.ToArray();
    }

    /// <summary>
    /// Computes the initial state of the LR(1) parser.
    /// </summary>
    /// <returns>The computed initial state.</returns>
    private LR1State ComputeInitialState()
    {
        var startProduction = Grammar.GetAugmentedStartProduction();

        var kernelItem = new LR1Item(
            production: startProduction,
            position: 0,
            lookaheads: new Eoi());

        var kernel = new LR1Kernel(kernelItem);
        var closure = ComputeKernelClosure(kernel);

        return new LR1State(
            kernel: kernel,
            closure: closure);
    }

    /// <summary>
    /// Computes the closure of the specified kernel.
    /// </summary>
    /// <param name="kernel">The kernel to compute the closure for.</param>
    /// <returns>The computed closure.</returns>
    private LR1Closure ComputeKernelClosure(LR1Kernel kernel)
    {
        var items = new List<LR1Item>(kernel);
        var processedItems = new HashSet<LR1Item>();

        while (true)
        {
            var counter = 0;
            var itemsToProcess = items
                .Except(processedItems)
                .ToArray();

            foreach (var item in itemsToProcess)
            {
                /* compute the closure of the item */
                var itemClosure = ComputeItemClosure(item);

                /* mark the item as processed */
                processedItems.Add(item);

                /* add the computed closure items to the list */
                foreach (var newItem in itemClosure)
                {
                    if (items.Any(i => i.Equals(newItem)))
                    {
                        continue;
                    }

                    items.Add(newItem);
                    counter++;
                }
            }

            /* if no new items were added, then the closure is complete */
            if (counter == 0)
            {
                break;
            }
        }

        /* remove the kernel items from the closure */
        items.RemoveRange(0, kernel.Length);

        return new LR1Closure(items.ToArray());
    }

    /// <summary>
    /// Computes the closure of the specified item.
    /// </summary>
    /// <param name="item">The item to compute the closure for.</param>
    /// <returns>An array of computed closure items.</returns>
    private LR1Item[] ComputeItemClosure(LR1Item item)
    {
        var symbol = item.Symbol;
        var symbolType = symbol?.Type;

        if (symbol is null)
        {
            return Array.Empty<LR1Item>();
        }
        if (symbolType != SymbolType.NonTerminal)
        {
            return Array.Empty<LR1Item>();
        }

        var nonTerminal = symbol.AsNonTerminal();
        var productions = Grammar.GetProductions(nonTerminal);
        var lookaheads = ComputeLookaheads(item, item.GetBeta());

        var items = new List<LR1Item>();

        foreach (var production in productions)
        {
            var newItem = new LR1Item(
                production: production,
                position: 0,
                lookaheads: lookaheads);

            items.Add(newItem);
        }

        return items.ToArray();
    }

    /// <summary>
    /// Computes the lookaheads for the specified item and beta.
    /// </summary>
    /// <param name="item">The item to compute the lookaheads for.</param>
    /// <param name="beta">The beta to compute the lookaheads for.</param>
    /// <returns>An array of computed lookaheads.</returns>
    private ITerminal[] ComputeLookaheads(
        LR1Item item,
        ISentence beta)
    {
        var originalLookaheads = item.Lookaheads;
        var firstSet = ComputeFirstSet(beta);
        var lookaheads = new List<ITerminal>(firstSet.Terminals);

        if (firstSet.ContainsEpsilon)
        {
            lookaheads.AddRange(originalLookaheads);
        }

        /* remove duplicates in case the original lookaheads converge with the first set */
        return lookaheads
            .Distinct(new TerminalEqualityComparer())
            .ToArray();
    }

    /// <summary>
    /// Computes the first set for the specified beta.
    /// </summary>
    /// <param name="beta">The beta to compute the first set for.</param>
    /// <returns>The computed first set.</returns>
    private FirstSet ComputeFirstSet(ISentence beta)
    {
        return FirstSetCalculator.ComputeFirstSet(beta);
    }

    /// <summary>
    /// Computes the goto states for the specified state.
    /// </summary>
    /// <param name="state">The state to compute the goto states for.</param>
    /// <returns>An array of computed goto states.</returns>
    private LR1State[] ComputeGotoStates(LR1State state)
    {
        var gotoStates = new List<LR1State>();

        var groups = state.Items
            .Where(x => x.Symbol is not null)
            .GroupBy(i => i.Symbol!, new SymbolEqualityComparer())
            .ToArray();

        foreach (var group in groups)
        {
            var nextSymbol = group.Key;
            var items = group.ToArray();

            var shiftedItems = items
                .Select(i => i.CreateNextItem())
                .ToArray();

            var kernel = new LR1Kernel(shiftedItems);
            var closure = ComputeKernelClosure(kernel);

            var nextState = new LR1State(
                kernel: kernel,
                closure: closure);

            if (gotoStates.Any(s => s.Equals(nextState)))
            {
                continue;
            }

            gotoStates.Add(nextState);
        }

        return gotoStates.ToArray();
    }

    /// <summary>
    /// Computes the follow set for the grammar.
    /// </summary>
    /// <returns>The computed follow set.</returns>
    /// <exception cref="NotImplementedException">Thrown when the method is not implemented.</exception>
    private FollowSetCalculator ComputeFollowSet()
    {
        throw new NotImplementedException();
    }
}
