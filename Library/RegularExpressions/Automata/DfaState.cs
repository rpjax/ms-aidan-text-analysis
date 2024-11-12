using Aidan.Core.Patterns;
using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;
using System.Text;

namespace Aidan.TextAnalysis.RegularExpressions.Automata;

/// <summary>
/// Represents a state in a deterministic finite automaton (DFA).
/// </summary>
public class DfaState : IEquatable<DfaState>
{
    /// <summary>
    /// Gets the name of the DFA state.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the transitions from the DFA state.
    /// </summary>
    public IReadOnlyList<DfaTransition> Transitions { get; }

    /// <summary>
    /// Gets a value indicating whether the DFA state is an accepting state.
    /// </summary>
    public bool IsAccepting { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DfaState"/> class.
    /// </summary>
    /// <param name="name">The name of the DFA state.</param>
    /// <param name="transitions">The transitions from the DFA state.</param>
    /// <param name="isAccepting">A value indicating whether the DFA state is an accepting state.</param>
    public DfaState(
        string name,
        IEnumerable<DfaTransition> transitions,
        bool isAccepting)
    {
        Name = name;
        Transitions = transitions.ToArray();
        IsAccepting = isAccepting;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="DfaStateBuilder"/> class.
    /// </summary>
    /// <returns>A new instance of the <see cref="DfaStateBuilder"/> class.</returns>
    public static DfaStateBuilder Create() => new DfaStateBuilder();

    /// <summary>
    /// Determines whether the specified <see cref="DfaState"/> is equal to the current <see cref="DfaState"/>.
    /// </summary>
    /// <param name="other">The <see cref="DfaState"/> to compare with the current <see cref="DfaState"/>.</param>
    /// <returns>true if the specified <see cref="DfaState"/> is equal to the current <see cref="DfaState"/>; otherwise, false.</returns>
    public bool Equals(DfaState? other)
    {
        return other is not null
            && Name == other.Name
            ;
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="DfaState"/>.
    /// </summary>
    /// <returns>A string that represents the current <see cref="DfaState"/>.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(Name);

        if(IsAccepting)
        {
            sb.Append($" (accept)");
        }

        return sb.ToString();
    }
}

/// <summary>
/// Provides a builder for creating instances of the <see cref="DfaState"/> class.
/// </summary>
public class DfaStateBuilder : IBuilder<DfaState>
{
    private string? Name { get; set; }
    private List<DfaTransition> Transitions { get; } = new List<DfaTransition>();
    private bool IsAccepting { get; set; }

    /// <summary>
    /// Sets the name of the DFA state.
    /// </summary>
    /// <param name="name">The name of the DFA state.</param>
    /// <returns>The current instance of the <see cref="DfaStateBuilder"/> class.</returns>
    public DfaStateBuilder WithName(string name)
    {
        Name = name;
        return this;
    }

    /// <summary>
    /// Adds a transition to the DFA state.
    /// </summary>
    /// <param name="character">The character that triggers the transition.</param>
    /// <param name="nextState">The name of the next state.</param>
    /// <returns>The current instance of the <see cref="DfaStateBuilder"/> class.</returns>
    public DfaStateBuilder WithTransition(char character, string nextState)
    {
        Transitions.Add(new DfaTransition(character, nextState));
        return this;
    }

    /// <summary>
    /// Adds multiple transitions to the DFA state.
    /// </summary>
    /// <param name="transitions">The transitions to add.</param>
    /// <returns>The current instance of the <see cref="DfaStateBuilder"/> class.</returns>
    public DfaStateBuilder WithTransitions(IEnumerable<DfaTransition> transitions)
    {
        Transitions.AddRange(transitions);
        return this;
    }

    /// <summary>
    /// Sets the DFA state as accepting.
    /// </summary>
    /// <returns>The current instance of the <see cref="DfaStateBuilder"/> class.</returns>
    public DfaStateBuilder SetAccepting()
    {
        IsAccepting = true;
        return this;
    }

    /// <summary>
    /// Builds a new instance of the <see cref="DfaState"/> class.
    /// </summary>
    /// <returns>A new instance of the <see cref="DfaState"/> class.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the name or pattern is not set.</exception>
    public DfaState Build()
    {
        return new DfaState(
            name: Name ?? throw new InvalidOperationException("Name is required"),
            transitions: Transitions,
            isAccepting: IsAccepting);
    }
}
