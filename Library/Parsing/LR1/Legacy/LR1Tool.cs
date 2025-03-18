using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Components.Symbols;
using Aidan.TextAnalysis.Language.Extensions;
using Aidan.TextAnalysis.Parsing.LR1.Components;

namespace Aidan.TextAnalysis.Parsing.LR1.Legacy;

/// <summary>
/// Provides a set of tools for working with LR(1) parsers. <br/>
/// It can be used to compute the LR(1) canonical collection of sets of items, and the LR(1) set of states.
/// </summary>
public class LR1Tool
{
    /// <summary>
    /// Computes the LR(1) states for a given production set.
    /// </summary>
    /// <param name="grammar">The production set to compute states for.</param>
    /// <returns>An array of LR(1) states.</returns>
    public static LR1State[] ComputeStates(IGrammar grammar)
    {
        grammar.EnsureNoMacros();

        var initialState = ComputeInitialState(grammar);

        var states = new List<LR1State>
        {
            initialState
        };

        var processedStates = new List<LR1State>();

        while (true)
        {
            var counter = 0;

            foreach (var state in states.ToArray())
            {
                if (processedStates.Any(x => x.Equals(state)))
                {
                    continue;
                }

                /*
                 * The kernel blacklist is used to prevent the same kernel from being computed twice.
                 * It exists for performance reasons only, so if left out the algorithm will still work, but slower.
                 */
                var kernelBlacklist = states
                    .Select(x => x.Kernel)
                    .ToArray();

                var nextStates = ComputeNextStates(
                    grammar: grammar,
                    state: state,
                    kernelBlacklist: kernelBlacklist
                );

                foreach (var nextState in nextStates)
                {
                    if (states.Any(x => x.Equals(nextState)))
                    {
                        continue;
                    }

                    states.Add(nextState);
                    counter++;
                }

                processedStates.Add(state);
            }

            if (counter == 0)
            {
                break;
            }
        }

        return states.ToArray();
    }

    /// <summary>
    /// Computes the LR(1) state collection for a given production set.
    /// </summary>
    /// <param name="grammar">The production set to compute the state collection for.</param>
    /// <returns>An LR(1) state collection.</returns>
    public static LR1StateCollection ComputeStatesCollection(IGrammar grammar)
    {
        return new LR1StateCollection(ComputeStates(grammar));
    }

    /// <summary>
    /// Computes the initial state for a given production set.
    /// </summary>
    /// <param name="grammar">The production set to compute the initial state for.</param>
    /// <returns>The initial LR(1) state.</returns>
    private static LR1State ComputeInitialState(IGrammar grammar)
    {
        var startSymbol = grammar.StartSymbol;
        var startSymbolProductions = grammar.GetProductions(startSymbol);

        if (startSymbolProductions.Length == 0)
        {
            throw new InvalidOperationException($"The start symbol '{startSymbol}' does not have any productions.");
        }

        if (startSymbolProductions.Length > 1)
        {
            throw new InvalidOperationException($"The start symbol '{startSymbol}' has more than one production.");
        }

        var augmentedProduction = startSymbolProductions.First();

        var initialItemProduction = new ProductionRule(
            head: augmentedProduction.Head,
            body: augmentedProduction.Body[0]
        );

        var kernelItem = new LR1Item(
            production: initialItemProduction,
            position: 0,
            lookaheads: Eoi.Instance
        );

        var kernel = new LR1Kernel(kernelItem);

        var closure = ComputeKernelClosure(
            grammar: grammar,
            kernel: kernel
        );

        return new LR1State(
            kernel: kernel,
            closure: closure
        );
    }

    /// <summary>
    /// Computes the next states for a given state and production set.
    /// </summary>
    /// <param name="set">The production set to compute the next states for.</param>
    /// <param name="state">The current state.</param>
    /// <param name="kernelBlacklist">The kernel blacklist to prevent duplicate computations.</param>
    /// <returns>An array of the next LR(1) states.</returns>
    private static LR1State[] ComputeNextStates(
        IGrammar grammar,
        LR1State state,
        LR1Kernel[] kernelBlacklist)
    {
        var states = new List<LR1State>();

        var gotosDictionary = ComputeGotoDictionary(
            state: state,
            kernelBlacklist: kernelBlacklist
        );

        foreach (var entry in gotosDictionary)
        {
            var nextStateKernel = entry.Value;

            var nextStateClosure = ComputeKernelClosure(
                grammar: grammar,
                kernel: nextStateKernel
            );

            var nextState = new LR1State(
                kernel: nextStateKernel,
                closure: nextStateClosure
            );

            states.Add(nextState);
        }

        return states.ToArray();
    }

    /// <summary>
    /// Computes the closure of an LR(1) item.
    /// </summary>
    /// <param name="grammar">The production set to compute the closure for.</param>
    /// <param name="item">The LR(1) item to compute the closure for.</param>
    /// <returns>The closure of the LR(1) item.</returns>
    private static LR1Closure ComputeItemClosure(
        IGrammar grammar,
        LR1Item item)
    {
        var items = new List<LR1Item>();

        var symbol = item.Symbol;

        if (symbol is not NonTerminal nonTerminal)
        {
            return new LR1Closure(items.ToArray());
        }

        var productions = grammar.GetProductions(nonTerminal);

        if (productions.Length == 0)
        {
            throw new InvalidOperationException($"The non-terminal '{nonTerminal}' does not have any productions.");
        }

        foreach (var production in productions)
        {
            var lookaheads = ComputeLookaheads(
                grammar: grammar,
                beta: item.GetBeta(),
                originalLookaheads: item.Lookaheads
            );

            var newItem = new LR1Item(
                production: production,
                position: 0,
                lookaheads: lookaheads
            );

            items.Add(newItem);
        }

        return new LR1Closure(items.ToArray());
    }

