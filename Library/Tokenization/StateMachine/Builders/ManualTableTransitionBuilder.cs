using Aidan.TextAnalysis.Tokenization.StateMachine.Components;

namespace Aidan.TextAnalysis.Tokenization.StateMachine.Builders;

/// <summary>
/// Represents a transition builder for manually constructing a tokenizer's DFA (Deterministic Finite Automaton).
/// <br/> This builder allows defining transitions between states, self-recursive transitions, and accepting states
/// <br/> for the tokenizer's state machine.
/// </summary>
public class ManualTableTransitionBuilder : TableTransitionBuilderBase<ManualTableTransitionBuilder>
{
    /// <summary>
    /// Gets the <see cref="ManualTokenizerBuilder"/> instance that owns this transition builder.
    /// </summary>
    private ManualTokenizerBuilder Builder { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ManualTableTransitionBuilder"/> class.
    /// </summary>
    /// <param name="builder">The <see cref="ManualTokenizerBuilder"/> that owns this builder.</param>
    /// <param name="currentState">The current state being configured in the DFA.</param>
    public ManualTableTransitionBuilder(
        ManualTokenizerBuilder builder,
        TokenizerState currentState)
        : base(
            currentState: currentState,
            charset: builder.GetCharset())
    {
        Builder = builder;
    }

    /// <summary>
    /// Creates a transition from the current state to the specified target state on the set of previously defined characters.
    /// <br/> This causes the DFA to transition to the target state and consume a character from the input.
    /// <br/> If the target state does not exist, it will be created.
    /// </summary>
    /// <param name="name">The name of the target state.</param>
    /// <returns>The <see cref="ManualTokenizerBuilder"/> instance for further configuration.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no characters have been set for the transition.</exception>
    public ManualTokenizerBuilder GoTo(string name)
    {
        if (Characters.Count == 0)
        {
            throw new InvalidOperationException("Characters not set.");
        }

        var nextState = Builder.FindState(name);

        if (nextState == null)
        {
            nextState = Builder.CreateState(name, isAccepting: false);
        }

        foreach (var character in Characters)
        {
            Builder.AddTransition(CurrentState.Name, character, nextState.Name);
        }

        return Builder;
    }

    /// <summary>
    /// Marks the current state as an accepting state, meaning it will emit a token if no valid transitions exist 
    /// <br/> for the current lookahead character.
    /// <br/> This does not consume a character or create a state transition.
    /// </summary>
    /// <returns>The <see cref="ManualTokenizerBuilder"/> instance for further configuration.</returns>
    public ManualTokenizerBuilder Accept()
    {
        if (!CurrentState.IsAccepting)
        {
            CurrentState.IsAccepting = true;
        }

        return Builder;
    }

    /// <summary>
    /// Configures the current state to recurse on itself if no valid transitions are found for the current lookahead character.
    /// <br/> This causes the DFA to consume the character and remain in the same state.
    /// </summary>
    /// <returns>The <see cref="ManualTokenizerBuilder"/> instance for further configuration.</returns>
    public ManualTokenizerBuilder RecurseOnNoTransition()
    {
        if (!CurrentState.IsRecursiveOnNoTransition)
        {
            CurrentState.IsRecursiveOnNoTransition = true;
        }

        return Builder;
    }

    /// <summary>
    /// Creates self-recursive transitions for the current state on the set of previously defined characters.
    /// <br/> This causes the DFA to loop back to the current state, consuming the input character.
    /// </summary>
    /// <returns>The <see cref="ManualTokenizerBuilder"/> instance for further configuration.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no characters have been set for the transition.</exception>
    public ManualTokenizerBuilder Recurse()
    {
        if (Characters.Count == 0)
        {
            throw new InvalidOperationException("Characters not set.");
        }

        foreach (var character in Characters)
        {
            Builder.AddTransition(CurrentState.Name, character, CurrentState.Name);
        }

        return Builder;
    }

    /// <summary>
    /// Creates transitions from the current state to the initial state on the set of previously defined characters.
    /// <br/> This causes the DFA to reset to the initial state while consuming the input character.
    /// </summary>
    /// <returns>The <see cref="ManualTokenizerBuilder"/> instance for further configuration.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no characters have been set for the transition.</exception>
    public ManualTokenizerBuilder GoToInitialState()
    {
        if (Characters.Count == 0)
        {
            throw new InvalidOperationException("Characters not set.");
        }

        var initialState = Builder.GetInitialState();

        foreach (var character in Characters)
        {
            Builder.AddTransition(CurrentState.Name, character, initialState.Name);
        }

        return Builder;
    }
}
