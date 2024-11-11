namespace Aidan.TextAnalysis.Tokenization.StateMachine;

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

    private bool EnableOverride { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableTransitionBuilder"/> class.
    /// </summary>
    /// <param name="builder">The parent <see cref="TokenizerDfaBuilder"/> instance.</param>
    /// <param name="currentState">The current state from which transitions are being added.</param>
    /// <param name="enableOverride">A value indicating whether to enable overriding transitions.</param>
    public TableTransitionBuilder(
        TokenizerDfaBuilder builder,
        State currentState,
        bool enableOverride)
    {
        Builder = builder;
        CurrentState = currentState;
        EnableOverride = enableOverride;
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
            if (characters.Contains(character))
            {
                continue;
            }

            AddCharacter(character);
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

    public TableTransitionBuilder ExceptWhitespace()
    {
        return Except(' ');
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
            Builder.AddTransition(CurrentState, character, nextState, EnableOverride);
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
            Builder.AddTransition(CurrentState, character, nextState, EnableOverride);
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
            Builder.AddTransition(CurrentState, character, CurrentState, EnableOverride);
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
            Builder.AddTransition(CurrentState, character, initialState, EnableOverride);
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
        if (EnableOverride && Characters.Contains(character))
        {
            Characters.Remove(character);
        }

        Characters.Add(character);
    }
}
