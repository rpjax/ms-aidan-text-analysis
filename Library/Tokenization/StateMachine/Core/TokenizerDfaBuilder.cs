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
    /// <summary>
    /// A dictionary mapping state names to their corresponding states.
    /// </summary>
    private Dictionary<string, State> States { get; }

    /// <summary>
    /// A dictionary mapping state names to their corresponding transitions.
    /// </summary>
    private Dictionary<string, List<Transition>> Transitions { get; }

    private char[] Charset { get; set; }

    private string InitialStateName { get; set; }

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
            isAccepting: false);

        States.Add(initialState.Name, initialState);
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
            isAccepting: isAcceptingState);

        States.Add(name, state);

        return state;
    }

    /// <summary>
    /// Adds a transition from the current state to the next state on the specified character.
    /// </summary>
    /// <param name="currentState">The current state.</param>
    /// <param name="character">The character triggering the transition.</param>
    /// <param name="nextState">The next state.</param>
    /// <returns>The current <see cref="TokenizerDfaBuilder"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the current or next state does not exist.</exception>
    public TokenizerDfaBuilder AddTransition(
        State currentState,
        char character,
        State nextState)
    {
        EnsureStateIsListed(currentState);
        EnsureStateIsListed(nextState);

        var transitions = GetTransitions(currentState);

        var transition = new Transition(
            character: character,
            stateId: nextState.Id);

        transitions.Add(transition);

        return this;
    }

    /// <summary>
    /// Ensures that the specified state is listed in the states dictionary.
    /// </summary>
    /// <param name="state">The state to check.</param>
    /// <exception cref="InvalidOperationException">Thrown when the state does not exist.</exception>
    private void EnsureStateIsListed(State state)
    {
        if (!States.ContainsKey(state.Name))
        {
            throw new InvalidOperationException($"State {state.Name} does not exist.");
        }
    }

    /// <summary>
    /// Gets the list of transitions for the specified state.
    /// </summary>
    /// <param name="state">The state to get transitions for.</param>
    /// <returns>The list of transitions for the specified state.</returns>
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
    /// <returns>A <see cref="TableTransitionBuilder"/> instance.</returns>
    public TableTransitionBuilder FromInitialState()
    {
        return new TableTransitionBuilder(
            builder: this,
            currentState: GetInitialState());
    }

    /// <summary>
    /// Creates a <see cref="TableTransitionBuilder"/> for adding transitions from the specified state.
    /// </summary>
    /// <param name="name">The name of the state.</param>
    /// <returns>A <see cref="TableTransitionBuilder"/> instance.</returns>
    public TableTransitionBuilder FromState(string name)
    {
        var state = FindState(name);

        if (state is null)
        {
            state = new State(
                id: States.Count,
                name: name,
                isAccepting: false);

            States.Add(name, state);
        }

        return new TableTransitionBuilder(
            builder: this,
            currentState: state);
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

        return new TokenizerMachine(new TokenizerTable(entries));
    }
}

/// <summary>
/// A builder class for constructing transitions in a <see cref="TokenizerTable"/>.
/// </summary>
public class TableTransitionBuilder
{
    /// <summary>
    /// The parent <see cref="TokenizerDfaBuilder"/> instance.
    /// </summary>
    private TokenizerDfaBuilder Builder { get; }

    /// <summary>
    /// The current state from which transitions are being added.
    /// </summary>
    private State CurrentState { get; }

