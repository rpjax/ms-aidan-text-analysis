using Aidan.Core;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;
using Aidan.TextAnalysis.Parsing.LR1.Components;

namespace Aidan.TextAnalysis.Parsing.LR1.Tools;

/// <summary>
/// Represents a factory that creates a LR(1) parsing table from a grammar.
/// </summary>
public class LR1ParsingTableFactory : IFactory<Grammar, LR1ParsingTable>
{
    /// <summary>
    /// Creates a LR(1) parsing table from a grammar. 
    /// </summary>
    /// <param name="grammar"></param>
    /// <returns></returns>
    public LR1ParsingTable Create(Grammar grammar)
    {
        var states = LR1Tool.ComputeStatesCollection(grammar.Productions);
        var entries = new List<LR1ParsingTableEntry>();

        var productions = states
            .SelectMany(s => s.Items)
            .Select(i => i.Production)
            .Distinct()
            ;

        var set = new ProductionSet(
            start: grammar.Start,
            productions: productions
        );

        foreach (var state in states)
        {
            var id = states.GetStateId(state);

            var actions = ComputeActionsForState(
                set: set,
                state: state,
                computedStates: states
            );

            var newEntry = new LR1ParsingTableEntry(
                id: id,
                state: state,
                actionsTable: actions
            );

            entries.Add(newEntry);
        }

        return new LR1ParsingTable(
            entries: entries.ToArray(),
            productions: set.ToArray()
        );
    }

    private Dictionary<Symbol, LR1Action> ComputeActionsForState(
        ProductionSet set,
        LR1State state,
        LR1StateCollection computedStates)
    {
        var actions = new Dictionary<Symbol, LR1Action>();

        var shiftActions = ComputeShiftActions(set, state, computedStates);
        var reduceActions = ComputeReduceActions(set, state, computedStates);
        var gotoActions = ComputeGotoActions(set, state, computedStates);

        var stateActions = shiftActions
            .Concat(reduceActions)
            .Concat(gotoActions)
            .ToArray();

        foreach (var action in stateActions)
        {
            if (actions.ContainsKey(action.Key))
            {
                throw new Exception($"Conflict at state {computedStates.GetStateId(state)} with symbol {action.Key}.");
            }

            actions.Add(action.Key, action.Value);
        }

        return actions;
    }

    /*
     * New API
     */

    private Dictionary<Symbol, LR1Action> ComputeShiftActions(
        ProductionSet set,
        LR1State state,
        LR1StateCollection computedStates)
    {
        var actions = new Dictionary<Symbol, LR1Action>();

        var shiftItems = state.Items
            .Where(x => x.Symbol is not null && x.Symbol.IsTerminal && !x.Symbol.IsEpsilon)
            .GroupBy(x => x.Symbol)
            .Select(x => new 
            { 
                Symbol = x.Key!, 
                GotoKernel = new LR1Kernel(x.Select(x => x.CreateNextItem()).ToArray()) 
            })
            .ToArray();

        foreach (var item in shiftItems)
        {
            var symbol = item.Symbol;
            var gotoKernel = item.GotoKernel;
            var nextStateId = computedStates.GetStateIdByKernel(gotoKernel);

            if (nextStateId == -1)
            {
                throw new InvalidOperationException("The next state does not exist.");
            }

            if (actions.ContainsKey(symbol))
            {
                throw new Exception($"Conflict at state {state} with symbol {symbol}.");
            }

            actions.Add(symbol, new LR1ShiftAction(nextStateId));
        }

        return actions;
    }

    private Dictionary<Symbol, LR1Action> ComputeReduceActions(
        ProductionSet set,
        LR1State state,
        LR1StateCollection computedStates)
    {
        var actions = new Dictionary<Symbol, LR1Action>();
        var isAcceptingState = state.IsAcceptingState(set);

        var reduceItems = state.Items
            .Where(x => x.Symbol is null || x.Symbol.IsEpsilon)
            .ToArray();

        if (isAcceptingState)
        {
            actions.Add(Eoi.Instance, new LR1AcceptAction());
        }

        foreach (var item in reduceItems)
        {
            var productionIndex = set.GetProductionIndex(item.Production);

            if (productionIndex == -1)
            {
                throw new InvalidOperationException("The production does not exist in the production set.");
            }

            foreach (var lookahead in item.Lookaheads)
            {
                if (lookahead == Eoi.Instance && isAcceptingState)
                {
                    continue;
                }

                actions.Add(lookahead, new LR1ReduceAction(productionIndex));
            }
        }

        return actions;
    }

    private Dictionary<Symbol, LR1Action> ComputeGotoActions(
        ProductionSet set,
        LR1State state,
        LR1StateCollection computedStates)
    {
        var actions = new Dictionary<Symbol, LR1Action>();

        var gotoItems = state.Items
            .Where(x => x.Symbol is not null && x.Symbol.IsNonTerminal)
            .GroupBy(x => x.Symbol)
            .Select(x => new
            {
                Symbol = x.Key!,
                GotoKernel = new LR1Kernel(x.Select(x => x.CreateNextItem()).ToArray())
            })
            .ToArray();

        foreach (var item in gotoItems)
        {
            var symbol = item.Symbol;
            var gotoKernel = item.GotoKernel;
            var nextStateId = computedStates.GetStateIdByKernel(gotoKernel);

            if (nextStateId == -1)
            {
                throw new InvalidOperationException("The next state does not exist.");
            }

            if (actions.ContainsKey(symbol))
            {
                throw new Exception($"Conflict at state {state} with symbol {symbol}.");
            }

            actions.Add(symbol, new LR1GotoAction(nextStateId));
        }

        return actions;
    }

}