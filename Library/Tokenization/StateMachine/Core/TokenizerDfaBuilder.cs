using Aidan.Core.Patterns;

namespace Aidan.TextAnalysis.Tokenization.StateMachine;

public enum CharsetType
{
    Ascii,
    Unicode,
    Utf8,
    Utf16,
}

public enum CharType
{
    Digit,
    Letter,
    Whitespace,
    Punctuation,
    Control
}

/// <summary>
/// A builder class for constructing a <see cref="TokenizerTable"/>.
/// </summary>
public class TokenizerDfaBuilder : IBuilder<TokenizerMachine>
{
    private Dictionary<string, State> States { get; }

    private Dictionary<string, List<Transition>> Transitions { get; }

    private string InitialStateName { get; set; }

    private char[] Charset { get; set; }

    private bool UseDebugger { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenizerDfaBuilder"/> class.
    /// </summary>
    public TokenizerDfaBuilder(string initialStateName = "0")
    {
        States = new Dictionary<string, State>();
        Transitions = new Dictionary<string, List<Transition>>();
        Charset = ComputeCharset(CharsetType.Ascii);
        InitialStateName = initialStateName;

        /* adds the initial state */
        var initialState = new State(
            id: 0,
            name: InitialStateName,
            isAccepting: false,
            isRecursiveOnNoTransition: false);

        States.Add(initialState.Name, initialState);
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="TokenizerDfaBuilder"/> class using an existing <see cref="TokenizerTable"/>.
    /// </summary>
    /// <param name="table">The tokenizer table to initialize the builder with.</param>
    public TokenizerDfaBuilder(ITokenizerTable table)
    {
        var entries = table.GetEntries();
        var initialState = table.GetInitialState();
        var states = entries
            .Select(x => x.Key)
            .ToArray();

        States = new Dictionary<string, State>();
        Transitions = new Dictionary<string, List<Transition>>();
        Charset = ComputeCharset(CharsetType.Ascii);
        InitialStateName = initialState.Name;

        foreach (var state in states)
        {
            States.Add(state.Name, state);

            var stateTransitions = entries
                .Where(x => x.Key.Id == state.Id)
                .Select(x => x.Value)
                .First()
                .ToList();

            Transitions.Add(state.Name, stateTransitions);
        }
    }

    public static char[] ComputeCharset(CharsetType charset)
    {
        return charset switch
        {
            CharsetType.Ascii => Enumerable.Range(0, 128)
                .Select(x => (char)x)
                .ToArray(),

            CharsetType.Unicode => Enumerable.Range(0, 0xFFFF + 1) // Generates BMP (Basic Multilingual Plane) characters only
                .Select(x => (char)x)
                .ToArray(),

            CharsetType.Utf8 => Enumerable.Range(0, 0xFFFF + 1) // BMP characters, as UTF-8 is variable-length but this is limited by char's 16-bit size
                .Select(x => (char)x)
                .ToArray(),

            CharsetType.Utf16 => Enumerable.Range(0, 0xFFFF + 1) // BMP characters in UTF-16 representation
                .Select(x => (char)x)
                .ToArray(),

            _ => throw new ArgumentOutOfRangeException(nameof(charset))
        };
    }

    public char[] GetCharset()
    {
        return Charset;
    }

    public TokenizerDfaBuilder SetCharset(params char[] chars)
    {
        if (chars.Length == 0)
        {
            throw new ArgumentException("Charset must contain at least one character.");
        }

        Charset = chars;
        return this;
    }

    public TokenizerDfaBuilder SetCharset(CharsetType charset)
    {
        Charset = ComputeCharset(charset);
        return this;
    }

    public TokenizerDfaBuilder EnableDebugger()
    {
        UseDebugger = true;
        return this;
    }

    /// <summary>
    /// Gets the initial state of the tokenizer.
    /// </summary>
    /// <returns>The initial state.</returns>
    public State GetInitialState()
    {
        if (States.Count == 0)
        {
            throw new InvalidOperationException("No initial state defined.");
        }

        return States.Values.First();
    }

    /// <summary>
    /// Creates a new state with the specified name and acceptance status.
    /// </summary>
    /// <param name="name">The name of the state.</param>
    /// <param name="isAcceptingState">Whether the state is an accepting state.</param>
    /// <returns>The created state.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a state with the same name but different acceptance status already exists.</exception>
    public State CreateState(string name, bool isAcceptingState)
    {
        if (States.TryGetValue(name, out var state))
        {
            if (state.IsAccepting != isAcceptingState)
            {
                throw new InvalidOperationException($"State {name} already exists with different accepting state.");
            }

            return state;
        }

        state = new State(
            id: States.Count,
            name: name,
            isAccepting: isAcceptingState,
            isRecursiveOnNoTransition: false);

        States.Add(name, state);

        return state;
    }

    /// <summary>
    /// Adds a transition from the current state to the next state on the specified character.
    /// </summary>
    /// <param name="currentState">The current state.</param>
    /// <param name="character">The character triggering the transition.</param>
    /// <param name="nextState">The next state.</param>
    /// <param name="enableOverride">Whether to enable overriding existing transitions.</param>
    /// <returns>The current <see cref="TokenizerDfaBuilder"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the current or next state does not exist.</exception>
    public TokenizerDfaBuilder AddTransition(
        State currentState,
        char character,
        State nextState,
        bool enableOverride = false)
    {
        EnsureStateIsListed(currentState);
        EnsureStateIsListed(nextState);

        var transitions = GetTransitions(currentState);

        var transition = new Transition(
            character: character,
            stateId: nextState.Id);

        var alreadyExists = transitions.Any(t => t.Character == character);

        if (alreadyExists)
        {
            if (!enableOverride)
            {
                throw new InvalidOperationException($"Transition on character '{character}' already exists.");
            }

            transitions.RemoveAll(t => t.Character == character);
        }

        transitions.Add(transition);

        return this;
    }

    private void EnsureStateIsListed(State state)
    {
        if (!States.ContainsKey(state.Name))
        {
            throw new InvalidOperationException($"State {state.Name} does not exist.");
        }
    }

    private List<Transition> GetTransitions(State state)
    {
        if (!Transitions.TryGetValue(state.Name, out var transitions))
        {
            transitions = new List<Transition>();
            Transitions.Add(state.Name, transitions);
        }

        return transitions;
    }

    /// <summary>
    /// Creates a <see cref="TableTransitionBuilder"/> for adding transitions from the initial state.
    /// </summary>
    /// <param name="enableOverride">Whether to enable overriding existing transitions.</param>
    /// <returns>A <see cref="TableTransitionBuilder"/> instance.</returns>
    public TableTransitionBuilder FromInitialState(bool enableOverride = false)
    {
        return new TableTransitionBuilder(
            builder: this,
            currentState: GetInitialState(),
            enableOverride: enableOverride);
    }

    /// <summary>
    /// Creates a <see cref="TableTransitionBuilder"/> for adding transitions from the specified state.
    /// </summary>
    /// <param name="name">The name of the state.</param>
    /// <param name="enableOverride">Whether to enable overriding existing transitions.</param>
    /// <returns>A <see cref="TableTransitionBuilder"/> instance.</returns>
    public TableTransitionBuilder FromState(string name, bool enableOverride = false)
    {
        var state = FindState(name);

        if (state is null)
        {
            state = CreateState(name, isAcceptingState: false);
        }

        return new TableTransitionBuilder(
            builder: this,
            currentState: state,
            enableOverride: enableOverride);
    }

    /// <summary>
    /// Finds a state by its name.
    /// </summary>
    /// <param name="name">The name of the state.</param>
    /// <returns>The state if found; otherwise, <c>null</c>.</returns>
    private State? FindState(string name)
    {
        if (!States.TryGetValue(name, out var state))
        {
            return null;
        }

        return state;
    }

    /// <summary>
    /// Builds the <see cref="TokenizerTable"/> from the current states and transitions.
    /// </summary>
    /// <returns>The constructed <see cref="TokenizerTable"/>.</returns>
    public TokenizerMachine Build()
    {
        var entries = new Dictionary<State, Transition[]>();

        foreach (var state in States.Values)
        {
            entries.Add(state, GetTransitions(state).ToArray());
        }

        return new TokenizerMachine(
            table: new TokenizerTable(entries),
            useDebugger: UseDebugger);
    }

    /// <summary>
    /// Builds the <see cref="TokenizerTable"/> from the current states and transitions.
    /// </summary>
    /// <returns>The constructed <see cref="TokenizerTable"/>.</returns>
    public TokenizerTable BuildTable()
    {
        var entries = new Dictionary<State, Transition[]>();

        foreach (var state in States.Values)
        {
            entries.Add(state, GetTransitions(state).ToArray());
        }

        return new TokenizerTable(entries);
    }

}
