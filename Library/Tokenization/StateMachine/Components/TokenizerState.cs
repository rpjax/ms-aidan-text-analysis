namespace Aidan.TextAnalysis.Tokenization.StateMachine.Components;

public enum OnNoTransitionBehavior
{
    None,
    Recurse,
    GoTo
}

/// <summary>
/// Represents a state in the state machine.
/// </summary>
public class TokenizerState : IEquatable<TokenizerState>
{
    /// <summary>
    /// Gets the unique identifier of the state.
    /// </summary>
    public uint Id { get; }

    /// <summary>
    /// Gets the name of the state.
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether the state is an accepting state.
    /// </summary>
    public bool IsAccepting { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether the state is recursive on no transition.
    /// </summary>
    public bool IsRecursiveOnNoTransition { get; internal set; }

    /* I'm still thinking about this one, it adds too much complexity for the parser, maybe a lookup table would be better. */
    //public uint Hash { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenizerState"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the state.</param>
    /// <param name="name">The name of the state.</param>
    /// <param name="isAccepting">A value indicating whether the state is an accepting state.</param>
    /// <param name="isRecursiveOnNoTransition">A value indicating whether the state is recursive on no transition.</param>
    public TokenizerState(
        uint id,
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
        return $"q{Id} {(IsAccepting ? $"accepts '{Name}'" : "")}";
    }

    /// <summary>
    /// Calculates the hash code for the state.
    /// </summary>
    /// <returns>The hash code for the state.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Id.GetHashCode();
            hash = hash * 23 + Name.GetHashCode();
            hash = hash * 23 + IsAccepting.GetHashCode();
            return hash;
        }
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other"></param>
    /// <returns> Returns <c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public bool Equals(TokenizerState? other)
    {
        if (other is null)
        {
            return false;
        }

        return Id == other.Id
            && Name == other.Name
            && IsAccepting == other.IsAccepting;
    }

}
