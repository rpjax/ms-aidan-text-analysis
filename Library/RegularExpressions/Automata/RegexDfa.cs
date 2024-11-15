﻿namespace Aidan.TextAnalysis.RegularExpressions.Automata;

/// <summary>
/// Represents a deterministic finite automaton (DFA) for regular expressions.
/// </summary>
public class RegexDfa
{
    /// <summary>
    /// Gets the alphabet of the DFA.
    /// </summary>
    public IReadOnlyList<char> Alphabet { get; }

    /// <summary>
    /// Gets the states of the DFA.
    /// </summary>
    public IReadOnlyList<AutomatonNode> States { get; }

    /// <summary>
    /// Gets the initial state of the DFA.
    /// </summary>
    public AutomatonNode InitialState { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexDfa"/> class.
    /// </summary>
    /// <param name="alphabet">The alphabet of the DFA.</param>
    /// <param name="states">The states of the DFA.</param>
    /// <exception cref="InvalidOperationException">Thrown when duplicate states are found.</exception>
    public RegexDfa(
        IEnumerable<char> alphabet,
        IEnumerable<AutomatonNode> states)
    {
        Alphabet = alphabet.ToArray();
        States = states.ToArray();
        InitialState = States.First();

        // Ensure that there are no duplicate states
        var groups = States
            .GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .ToArray();

        if (groups.Length > 1)
        {
            throw new InvalidOperationException("Duplicate states found.");
        }
    }
}
