using Aidan.TextAnalysis.Tokenization.StateMachine.Components;

namespace Aidan.TextAnalysis.Tokenization.StateMachine.Builders;

public class ManualTokenizerBuilder
{
    private Dictionary<string, TokenizerState> States { get; }
    private Dictionary<string, List<TokenizerTransition>> Transitions { get; }
    private char[] Charset { get; set; }
    private bool UseDebugger { get; set; }

    public ManualTokenizerBuilder(
        IEnumerable<char>? charset = null,
        bool useDebugger = false)
    {
        States = new();
        Transitions = new();
        Charset = charset?.ToArray() ?? TokenizerBuilder.ComputeCharset(CharsetType.Ascii);
        UseDebugger = useDebugger;
    }

    /* getter methods */

    public char[] GetCharset()
    {
        return Charset;
    }

    public TokenizerState? FindState(string name)
    {
        if (!States.TryGetValue(name, out var state))
        {
            return null;
        }

        return state;
    }

    public TokenizerState GetState(string name)
    {
        if (!States.TryGetValue(name, out var state))
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

        return States.First().Value;
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

        if (States.ContainsKey(name))
        {
            throw new InvalidOperationException($"State '{name}' already exists.");
        }
        if (Transitions.ContainsKey(name))
        {
            throw new InvalidOperationException($"State '{name}' already has transitions.");
        }

        States.Add(name, state);
        Transitions.Add(name, new List<TokenizerTransition>());
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

        GetStateTransitions(currentState).Add(transition);
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

        foreach (var state in States.Values)
        {
            entries.Add(state, GetStateTransitions(state.Name).ToArray());
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
        if (!States.ContainsKey(name))
        {
            throw new InvalidOperationException($"State '{name}' does not exist.");
        }
    }

    private List<TokenizerTransition> GetStateTransitions(string name)
    {
        EnsureStateIsListed(name);
        return Transitions[name];
    }

}
