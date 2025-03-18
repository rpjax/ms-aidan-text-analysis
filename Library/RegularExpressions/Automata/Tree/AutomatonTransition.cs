namespace Aidan.TextAnalysis.RegularExpressions.Automata.Tree;

/// <summary>
/// Represents a transition in an automaton.
/// </summary>
public class AutomatonTransition
{
    /// <summary>
    /// Gets the character that triggers the transition.
    /// </summary>
    public char Character { get; }

    /// <summary>
    /// Gets the next state in the automaton.
    /// </summary>
    public AutomatonState NextState { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutomatonTransition"/> class.
    /// </summary>
    /// <param name="character">The character that triggers the transition.</param>
    /// <param name="nextState">The next state in the automaton.</param>
    public AutomatonTransition(char character, AutomatonState nextState)
    {
        Character = character;
        NextState = nextState;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"ON '{Character}' GOTO {NextState}";
    }
}
