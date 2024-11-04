using Aidan.Core;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;
using Aidan.TextAnalysis.Parsing.LR1.Tools;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents a factory that creates a LR(1) parsing table from a grammar.
/// </summary>
public class LR1ParserTableFactory : IFactory<LR1ParserTable>
{
    private IGrammar Grammar { get; }
    private IProductionRule[] Productions { get; }
    private LR1State[] States { get; }

    public LR1ParserTableFactory(IGrammar grammar)
    {
        grammar = grammar.ExpandMacros();

        Grammar = grammar;
        Productions = Grammar.ProductionRules
            .Distinct()
            .ToArray();
        States = LR1Tool.ComputeStates(Grammar);
    }

    /// <summary>
    /// Creates a LR(1) parsing table from a grammar. 
    /// </summary>
    /// <returns></returns>
    public LR1ParserTable Create()
    {
        var entries = new Dictionary<uint, List<LR1ParserTransition>>();

        foreach (var state in States)
        {
            var id = GetStateId(state);
            var actions = ComputeActionsForState(
                state: state
            );

            var transitions = actions
                .Select(kv => new LR1ParserTransition(
                    stateId: id,
                    symbol: kv.Key,
                    action: kv.Value
                ))
                .ToList();

            entries.Add(id, transitions);
        }

        return new LR1ParserTable(
            stateTransitionsDictionary: entries.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.ToArray()
            ),
            productions: Productions);
    }

    private uint GetStateId(LR1State state)
    {
        for (uint i = 0; i < States.Length; i++)
        {
            if (States[i].Equals(state))
            {
                return i;
            }
        }

        throw new Exception($"State {state} not found.");
    }

    private uint GetStateIdByKernel(LR1Kernel kernel)
    {
        for (uint i = 0; i < States.Length; i++)
        {
            if (States[i].Kernel.Equals(kernel))
            {
                return i;
            }
        }

        throw new Exception($"State with kernel {kernel} not found.");
    }

    private uint GetProductionIndex(IProductionRule production)
    {
        for (uint i = 0; i < Productions.Length; i++)
        {
            if (Productions[i].Equals(production))
            {
                return i;
            }
        }

        throw new Exception($"Production {production} not found.");
    }

    private Dictionary<ISymbol, LR1Action> ComputeActionsForState(LR1State state)
    {
        var actions = new Dictionary<ISymbol, LR1Action>();

        var shiftActions = ComputeShiftActions(state);
        var reduceActions = ComputeReduceActions(state);
        var gotoActions = ComputeGotoActions(state);

        var stateActions = shiftActions
            .Concat(reduceActions)
            .Concat(gotoActions)
            .ToArray();

        foreach (var action in stateActions)
        {
            if (actions.ContainsKey(action.Key))
            {
                throw new Exception($"Conflict at state {GetStateId(state)} with symbol {action.Key}.");
            }

            actions.Add(action.Key, action.Value);
        }

        return actions;
    }

    private Dictionary<ISymbol, LR1Action> ComputeShiftActions(LR1State state)
    {
        var actions = new Dictionary<ISymbol, LR1Action>();

        var shiftItems = state.Items
            .Where(x => x.Symbol is not null && x.Symbol.IsTerminal() && !x.Symbol.IsEpsilon())
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
            var nextStateId = GetStateIdByKernel(gotoKernel);

            if (actions.ContainsKey(symbol))
            {
                throw new Exception($"Conflict at state {state} with symbol {symbol}.");
            }

            actions.Add(symbol, new LR1ShiftAction(nextStateId));
        }

        return actions;
    }

    private Dictionary<ISymbol, LR1Action> ComputeReduceActions(LR1State state)
    {
        var actions = new Dictionary<ISymbol, LR1Action>();
        var isAcceptingState = state.IsAcceptingState(Grammar);

        var reduceItems = state.Items
            .Where(x => x.Symbol is null || x.Symbol.IsEpsilon())
            .ToArray();

        if (isAcceptingState)
        {
            actions.Add(Eoi.Instance, new LR1AcceptAction());
        }

        foreach (var item in reduceItems)
        {
            var productionIndex = GetProductionIndex(item.Production);

            foreach (var lookahead in item.Lookaheads)
            {
                if (lookahead.IsEoi() && isAcceptingState)
                {
                    continue;
                }

                actions.Add(lookahead, new LR1ReduceAction(productionIndex));
            }
        }

        return actions;
    }

    private Dictionary<ISymbol, LR1Action> ComputeGotoActions(LR1State state)
    {
        var actions = new Dictionary<ISymbol, LR1Action>();

        var gotoItems = state.Items
            .Where(x => x.Symbol is not null && x.Symbol.IsNonTerminal())
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
            var nextStateId = GetStateIdByKernel(gotoKernel);

            if (actions.ContainsKey(symbol))
            {
                throw new Exception($"Conflict at state {state} with symbol {symbol}.");
            }

            actions.Add(symbol, new LR1GotoAction(nextStateId));
        }

        return actions;
    }

}
