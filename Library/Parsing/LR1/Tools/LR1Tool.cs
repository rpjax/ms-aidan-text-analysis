using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;
using Aidan.TextAnalysis.Parsing.LR1.Components;

namespace Aidan.TextAnalysis.Parsing.LR1.Tools;

/// <summary>
/// Provides a set of tools for working with LR(1) parsers. <br/>
/// It can be used to compute the LR(1) canonical collection of sets of items, and the LR(1) set of states.
/// </summary>
public class LR1Tool
{
    public static LR1State[] ComputeStates(
        ProductionSet set)
    {
        set.EnsureNoMacros();
        set.EnsureAugmented();

        var initialState = ComputeInitialState(set);

        var states = new List<LR1State>
        {
            initialState
        };

        var proccessedStates = new List<LR1State>();

        while (true)
        {
            var counter = 0;

            foreach (var state in states.ToArray())
            {
                if(proccessedStates.Any(x => x.Equals(state)))
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
                    set: set,
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

                proccessedStates.Add(state);
            }

            if (counter == 0)
            {
                break;
            }
        }

        return states.ToArray();
    }

    public static LR1StateCollection ComputeStatesCollection(
        ProductionSet set)
    {
        return new LR1StateCollection(ComputeStates(set));
    }

    private static LR1State ComputeInitialState(
        ProductionSet set)
    {
        var augmentedProduction = set.GetAugmentedStartProduction();

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
            set: set,
            kernel: kernel
        );

        return new LR1State(
            kernel: kernel,
            closure: closure
        );
    }

    private static LR1State[] ComputeNextStates(
        ProductionSet set,
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
                set: set,
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

    /*
     * Closure rewritten to use a stack instead of recursion.
     */
    private static LR1Closure ComputeItemClosure(
        ProductionSet set,
        LR1Item item)
    {
        var items = new List<LR1Item>();

        var symbol = item.Symbol;

        if (symbol is not NonTerminal nonTerminal)
        {
            return new LR1Closure(items.ToArray());
        }

        var productions = set.Lookup(nonTerminal)
            .ToArray();

        if (productions.Length == 0)
        {
            throw new InvalidOperationException($"The non-terminal '{nonTerminal}' does not have any productions.");
        }

        foreach (var production in productions)
        {
            var lookaheads = ComputeLookaheads(
                set: set,
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

    private static LR1Closure ComputeKernelClosure(
        ProductionSet set,
        LR1Kernel kernel)
    {
        var items = new List<LR1Item>(kernel);

        while (true)
        {
            var counter = 0;

            foreach (var item in items.ToArray())
            {
                var closure = ComputeItemClosure(
                    set: set,
                    item: item
                );

                foreach (var newItem in closure)
                {
                    if (items.Any(x => x == newItem))
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
            .GroupBy(item => item.GetSignature(useLookaheads: false))
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

    private static Terminal[] ComputeLookaheads(
        ProductionSet set,
        Sentence beta,
        Terminal[] originalLookaheads)
    {
        var stack = new Stack<Sentence>();
        var lookaheads = new List<Terminal>();

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

            var productions = set.Lookup(nonTerminal).ToArray();
            var producesEpsilon = productions.Any(x => x.IsEpsilonProduction);

            // S -> .A B c (beta is B c)
            // B -> b
            // B -> .epsilon

            foreach (var production in productions)
            {
                if (production.IsEpsilonProduction)
                {
                    stack.Push(sentence.Skip(1).ToArray());
                    continue;
                }

                var newSentence = new Sentence(
                    production.Body.Concat(sentence.Skip(1)).ToArray()
                );

                if (newSentence.Length == 0)
                {
                    Console.WriteLine();
                }

                stack.Push(newSentence);
            }
        }

        return lookaheads
            .Distinct()
            .ToArray();
    }

    private static Dictionary<Symbol, LR1Kernel> ComputeGotoDictionary(
        LR1State state,
        LR1Kernel[] kernelBlacklist)
    {
        var symbolGroups = state.Items
            .Where(item => item.Symbol is not null)
            .Where(item => item.Symbol is not Epsilon)
            .GroupBy(item => item.Symbol!)
            .ToArray();

        var dictionary = new Dictionary<Symbol, LR1Kernel>();

        foreach (var entry in symbolGroups)
        {
            var symbol = entry.Key;

            var kernelItems = entry
                .Select(item => item.CreateNextItem())
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
