namespace Aidan.TextAnalysis.Tokenization.StateMachine;

/// <summary>
/// Represents a state in the state machine.
/// </summary>
public class State
{
    /// <summary>
    /// Gets the unique identifier of the state.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the name of the state.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets a value indicating whether the state is an accepting state.
    /// </summary>
    public bool IsAccepting { get; }

    /// <summary>
    /// Gets a value indicating whether the state is recursive on no transition.
    /// </summary>
    public bool IsRecursiveOnNoTransition { get; internal set; }

    /* I'm still thinking about this one, it adds too much complexity for the parser, maybe a lookup table would be better. */
    //public uint Hash { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="State"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the state.</param>
    /// <param name="name">The name of the state.</param>
    /// <param name="isAccepting">A value indicating whether the state is an accepting state.</param>
    /// <param name="isRecursiveOnNoTransition">A value indicating whether the state is recursive on no transition.</param>
    public State(
        int id,
        string name,
        bool isAccepting,
        bool isRecursiveOnNoTransition)
    {
        Id = id;
        Name = name;
        IsAccepting = isAccepting;
        IsRecursiveOnNoTransition = isRecursiveOnNoTransition;
    }

    /// <summary>
    /// Returns a string that represents the current state.
    /// </summary>
    /// <returns>A string that represents the current state.</returns>
    public override string ToString()
    {
        return $"{Name} ({Id}) {(IsAccepting ? "Accepting" : "")}";
    }
}
