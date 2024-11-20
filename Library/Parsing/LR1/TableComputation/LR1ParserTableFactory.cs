using Aidan.Core;
using Aidan.Core.Exceptions;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;
using Aidan.TextAnalysis.Parsing.LR1.Components;
using System;

namespace Aidan.TextAnalysis.Parsing.LR1.TableComputation;

/// <summary>
/// Represents a factory that creates a LR(1) parsing table from a grammar.
/// </summary>
public class LR1ParserTableFactory : IFactory<LR1ParserTable>
{
    /// <summary>
    /// Gets the grammar used to create the LR(1) parsing table.
    /// </summary>
    private IGrammar Grammar { get; }

    /// <summary>
    /// Gets the production rules of the grammar.
    /// </summary>
    private IProductionRule[] Productions { get; }

    /// <summary>
    /// Gets the states of the LR(1) parser.
    /// </summary>
    private LR1State[] States { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LR1ParserTableFactory"/> class with the specified grammar.
    /// </summary>
    /// <param name="grammar">The grammar to use for creating the LR(1) parsing table.</param>
    public LR1ParserTableFactory(IGrammar grammar)
    {
        grammar = grammar
            .ExpandMacros()
            .AugmentStart();

        Grammar = grammar;
        Productions = Grammar.ProductionRules
            .Distinct()
            .ToArray();
        /* state computation has been refactored to LR1StatesCalculator */
        //States = LR1Tool.ComputeStates(Grammar);
        States = LR1StatesCalculator.ComputeStates(Grammar);
    }

    /// <summary>
    /// Creates a LR(1) parsing table from the grammar.
    /// </summary>
    /// <returns>The created LR(1) parsing table.</returns>
    public LR1ParserTable Create()
    {
        var entries = new Dictionary<uint, LR1ParserTransition[]>();

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
                .ToArray();

            entries.Add(id, transitions);
        }

        return new LR1ParserTable(
            stateTransitionsDictionary: entries.ToDictionary(
                kv => kv.Key,
                kv => kv.Value
            ),
            productions: Productions,
            states: States);
    }

    /// <summary>
    /// Gets the ID of the specified state.
    /// </summary>
    /// <param name="state">The state to get the ID for.</param>
    /// <returns>The ID of the state.</returns>
    /// <exception cref="Exception">Thrown when the state is not found.</exception>
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

    /// <summary>
    /// Gets the ID of the state with the specified kernel.
    /// </summary>
    /// <param name="kernel">The kernel to get the state ID for.</param>
    /// <returns>The ID of the state with the specified kernel.</returns>
    /// <exception cref="Exception">Thrown when the state with the specified kernel is not found.</exception>
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

    /// <summary>
    /// Gets the index of the specified production rule.
    /// </summary>
    /// <param name="production">The production rule to get the index for.</param>
    /// <returns>The index of the production rule.</returns>
    /// <exception cref="Exception">Thrown when the production rule is not found.</exception>
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

