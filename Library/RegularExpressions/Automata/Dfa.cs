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
    /// Gets the initial state of the DFA.
    /// </summary>
    public DfaState InitialState { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Dfa"/> class.
    /// </summary>
    /// <param name="states">The states of the DFA.</param>
    /// <param name="alphabet">The alphabet of the DFA.</param>
    public Dfa(IEnumerable<DfaState> states, IReadOnlyList<char> alphabet)
    {
        Validate(states);
        States = states.ToArray();
        Alphabet = alphabet;
        InitialState = States.First();
    }

    private static void Validate(IEnumerable<DfaState> states)
    {
        if (!states.Any())
        {
            throw new InvalidOperationException("No states detected");
        }

        foreach (var state in states)
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

public class RegexDfa
{
    public IReadOnlyList<AutomatonNode> States { get; }
    public AutomatonNode InitialState { get; }

    public RegexDfa(IEnumerable<AutomatonNode> states)
    {
        States = states.ToArray();
        InitialState = States.First();
    }
}
