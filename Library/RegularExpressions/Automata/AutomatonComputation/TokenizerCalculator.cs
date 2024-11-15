using Aidan.TextAnalysis.RegularExpressions.Automata.Extensions;
using Aidan.TextAnalysis.Tokenization;
using Aidan.TextAnalysis.Tokenization.StateMachine.Builders;
using Aidan.TextAnalysis.Tokenization.StateMachine.Components;

namespace Aidan.TextAnalysis.RegularExpressions.Automata;

/// <summary>
/// Responsible for calculating the tokenizer table and creating a tokenizer.
/// </summary>
public class TokenizerCalculator
{
    private Lexeme[] Lexemes { get; }
    private DfaCalculator DfaCalculator { get; }
    private bool UseDebugger { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenizerCalculator"/> class.
    /// </summary>
    /// <param name="lexemes">The lexemes to be used in the tokenizer.</param>
    /// <param name="ignoredChars">The characters to be ignored during tokenization.</param>
    /// <param name="useDebug">Indicates whether to use the debugger.</param>
    public TokenizerCalculator(
        IEnumerable<Lexeme> lexemes,
        IEnumerable<char> ignoredChars,
        bool useDebug = false)
    {
        Lexemes = lexemes.ToArray();
        DfaCalculator = new DfaCalculator(Lexemes, ignoredChars);
        UseDebugger = useDebug;
    }

    /// <summary>
    /// Computes the tokenizer table based on the provided lexemes and ignored characters.
    /// </summary>
    /// <returns>A <see cref="TokenizerTable"/> representing the tokenizer state machine.</returns>
    public TokenizerTable ComputeTokenizerTable()
    {
        var builder = new TokenizerBuilder(useDebugger: false);
        var dfa = DfaCalculator.ComputeDfa();
        var states = dfa.States.ToList();

        foreach (var state in dfa.States)
        {
            var currentStateIsAccepting = state.IsEpsilonState();
            var currentStateId = (uint)states.IndexOf(state);

            /* the accepted lexeme is the first listed in the calculator input */
            var currentStateName = currentStateIsAccepting
                ? state.GetEpsilonLexemes().First().Name
                : $"q{currentStateId}";

            var currentState = builder.CreateState(
                id: currentStateId,
                name: currentStateName,
                isAccepting: currentStateIsAccepting);

            foreach (var transition in state.Transitions)
            {
                var nextStateIsAccepting = transition.NextState.IsEpsilonState();
                var nextStateId = (uint)states.IndexOf(transition.NextState);

                builder.FromState(currentStateId)
                    .OnCharacter(transition.Character)
                    .GoTo(nextStateId);
            }
        }

        return builder.BuildTable();
    }

    /// <summary>
    /// Computes the tokenizer based on the tokenizer table.
    /// </summary>
    /// <returns>A <see cref="Tokenizer"/> instance.</returns>
    public Tokenizer ComputeTokenizer()
    {
        return new Tokenizer(ComputeTokenizerTable(), UseDebugger);
    }

    /// <summary>
    /// Gets the alphabet used by the DFA calculator.
    /// </summary>
    /// <returns>A read-only list of characters representing the alphabet.</returns>
    public IReadOnlyList<char> GetAlphabet()
    {
        return DfaCalculator.Alphabet;
    }
}
