using Aidan.TextAnalysis.Tokenization.StateMachine.Components;

namespace Aidan.TextAnalysis.Tokenization.StateMachine.Builders;

public class ManualTokenizerBuilder
{
    private List<TokenizerState> States { get; }
    private Dictionary<string, TokenizerState> NameStateMap { get; }
    private Dictionary<TokenizerState, List<TokenizerTransition>> Transitions { get; }
    private char[] Charset { get; set; }
    private bool UseDebugger { get; set; }

    public ManualTokenizerBuilder(
        IEnumerable<char>? charset = null,
        bool useDebugger = false)
    {
        States = new();
        NameStateMap = new();
        Transitions = new();
        Charset = charset?.ToArray() ?? TokenizerBuilder.ComputeCharset(CharsetType.Ascii);
        UseDebugger = useDebugger;
    }

    public ManualTokenizerBuilder(
        TokenizerTable table,
        IEnumerable<char>? charset = null,
        bool useDebugger = false)
    {
        States = new();
        NameStateMap = new();
        Transitions = new();
        Charset = charset?.ToArray() ?? TokenizerBuilder.ComputeCharset(CharsetType.Ascii);
        UseDebugger = useDebugger;

        var entries = table.GetEntries();
        var initialState = table.GetInitialState();

        foreach (var entry in entries)
        {
            var state = entry.Key;
            var transitions = entry.Value;

            if (state == initialState)
            {
                NameStateMap.Add(state.Name, state);
            }

            States.Add(state);
            Transitions.Add(state, new List<TokenizerTransition>());

            foreach (var transition in transitions)
            {
                Transitions[state].Add(transition);
            }
        }
    }

    /* getter methods */

    public char[] GetCharset()
    {
        return Charset;
    }

    public TokenizerState? FindState(string name)
    {
        if (!NameStateMap.TryGetValue(name, out var state))
        {
            return null;
        }

        return state;
    }

    public TokenizerState GetState(string name)
    {
        if (!NameStateMap.TryGetValue(name, out var state))
        {
            throw new InvalidOperationException($"State {name} does not exist.");
        }

        return state;
    }

    public TokenizerState GetInitialState()
    {
        if (States.Count == 0)
        {
            throw new InvalidOperationException("No initial state defined.");
        }

        return States.First();
    }

    /* setter methods */

    public ManualTokenizerBuilder SetCharset(params char[] chars)
    {
        if (chars.Length == 0)
        {
            throw new ArgumentException("Charset must contain at least one character.");
        }

        Charset = chars;
        return this;
    }

    public ManualTokenizerBuilder SetCharset(IEnumerable<char> chars)
    {
        return SetCharset(chars.ToArray());
    }

    public ManualTokenizerBuilder SetCharset(CharsetType charset)
    {
        Charset = TokenizerBuilder.ComputeCharset(charset);
        return this;
    }

    public ManualTokenizerBuilder EnableDebugger()
    {
        UseDebugger = true;
        return this;
    }

    public TokenizerState CreateState(
        string name,
        bool isAccepting)
    {
        var state = new TokenizerState(
            id: GetNextStateId(),
            name: name,
            isAccepting: isAccepting,
            isRecursiveOnNoTransition: false);

        if (NameStateMap.ContainsKey(name))
        {
            throw new InvalidOperationException($"State '{name}' already exists.");
        }
        if (Transitions.ContainsKey(state))
        {
            throw new InvalidOperationException($"State '{name}' already has transitions.");
        }

        States.Add(state);
        NameStateMap.Add(name, state);
        Transitions.Add(state, new List<TokenizerTransition>());
        return state;
    }

    public ManualTokenizerBuilder AddTransition(
        string currentState,
        char character,
        string nextState)
    {
        EnsureStateIsListed(currentState);
        EnsureStateIsListed(nextState);

        var transition = new TokenizerTransition(
            character: character,
            stateId: GetState(nextState).Id);

        GetStateTransitions(GetState(currentState)).Add(transition);
        return this;
    }

    public ManualTableTransitionBuilder FromInitialState()
    {
        return new ManualTableTransitionBuilder(
            builder: this,
            currentState: GetInitialState());
    }

    public ManualTableTransitionBuilder FromState(string name)
    {
        EnsureStateIsListed(name);

        return new ManualTableTransitionBuilder(
            builder: this,
            currentState: GetState(name));
    }

    /* builder methods */

    public TokenizerTable BuildTable()
    {
        var entries = new Dictionary<TokenizerState, TokenizerTransition[]>();

        foreach (var state in States)
        {
            var transitions = Transitions.ContainsKey(state)
                ? Transitions[state].ToArray()
                : Array.Empty<TokenizerTransition>()
                ;

            entries.Add(state, transitions);
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

    private uint GetNextStateId()
    {
        return (uint)States.Count;
    }

    private void EnsureStateIsListed(string name)
    {
        if (!NameStateMap.ContainsKey(name))
        {
            throw new InvalidOperationException($"State '{name}' does not exist.");
        }
    }

    private List<TokenizerTransition> GetStateTransitions(TokenizerState state)
    {
        if (!Transitions.TryGetValue(state, out var transitions))
        {
            throw new InvalidOperationException($"State '{state.Name}' does not have transitions.");
        }

        return transitions;
    }

}
