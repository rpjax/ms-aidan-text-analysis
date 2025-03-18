using Aidan.Core.Patterns;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Tokenization.StateMachine.Components;

namespace Aidan.TextAnalysis.Tokenization.StateMachine.Builders;

public class TokenizerBuilder : IBuilder<Tokenizer>
{
    private Dictionary<uint, TokenizerState> States { get; }
    private Dictionary<uint, List<TokenizerTransition>> Transitions { get; }
    private Charset Charset { get; set; }
    private bool UseDebugger { get; set; }

    public TokenizerBuilder(
        IEnumerable<char>? charset = null,
        bool useDebugger = false)
    {
        States = new();
        Transitions = new();
        Charset = new Charset(charset ?? ComputeCharset(CharsetType.Ascii));
    }

    public TokenizerBuilder(
        ITokenizerTable table,
        IEnumerable<char> charset)
    {
        var entries = table.GetEntries();
        var initialState = table.GetInitialState();
        var states = entries
            .Select(x => x.Key)
            .ToArray();

        States = new();
        Transitions = new();
        Charset = new Charset(charset);

        foreach (var state in states)
        {
            CreateState(
                id: state.Id,
                name: state.Name,
                isAccepting: state.IsAccepting);

            var stateTransitions = entries
                .Where(x => x.Key.Id == state.Id)
                .Select(x => x.Value)
                .First()
                .ToList();

            foreach (var transition in stateTransitions)
            {
                AddTransition(
                    sourceStateId: state.Id,
                    character: transition.Character,
                    nextStateId: transition.StateId);
            }
        }
    }

    /* static methods */

    public static Charset ComputeCharset(CharsetType charset)
    {
        return Language.Components.Charset.Compute(charset);
    }

    /* instace methods */

    /* getter methods */

    public Charset GetCharset()
    {
        return Charset;
    }

    public TokenizerState? FindState(uint id)
    {
        if (!States.TryGetValue(id, out var state))
        {
            return null;
        }

        return state;
    }

    public TokenizerState GetState(uint id)
    {
        if (!States.TryGetValue(id, out var state))
        {
            throw new InvalidOperationException($"State with ID {id} does not exist.");
        }

        return state;
    }

    public TokenizerState GetInitialState()
    {
        if (States.Count == 0)
        {
            throw new InvalidOperationException("No initial state defined.");
        }

        return States.First().Value;
    }

    /* setter methods */

    public TokenizerBuilder SetCharset(params char[] chars)
    {
        if (chars.Length == 0)
        {
            throw new ArgumentException("Charset must contain at least one character.");
        }

        Charset = new Charset(chars);
        return this;
    }

    public TokenizerBuilder SetCharset(IEnumerable<char> chars)
    {
        return SetCharset(chars.ToArray());
    }

    public TokenizerBuilder SetCharset(CharsetType charset)
    {
        Charset = ComputeCharset(charset);
        return this;
    }

    public TokenizerBuilder EnableDebugger()
    {
        UseDebugger = true;
        return this;
    }

    public TokenizerState CreateState(
        uint id,
        string name,
        bool isAccepting)
    {
        var state = new TokenizerState(
            id: id,
            name: name,
            isAccepting: isAccepting,
            isRecursiveOnNoTransition: false);

        if (States.ContainsKey(id))
        {
            throw new InvalidOperationException($"A state with ID {id} already exists.");
        }
        if (Transitions.ContainsKey(id))
        {
            throw new InvalidOperationException($"A state with ID {id} already has transitions.");
        }

        States.Add(id, state);
        Transitions.Add(id, new List<TokenizerTransition>());

        return state;
    }

    public TokenizerBuilder AddTransition(
        uint sourceStateId,
        char character,
        uint nextStateId)
    {
        EnsureStateIsListed(sourceStateId);

        var transition = new TokenizerTransition(
            character: character,
            stateId: nextStateId);

        GetStateTransitions(sourceStateId)
            .Add(transition);

        return this;
    }

    public TableTransitionBuilder FromInitialState()
    {
        return new TableTransitionBuilder(
            builder: this,
            currentState: GetInitialState());
    }

    public TableTransitionBuilder FromState(uint id)
    {
        EnsureStateIsListed(id);

        return new TableTransitionBuilder(
            builder: this,
            currentState: GetState(id));
    }

    /* builder methods */

    public TokenizerTable BuildTable()
    {
        var entries = new Dictionary<TokenizerState, TokenizerTransition[]>();

        foreach (var state in States.Values)
        {
            entries.Add(state, GetStateTransitions(state.Id).ToArray());
        }

        return new TokenizerTable(entries);
    }

    public Tokenizer Build()
    {
        return new Tokenizer(
            table: BuildTable(),
            useDebugger: UseDebugger);
    }

    /* private methods */

    private void EnsureStateIsListed(uint id)
    {
        if (!States.ContainsKey(id))
        {
            throw new InvalidOperationException($"State with ID {id} does not exist.");
        }
    }

    private List<TokenizerTransition> GetStateTransitions(uint id)
    {
        EnsureStateIsListed(id);
        return Transitions[id];
    }

}
