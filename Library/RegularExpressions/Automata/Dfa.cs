namespace Aidan.TextAnalysis.RegularExpressions.Automata;

/// <summary>
/// Represents a deterministic finite automaton (DFA).
/// </summary>
public class Dfa
{
    /// <summary>
    /// Gets the states of the DFA.
    /// </summary>
    public IReadOnlyList<DfaState> States { get; }

    /// <summary>
    /// Gets the alphabet of the DFA.
    /// </summary>
    public IReadOnlyList<char> Alphabet { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Dfa"/> class.
    /// </summary>
    /// <param name="states">The states of the DFA.</param>
    /// <param name="alphabet">The alphabet of the DFA.</param>
    public Dfa(IEnumerable<DfaState> states, IReadOnlyList<char> alphabet)
    {
        States = states.ToArray();
        Alphabet = alphabet;
        Validate();
    }

    private void Validate()
    {
        foreach (var state in States)
        {
            var chars = state.Transitions
                .Select(x => x.Character)
                .ToArray();

            var group = chars
                .GroupBy(x => x)
                .ToArray();

            if (group.Any(x => x.Count() > 1))
            {
                throw new InvalidOperationException("Duplicate transitions detected");
            }
        }
    }
}