    /// <summary>
    /// Computes the actions for the specified state.
    /// </summary>
    /// <param name="state">The state to compute the actions for.</param>
    /// <returns>A dictionary of actions for the state.</returns>
    /// <exception cref="Exception">Thrown when there is a conflict with the actions for the state.</exception>
    private Dictionary<ISymbol, LR1Action> ComputeActionsForState(LR1State state)
    {
        Dictionary<ISymbol, LR1Action> actions = new();

        KeyValuePair<ISymbol, LR1Action>[] shiftActions = ComputeShiftActions(state);
        KeyValuePair<ISymbol, LR1Action>[] reduceActions = ComputeReduceActions(state);
        KeyValuePair<ISymbol, LR1Action>[] gotoActions = ComputeGotoActions(state);

        KeyValuePair<ISymbol, LR1Action>[] stateActions = shiftActions
            .Concat(reduceActions)
            .Concat(gotoActions)
            .ToArray();

        Dictionary<ISymbol, LR1Action[]> conflictingActions = stateActions
            .GroupBy(x => x.Key)
            .Select(x => new KeyValuePair<ISymbol, LR1Action[]>(x.Key, x.Select(y => y.Value).ToArray()))
            .Where(kv => kv.Value.Length > 1)
            .ToDictionary(x => x.Key, x => x.Value);

        if (conflictingActions.Count > 0)
        {
            List<Error> errors = new();

            foreach (var item in conflictingActions)
            {
                var actionTypes = item.Value
                    .Select(x => x.Type)
                    .ToArray();

                var typesStr = string.Join("/", actionTypes);
                var e = new Exception();

                var error = Error.Create()
                    .WithTitle($"A {typesStr} conflict was found on state {GetStateId(state)} for the symbol {item.Key}.")
                    .WithDetail("State Items", state.ToString())
                    .Build();

                errors.Add(error);
            }         

            throw new ErrorException(errors);
        }

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

    /// <summary>
    /// Computes the shift actions for the specified state.
    /// </summary>
    /// <param name="state">The state to compute the shift actions for.</param>
    /// <returns>A dictionary of shift actions for the state.</returns>
    /// <exception cref="Exception">Thrown when there is a conflict with the shift actions for the state.</exception>
    private KeyValuePair<ISymbol, LR1Action>[] ComputeShiftActions(LR1State state)
    {
        var actions = new List<KeyValuePair<ISymbol, LR1Action>>();

        var shiftItems = state.Items
            .Where(x => x.Symbol is not null && x.Symbol.IsTerminal())
            .GroupBy(x => x.Symbol)
            .Select(x => new
            {
                Symbol = x.Key!,
                GotoKernel = new LR1Kernel(x.Select(x => x.AdvancePosition()).ToArray())
            })
            .ToArray();

        foreach (var item in shiftItems)
        {
            var symbol = item.Symbol;
            var gotoKernel = item.GotoKernel;
            var nextStateId = GetStateIdByKernel(gotoKernel);

            var keyValuePair = new KeyValuePair<ISymbol, LR1Action>(
                key: symbol, 
                value: new LR1ShiftAction(nextStateId));

            actions.Add(keyValuePair);
        }

        return actions.ToArray();
    }

    /// <summary>
    /// Computes the reduce actions for the specified state.
    /// </summary>
    /// <param name="state">The state to compute the reduce actions for.</param>
    /// <returns>A dictionary of reduce actions for the state.</returns>
    /// <exception cref="Exception">Thrown when there is a conflict with the reduce actions for the state.</exception>
    private KeyValuePair<ISymbol, LR1Action>[] ComputeReduceActions(LR1State state)
    {
        var actions = new List<KeyValuePair<ISymbol, LR1Action>>();

        var stateId = GetStateId(state);
        /* 
         * The state 0 is always the initial state, and the first GOTO state to be computed is always the accepting state.
         * So the accepting state is always the state 1, which is the second state in the states array.
         */
        var isAcceptingState = stateId == 1;

        var reduceItems = state.Items
           .Where(x => x.Symbol is null || x.Symbol.IsEpsilon())
           //.Where(x => x.Symbol is null)
           .ToArray();

        if (isAcceptingState)
        {
            var acceptAction = new KeyValuePair<ISymbol, LR1Action>(
                key: Eoi.Instance, 
                value: new LR1AcceptAction());

            actions.Add(acceptAction);
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

                var reduceAction = new KeyValuePair<ISymbol, LR1Action>(
                    key: lookahead,
                    value: new LR1ReduceAction(productionIndex));

                actions.Add(reduceAction);
            }
        }

        return actions.ToArray();
    }

    /// <summary>
    /// Computes the goto actions for the specified state.
    /// </summary>
    /// <param name="state">The state to compute the goto actions for.</param>
    /// <returns>A dictionary of goto actions for the state.</returns>
    /// <exception cref="Exception">Thrown when there is a conflict with the goto actions for the state.</exception>
    private KeyValuePair<ISymbol, LR1Action>[] ComputeGotoActions(LR1State state)
    {
        var actions = new List<KeyValuePair<ISymbol, LR1Action>>();

        var gotoItems = state.Items
            .Where(x => x.Symbol is not null && x.Symbol.IsNonTerminal())
            .GroupBy(x => x.Symbol)
            .Select(x => new
            {
                Symbol = x.Key!,
                GotoKernel = new LR1Kernel(x.Select(x => x.AdvancePosition()).ToArray())
            })
            .ToArray();

        foreach (var item in gotoItems)
        {
            var symbol = item.Symbol;
            var gotoKernel = item.GotoKernel;
            var nextStateId = GetStateIdByKernel(gotoKernel);

            var gotoAction = new KeyValuePair<ISymbol, LR1Action>(
                key: symbol,
                value: new LR1GotoAction(nextStateId));

            actions.Add(gotoAction);
        }

        return actions.ToArray();
    }

}
