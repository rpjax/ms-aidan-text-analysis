using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;
using Aidan.TextAnalysis.RegularExpressions.Automata.Extensions;
using Aidan.TextAnalysis.RegularExpressions.Automata.Tree;
using Aidan.TextAnalysis.RegularExpressions.Derivative;

namespace Aidan.TextAnalysis.RegularExpressions.Automata.AutomatonComputation;

/// <summary>
/// Responsible for calculating the DFA (Deterministic Finite Automaton) from a set of lexemes and ignored characters.
/// </summary>
public class DfaCalculator
{
    public Lexeme[] Lexemes { get; }
    public IReadOnlyList<char> IgnoredCharacters { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DfaCalculator"/> class.
    /// </summary>
    /// <param name="lexemes">The lexemes to be used in the DFA.</param>
    /// <param name="ignoredCharacters">The characters to be ignored during DFA computation.</param>
    public DfaCalculator(
        IEnumerable<Lexeme> lexemes,
        IEnumerable<char> ignoredCharacters)
    {
        Lexemes = lexemes.ToArray();
        IgnoredCharacters = ignoredCharacters.ToArray();
    }

    /// <summary>
    /// Computes the DFA based on the provided lexemes and ignored characters.
    /// </summary>
    /// <returns>A <see cref="RegexDfa"/> representing the computed DFA.</returns>
    public RegexDfa ComputeDfa()
    {
        var processedStates = new HashSet<AutomatonState>();
        var statesToProcess = new Queue<AutomatonState>();

        /* compute the initial state */
        var initialState = ComputeInitialState();

        /* add the initial state to the queue */
        statesToProcess.Enqueue(initialState);

        while (statesToProcess.TryDequeue(out var state))
        {
            processedStates.Add(state);
            state.AddTransitions(ComputeTransitions(state, processedStates));

            var nextStates = state.Transitions
                .Select(x => x.NextState)
                .ToArray();

            foreach (var nextState in nextStates)
            {
                if (processedStates.Contains(nextState))
                {
                    continue;
                }

                statesToProcess.Enqueue(nextState);
            }
        }

        /* add transitions to the initial state to skip over ignored characters */
        var ignoredTransitions = IgnoredCharacters
            .Select(x => new AutomatonTransition(x, initialState))
            .ToArray();

        initialState.AddTransitions(ignoredTransitions);

        return new RegexDfa(processedStates);
    }

    private AutomatonState ComputeInitialState()
    {
        var lexemes = Lexemes
            .Select(x => new LexemeRegex(x.Name, x.Pattern))
            .ToArray();

        return new AutomatonState(lexemes);
    }

    /// <summary>
    /// Computes the transitions for a given state.
    /// </summary>
    /// <param name="state">The current state node.</param>
    /// <param name="processedStates">The set of already processed states.</param>
    /// <returns>An array of <see cref="AutomatonTransition"/> representing the transitions from the current state.</returns>
    private AutomatonTransition[] ComputeTransitions(
        AutomatonState state,
        HashSet<AutomatonState> processedStates)
    {
        /* if the state has only one lexeme and it is an epsilon, then no transitions are needed */
        if (state.Regexes.Length == 1 && state.Regexes[0].Regex.IsEpsilon())
        {
            return Array.Empty<AutomatonTransition>();
        }

        var lexemes = state.Regexes;
        var transitions = new List<AutomatonTransition>();

        foreach (var c in state.ComputeAlphabet())
        {
            var derivatives = lexemes
                .Select(x => x.ComputeDerivative(c))
                .ToArray();

            var validDerivatives = derivatives
                .Where(x => !x.Regex.IsEmptySet())
                .ToArray();

            if (validDerivatives.Length == 0)
            {
                continue;
            }

            var nextState = new AutomatonState(validDerivatives);

            if (processedStates.TryGetValue(nextState, out var recursiveState))
            {
                var recursiveTransition = new AutomatonTransition(c, recursiveState);
                transitions.Add(recursiveTransition);
                continue;
            }

            var gotoTransition = new AutomatonTransition(c, nextState);
            transitions.Add(gotoTransition);
        }

        return transitions.ToArray();
    }

    /// <summary>
    /// Computes the derivative of a regex with respect to a character.
    /// </summary>
    /// <param name="regex">The regex to compute the derivative for.</param>
    /// <param name="c">The character to derive with respect to.</param>
    /// <returns>A <see cref="RegExpr"/> representing the derivative of the regex.</returns>
    private RegExpr ComputeDerivative(
        RegExpr regex,
        char c)
    {
        var calculator = new RegexDerivativeCalculator();

        var derivative = calculator
            .Derive(regex, c);

        return derivative;
    }

}
