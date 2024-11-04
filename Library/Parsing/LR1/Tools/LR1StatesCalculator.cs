using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;
using Aidan.TextAnalysis.Parsing.LR1.Components;

namespace Aidan.TextAnalysis.Parsing.LR1.Tools;

public class LR1StatesCalculator
{
    private IGrammar Grammar { get; }

    public LR1StatesCalculator(IGrammar grammar)
    {
        Grammar = grammar;
    }

    public LR1State[] ComputeStates()
    {
        var initialState = ComputeInitialState();

        throw new NotImplementedException();
    }

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

    private LR1Closure ComputeKernelClosure(LR1Kernel kernel)
    {
        var items = new List<LR1Item>(kernel);

        while (true)
        {
            var counter = 0;

            foreach (var item in items)
            {
                var itemClosure = ComputeItemClosure(item);

                if (itemClosure.Length == 0)
                {
                    continue;
                }

                foreach (var newItem in itemClosure)
                {
                    if (items.Contains(newItem))
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

        throw new NotImplementedException();
    }

    private FirstSet ComputeFirstSet()
    {
        throw new NotImplementedException();
    }

    private FollowSet ComputeFollowSet()
    {
        throw new NotImplementedException();
    }
}

/* 
 * auxiliary classes 
 */

public class FirstSet
{

}

public class FollowSet
{

}