    /// <summary>
    /// A list of characters that trigger transitions.
    /// </summary>
    private List<char> Characters { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TableTransitionBuilder"/> class.
    /// </summary>
    /// <param name="builder">The parent <see cref="TokenizerDfaBuilder"/> instance.</param>
    /// <param name="currentState">The current state from which transitions are being added.</param>
    public TableTransitionBuilder(
        TokenizerDfaBuilder builder,
        State currentState)
    {
        Builder = builder;
        CurrentState = currentState;
    }

    /// <summary>
    /// Adds a character that triggers a transition.
    /// </summary>
    /// <param name="character">The character to add.</param>
    /// <returns>The current <see cref="TableTransitionBuilder"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the character has already been added.</exception>
    public TableTransitionBuilder OnCharacter(char character)
    {
        AddCharacter(character);
        return this;
    }

    /// <summary>
    /// Adds a range of characters that trigger transitions.
    /// </summary>
    /// <param name="start">The starting character of the range.</param>
    /// <param name="end">The ending character of the range.</param>
    /// <returns>The current <see cref="TableTransitionBuilder"/> instance.</returns>
    public TableTransitionBuilder OnCharacterRange(char start, char end)
    {
        for (var i = start; i <= end; i++)
        {
            AddCharacter(i);
        }

        return this;
    }

    /// <summary>
    /// Adds multiple characters that trigger transitions.
    /// </summary>
    /// <param name="characters">The characters to add.</param>
    /// <returns>The current <see cref="TableTransitionBuilder"/> instance.</returns>
    public TableTransitionBuilder OnManyCharacters(params char[] characters)
    {
        foreach (var character in characters)
        {
            AddCharacter(character);
        }

        return this;
    }

    public TableTransitionBuilder OnAnyCharacter()
    {
        foreach (var character in Builder.GetCharset())
        {
            AddCharacter(character);
        }

        return this;
    }

    public TableTransitionBuilder OnAnyCharacterExcept(params char[] characters)
    {
        foreach (var character in Builder.GetCharset())
        {
            if (!characters.Contains(character))
            {
                AddCharacter(character);
            }
        }

        return this;
    }

    public TableTransitionBuilder OnAnyCharacterExceptRange(char start, char end)
    {
        foreach (var character in Builder.GetCharset())
        {
            if (character < start || character > end)
            {
                AddCharacter(character);
            }
        }

        return this;
    }

    public TableTransitionBuilder Except(params char[] characters)
    {
        foreach (var character in characters)
        {
            Characters.Remove(character);
        }

        return this;
    }

    public TableTransitionBuilder ExceptRange(char start, char end)
    {
        for (var i = start; i <= end; i++)
        {
            Characters.Remove(i);
        }

        return this;
    }

    /// <summary>
    /// Adds the end of input character as a trigger for a transition.
    /// </summary>
    /// <returns>The current <see cref="TableTransitionBuilder"/> instance.</returns>
    public TableTransitionBuilder OnEoi()
    {
        return OnCharacter(TokenizerMachine.EoiChar);
    }

    /// <summary>
    /// Adds a whitespace character as a trigger for a transition.
    /// </summary>
    /// <returns>The current <see cref="TableTransitionBuilder"/> instance.</returns>
    public TableTransitionBuilder OnWhitespace()
    {
        return OnCharacter(' ');
    }

    public TableTransitionBuilder OnDigit()
    {
        return OnCharacterRange('0', '9');
    }

    /// <summary>
    /// Specifies the next state to transition to.
    /// </summary>
    /// <param name="name">The name of the next state.</param>
    /// <returns>The parent <see cref="TokenizerDfaBuilder"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no characters have been set.</exception>
    public TokenizerDfaBuilder GoTo(string name)
    {
        if (Characters.Count == 0)
        {
            throw new InvalidOperationException("Characters not set.");
        }

        var nextState = Builder.CreateState(name, isAcceptingState: false);

        foreach (var character in Characters)
        {
            Builder.AddTransition(CurrentState, character, nextState);
        }

        return Builder;
    }

    /// <summary>
    /// Specifies the next state to transition to and marks it as an accepting state.
    /// </summary>
    /// <param name="name">The name of the next state.</param>
    /// <returns>The parent <see cref="TokenizerDfaBuilder"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no characters have been set.</exception>
    public TokenizerDfaBuilder Accept(string name)
    {
        if (Characters.Count == 0)
        {
            throw new InvalidOperationException("Characters not set.");
        }

        var nextState = Builder.CreateState(name, isAcceptingState: true);

        foreach (var character in Characters)
        {
            Builder.AddTransition(CurrentState, character, nextState);
        }

        return Builder;
    }

    /// <summary>
    /// Specifies that the current state should transition to itself on the specified characters.
    /// </summary>
    /// <returns>The parent <see cref="TokenizerDfaBuilder"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no characters have been set.</exception>
    public TokenizerDfaBuilder Recurse()
    {
        if (Characters.Count == 0)
        {
            throw new InvalidOperationException("Characters not set.");
        }

        foreach (var character in Characters)
        {
            Builder.AddTransition(CurrentState, character, CurrentState);
        }

        return Builder;
    }

    /// <summary>
    /// Specifies that the current state should transition to the initial state on the specified characters.
    /// </summary>
    /// <returns>The parent <see cref="TokenizerDfaBuilder"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no characters have been set.</exception>
    public virtual TokenizerDfaBuilder GoToInitialState()
    {
        if (Characters.Count == 0)
        {
            throw new InvalidOperationException("Characters not set.");
        }

        var initialState = Builder.GetInitialState();

        foreach (var character in Characters)
        {
            Builder.AddTransition(CurrentState, character, initialState);
        }

        return Builder;
    }

    /// <summary>
    /// Adds a character to the list of characters that trigger transitions.
    /// </summary>
    /// <param name="character">The character to add.</param>
    /// <exception cref="InvalidOperationException">Thrown when the character has already been added.</exception>
    private void AddCharacter(char character)
    {
        if (Characters.Contains(character))
        {
            throw new InvalidOperationException($"Character '{character}' already added.");
        }
        if (!Builder.GetCharset().Contains(character))
        {
            throw new InvalidOperationException($"Character '{character}' is not in the charset.");
        }

        Characters.Add(character);
    }
}
