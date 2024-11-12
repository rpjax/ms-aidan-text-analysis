using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;
using Aidan.TextAnalysis.RegularExpressions.Automata.Extensions;
using Aidan.TextAnalysis.Tokenization.StateMachine;

namespace Aidan.TextAnalysis.RegularExpressions.Automata;

public class TokenizerCalculator
{
    private Lexeme[] Lexemes { get; }
    private DfaCalculator DfaCalculator { get; }
    private bool UseDebugger { get; }

    public TokenizerCalculator(
        IEnumerable<Lexeme> lexemes,
        IEnumerable<char> ignoredChars,
        bool useDebug = false)
    {
        Lexemes = lexemes.ToArray();
        DfaCalculator = new DfaCalculator(Lexemes, ignoredChars);
        UseDebugger = useDebug;
    }

    public TokenizerTable ComputeTokenizerTable()
    {
        var dfa = DfaCalculator.ComputeDfa();

        var builder = new TokenizerDfaBuilder(
            initialStateName: dfa.InitialState.Name);
            
        builder.SetCharset(dfa.Alphabet.ToArray());
        CreateStates(dfa, builder);
        CreateTransitions(dfa, builder);

        return builder.BuildTable();
    }

    public TokenizerMachine ComputeTokenizer()
    {
        return new TokenizerMachine(ComputeTokenizerTable(), UseDebugger);
    }

    public IReadOnlyList<char> GetAlphabet()
    {
        return DfaCalculator.Alphabet;
    }

    private void CreateStates(RegexDfa dfa, TokenizerDfaBuilder builder)
    {
        AutomatonNode a;
        foreach (var state in dfa.States)
        {
            var canProduceEpsilon = state.CanProduceEpsilon();
            var containsEpsilonRegex = state.ContainsEpsilonRegex();

            builder.CreateState(state.Name, state.IsAccepting);
        }
    }

    private void CreateTransitions(Dfa dfa, TokenizerDfaBuilder builder)
    {
        foreach (var state in dfa.States)
        {
            foreach (var transition in state.Transitions)
            {
                builder.AddTransition(state.Name, transition.Character, transition.NextState);
            }
        }
    }

}
