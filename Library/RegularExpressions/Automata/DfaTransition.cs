using Aidan.TextAnalysis.RegularExpressions.Ast;

namespace Aidan.TextAnalysis.RegularExpressions.Automata;

/// <summary>
/// Represents a transition in a deterministic finite automaton (DFA).
/// </summary>
public class DfaTransition : IEquatable<DfaTransition>
{
    /// <summary>
    /// Gets the character that triggers the transition.
    /// </summary>
    public char Character { get; }

    /// <summary>
    /// Gets the source state of the transition.
    /// </summary>
    public string NextState { get; }

    /// <summary>
    /// Gets the derivative of the transition.
    /// </summary>
    public RegexNode Derivative { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DfaTransition"/> class.
    /// </summary>
    /// <param name="character">The character that triggers the transition.</param>
    /// <param name="nextState">The next state of the transition.</param>
    /// <param name="derivative">The derivative of the transition.</param>
    public DfaTransition(
        char character,
        string nextState,
        RegexNode derivative)
    {
        Character = character;
        NextState = nextState;
        Derivative = derivative;
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="DfaTransition"/>.
    /// </summary>
    /// <returns>A string that represents the current <see cref="DfaTransition"/>.</returns>
    public override string ToString()
    {
        return $"ON '{Character}' GOTO `{NextState}`";
    }

    /// <summary>
    /// Determines whether the specified <see cref="DfaTransition"/> is equal to the current <see cref="DfaTransition"/>.
    /// </summary>
    /// <param name="other">The <see cref="DfaTransition"/> to compare with the current <see cref="DfaTransition"/>.</param>
    /// <returns>true if the specified <see cref="DfaTransition"/> is equal to the current <see cref="DfaTransition"/>; otherwise, false.</returns>
    public bool Equals(DfaTransition? other)
    {
        return other is not null
            && Character == other.Character
            && NextState == other.NextState;
    }
}