    /// <summary>
    /// Computes the closure of an LR(1) kernel.
    /// </summary>
    /// <param name="grammar">The production set to compute the closure for.</param>
    /// <param name="kernel">The LR(1) kernel to compute the closure for.</param>
    /// <returns>The closure of the LR(1) kernel.</returns>
    private static LR1Closure ComputeKernelClosure(
        IGrammar grammar,
        LR1Kernel kernel)
    {
        var items = new List<LR1Item>(kernel);

        while (true)
        {
            var counter = 0;

            foreach (var item in items.ToArray())
            {
                var closure = ComputeItemClosure(
                    grammar: grammar,
                    item: item
                );

                foreach (var newItem in closure)
                {
                    if (items.Any(x => x.Equals(newItem)))
                    {
                        continue;
                    }

                    items.Add(newItem);
                    counter++;
                }
            }

            if (counter == 0)
            {
                break;
            }
        }

        /*
         * This section is commented out because i'm currently figuring out if the code bellow is correct for LR(1). 
         * It combines kernels, withing the same closure, with identical productions and dot positions but different lookaheads. 
         * Ex: (A -> .a b, {c}) and (A -> .a b, {d}) into (A -> .a b, {c, d}).
         * 
         * NOTE: for now i'll uncomment it because it seems to be working.
         */

        var groups = items
            .Skip(kernel.Length)
            .GroupBy(item => item.ComputeSignature(useLookaheads: false))
            .ToArray();

        var uniqueItems = new List<LR1Item>();

        foreach (var group in groups)
        {
            var production = group.First().Production;
            var position = group.First().Position;
            var lookaheads = group
                .SelectMany(item => item.Lookaheads)
                .Distinct()
                .ToArray();

            var uniqueItem = new LR1Item(
                production: production,
                position: position,
                lookaheads: lookaheads
            );

            uniqueItems.Add(uniqueItem);
        }

        return new LR1Closure(uniqueItems.ToArray());
    }

    /// <summary>
    /// Computes the lookaheads for a given beta sentence and original lookaheads.
    /// </summary>
    /// <param name="grammar">The production set to compute the lookaheads for.</param>
    /// <param name="beta">The beta sentence.</param>
    /// <param name="originalLookaheads">The original lookaheads.</param>
    /// <returns>An array of computed lookaheads.</returns>
    private static ITerminal[] ComputeLookaheads(
        IGrammar grammar,
        ISentence beta,
        ITerminal[] originalLookaheads)
    {
        var stack = new Stack<ISentence>();
        var lookaheads = new List<ITerminal>();

        stack.Push(beta);

        while (true)
        {
            if (stack.Count == 0)
            {
                break;
            }

            var sentence = stack.Pop();

            if (sentence.Length == 0)
            {
                lookaheads.AddRange(originalLookaheads);
                continue;
            }

            var symbol = sentence[0];

            if (symbol is Terminal terminal)
            {
                lookaheads.Add(terminal);
                continue;
            }

            if (symbol is Epsilon)
            {
                throw new NotImplementedException();
            }

            if (symbol is not NonTerminal nonTerminal)
            {
                throw new InvalidOperationException("The symbol is not a nonterminal.");
            }

            var productions = grammar.GetProductions(nonTerminal);
            var producesEpsilon = productions.Any(x => x.IsEpsilonProduction());

            // S -> .A B c (beta is B c)
            // B -> b
            // B -> .epsilon

            foreach (var production in productions)
            {
                if (production.IsEpsilonProduction())
                {
                    stack.Push(new Sentence(sentence.Skip(1).ToArray()));
                    continue;
                }

                var newSentence = new Sentence(
                    production.Body.Concat(sentence.Skip(1)).ToArray()
                );

                if (newSentence.Length == 0)
                {
                    // i don't remember why i added this, but it seems to be working.
                    //Console.WriteLine();
                }

                stack.Push(newSentence);
            }
        }

        return lookaheads
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Computes the GOTO dictionary for a given state and kernel blacklist.
    /// </summary>
    /// <param name="state">The current state.</param>
    /// <param name="kernelBlacklist">The kernel blacklist to prevent duplicate computations.</param>
    /// <returns>A dictionary mapping symbols to LR(1) kernels.</returns>
    private static Dictionary<ISymbol, LR1Kernel> ComputeGotoDictionary(
        LR1State state,
        LR1Kernel[] kernelBlacklist)
    {
        var symbolGroups = state.Items
            .Where(item => item.Symbol is not null)
            .Where(item => item.Symbol?.Type != SymbolType.Epsilon)
            .GroupBy(item => item.Symbol!)
            .ToArray();

        var dictionary = new Dictionary<ISymbol, LR1Kernel>();

        foreach (var entry in symbolGroups)
        {
            var symbol = entry.Key;

            var kernelItems = entry
                .Select(item => item.AdvancePosition())
                .ToArray();

            var kernel = new LR1Kernel(kernelItems);

            if (kernelBlacklist.Any(x => x.Equals(kernel)))
            {
                continue;
            }

            dictionary.Add(symbol, kernel);
        }

        return dictionary;
    }

}
