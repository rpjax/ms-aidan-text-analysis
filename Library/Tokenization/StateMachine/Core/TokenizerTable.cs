using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.Tokenization.StateMachine;

/// <summary>
/// Represents a table used for tokenization, containing states and transitions.
/// </summary>
public class TokenizerTable : ITokenizerTable
{
    /// <summary>
    /// The maximum allowed state ID.
    /// </summary>
    public const int MaxStateId = short.MaxValue;

    /// <summary>
    /// The maximum number of states allowed.
    /// </summary>
    public const int MaxStateCount = MaxStateId + 1;

    /// <summary>
    /// A dictionary mapping state IDs to their corresponding states.
    /// </summary>
    private Dictionary<int, State> States { get; }

    /// <summary>
    /// A dictionary mapping combined state and character keys to their corresponding transitions.
    /// </summary>
    private Dictionary<int, Transition> Transitions { get; }

    /// <summary>
    /// Gets the state and transition entries.
    /// </summary>
    private Dictionary<State, Transition[]> Entries { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenizerTable"/> class with the specified state and transition entries.
    /// </summary>
    /// <param name="entries">A dictionary containing states and their corresponding transitions.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the number of states exceeds the maximum allowed count or when a state ID exceeds the maximum allowed value.
    /// </exception>
    public TokenizerTable(Dictionary<State, Transition[]> entries)
    {
        States = entries.Keys
            .ToDictionary(x => x.Id, x => x);

        Transitions = new Dictionary<int, Transition>();

        Entries = entries;

        if (States.Count > MaxStateCount)
        {
            throw new InvalidOperationException($"State count exceeds maximum of {MaxStateCount}");
        }

        if (States.Any(x => x.Key > MaxStateId))
        {
            throw new InvalidOperationException($"State ID exceeds maximum of {MaxStateId}");
        }

        foreach (var entry in entries)
        {
            var state = entry.Key;

            foreach (var transition in entry.Value)
            {
                var key = Combine(state.Id, transition.Character);
                var stateName = state.Name;

                if (Transitions.ContainsKey(key))
                {
                    var c = transition.Character == '\0'
                        ? "EOF"
                        : transition.Character.ToString();

                    throw new InvalidOperationException($"Duplicate transition for state '{stateName}' on character '{c}'");
                }

                Transitions.Add(key, transition);
            }
        }
    }

    /// <summary>
    /// Gets the initial state of the tokenizer.
    /// </summary>
    /// <returns>The initial state.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public State GetInitialState()
    {
        return States[0];
    }

    /// <summary>
    /// Looks up the next state based on the current state and character.
    /// </summary>
    /// <param name="state">The current state ID.</param>
    /// <param name="character">The current character.</param>
    /// <returns>The next state, or <c>null</c> if no transition is found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public State? LookUp(int state, char character)
    {
        var key = Combine(state, character);

        if (!Transitions.TryGetValue(key, out Transition? transition))
        {
            return null;
        }

        if (!States.TryGetValue(transition.StateId, out State? nextState))
        {
            return null;
        }

        return nextState;
    }

    /// <summary>
    /// Gets the state and transition entries.
    /// </summary>
    /// <returns>A dictionary containing states and their corresponding transitions.</returns>
    public Dictionary<State, Transition[]> GetEntries()
    {
        return Entries;
    }

    /// <summary>
    /// Combines the state ID and character into a single key for dictionary lookup.
    /// </summary>
    /// <param name="state">The state ID.</param>
    /// <param name="character">The character.</param>
    /// <returns>A combined key representing the state and character.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Combine(int state, char character)
    {
        return (state << 16) | character;
    }
}
