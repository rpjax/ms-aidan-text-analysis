namespace Aidan.TextAnalysis.Tokenization.StateMachine.Components;

/// <summary>
/// Represents a transition in the state machine.
/// </summary>
public class TokenizerTransition
{
    /// <summary>
    /// Gets the character that triggers the transition.
    /// </summary>
    public char Character { get; }

    /// <summary>
    /// Gets the identifier of the state to transition to.
    /// </summary>
    public uint StateId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenizerTransition"/> class.
    /// </summary>
    /// <param name="character">The character that triggers the transition.</param>
    /// <param name="stateId">The identifier of the state to transition to.</param>
    public TokenizerTransition(char character, uint stateId)
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

