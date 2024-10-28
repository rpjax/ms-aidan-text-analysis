namespace Aidan.TextAnalysis.Tokenization.StateMachine;

/// <summary>
/// Represents a transition in the state machine.
/// </summary>
public class Transition
{
    /// <summary>
    /// Gets the character that triggers the transition.
    /// </summary>
    public char Character { get; }

    /// <summary>
    /// Gets the identifier of the state to transition to.
    /// </summary>
    public int StateId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Transition"/> class.
    /// </summary>
    /// <param name="character">The character that triggers the transition.</param>
    /// <param name="stateId">The identifier of the state to transition to.</param>
    public Transition(char character, int stateId)
    {
        Character = character;
        StateId = stateId;
    }

    /// <summary>
    /// Returns a string that represents the current transition.
    /// </summary>
    /// <returns>A string that represents the current transition.</returns>
    public override string ToString()
    {
        var c = Character == '\0'
            ? "EOI"
            : $"'{Character}'"
            ;

        return $"on {c} goto state {StateId}";
    }
}

/// <summary>
/// Represents a table of transitions in the state machine.
/// </summary>
public class TransitionTable
{
    /// <summary>
    /// Gets the dictionary of transitions.
    /// </summary>
    private Dictionary<char, Transition> Entries { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TransitionTable"/> class.
    /// </summary>
    /// <param name="entries">The dictionary of transitions.</param>
    public TransitionTable(Dictionary<char, Transition> entries)
    {
        Entries = entries;
    }
}
