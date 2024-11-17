using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Tokenization.Components;
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

    /* Getter Methods */

    /// <summary>
    /// Retrieves the character set used by the tokenizer.
    /// </summary>
    /// <returns>An array of characters representing the character set.</returns>
    public char[] GetCharset()
    {
        return Charset;
    }

    /// <summary>
    /// Finds a tokenizer state by its name.
    /// </summary>
    /// <param name="name">The name of the state to search for.</param>
    /// <returns>The <see cref="TokenizerState"/> corresponding to the name, or <c>null</c> if no state is found.</returns>
    public TokenizerState? FindState(string name)
    {
        if (!NameStateMap.TryGetValue(name, out var state))
        {
            return null;
        }

        return state;
    }

    /// <summary>
    /// Retrieves a tokenizer state by its name. If the state is not found, an exception is thrown.
    /// </summary>
    /// <param name="name">The name of the state to retrieve.</param>
    /// <returns>The corresponding <see cref="TokenizerState"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the state does not exist.</exception>
    public TokenizerState GetState(string name)
    {
        if (!NameStateMap.TryGetValue(name, out var state))
        {
            throw new InvalidOperationException($"State {name} does not exist.");
        }

        return state;
    }

    /// <summary>
    /// Retrieves the initial state of the tokenizer.
    /// </summary>
    /// <returns>The initial <see cref="TokenizerState"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no states are defined.</exception>
    public TokenizerState GetInitialState()
    {
        if (States.Count == 0)
        {
            throw new InvalidOperationException("No initial state defined.");
        }

        return States.First();
    }

    /* Setter Methods */

    /// <summary>
    /// Sets the character set for the tokenizer using an array of characters.
    /// </summary>
    /// <param name="chars">The array of characters to use as the character set.</param>
    /// <returns>The current <see cref="ManualTokenizerBuilder"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown if the character set is empty.</exception>
    public ManualTokenizerBuilder SetCharset(params char[] chars)
    {
        if (chars.Length == 0)
        {
            throw new ArgumentException("Charset must contain at least one character.");
        }

        Charset = chars;
        return this;
    }

    /// <summary>
    /// Sets the character set for the tokenizer using an enumerable collection of characters.
    /// </summary>
    /// <param name="chars">The collection of characters to use as the character set.</param>
    /// <returns>The current <see cref="ManualTokenizerBuilder"/> instance.</returns>
    public ManualTokenizerBuilder SetCharset(IEnumerable<char> chars)
    {
        return SetCharset(chars.ToArray());
    }

    /// <summary>
    /// Sets the character set for the tokenizer using a predefined charset type (e.g., ASCII, UTF-8).
    /// </summary>
    /// <param name="charset">The predefined charset type.</param>
    /// <returns>The current <see cref="ManualTokenizerBuilder"/> instance.</returns>
    public ManualTokenizerBuilder SetCharset(CharsetType charset)
    {
        Charset = TokenizerBuilder.ComputeCharset(charset);
        return this;
    }

    /// <summary>
    /// Enables debugging features for the tokenizer.
    /// </summary>
    /// <returns>The current <see cref="ManualTokenizerBuilder"/> instance.</returns>
    public ManualTokenizerBuilder EnableDebugger()
    {
        UseDebugger = true;
        return this;
    }

    /// <summary>
    /// Creates a new tokenizer state with the specified name and acceptance status.
    /// </summary>
    /// <param name="name">The name of the new state.</param>
    /// <param name="isAccepting">Indicates whether the state is an accepting state.</param>
    /// <returns>The created <see cref="TokenizerState"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a state with the same name already exists or if the state already has transitions.
    /// </exception>
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

    /// <summary>
    /// Adds a transition between two states based on a specific character.
    /// </summary>
    /// <param name="currentState">The name of the current state.</param>
    /// <param name="character">The character that triggers the transition.</param>
    /// <param name="nextState">The name of the next state.</param>
    /// <returns>The current <see cref="ManualTokenizerBuilder"/> instance.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if either the current or next state is not defined.
    /// </exception>
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

    /// <summary>
    /// Creates a <see cref="ManualTableTransitionBuilder"/> for defining transitions from the initial state.
    /// </summary>
    /// <returns>A new <see cref="ManualTableTransitionBuilder"/> instance.</returns>
    public ManualTableTransitionBuilder FromInitialState()
    {
        return new ManualTableTransitionBuilder(
            builder: this,
            currentState: GetInitialState());
    }

    /// <summary>
    /// Creates a <see cref="ManualTableTransitionBuilder"/> for defining transitions from a specific state.
    /// </summary>
    /// <param name="name">The name of the state to define transitions from.</param>
    /// <returns>A new <see cref="ManualTableTransitionBuilder"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the specified state does not exist.</exception>
    public ManualTableTransitionBuilder FromState(string name)
    {
        EnsureStateIsListed(name);

        return new ManualTableTransitionBuilder(
            builder: this,
            currentState: GetState(name));
    }

    /* Builder Methods */

    /// <summary>
    /// Builds a <see cref="TokenizerTable"/> representing the tokenizer's state machine.
    /// </summary>
    /// <returns>A <see cref="TokenizerTable"/> instance.</returns>
    public TokenizerTable BuildTable()
    {
        var entries = new Dictionary<TokenizerState, TokenizerTransition[]>();

        foreach (var state in States)
        {
            var transitions = Transitions.ContainsKey(state)
                ? Transitions[state].ToArray()
                : Array.Empty<TokenizerTransition>();

            entries.Add(state, transitions);
        }

        return new TokenizerTable(entries);
    }

    /// <summary>
    /// Builds the tokenizer using the defined states, transitions, and configuration.
    /// </summary>
    /// <returns>A new <see cref="Tokenizer"/> instance.</returns>
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
