using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;
using Aidan.TextAnalysis.Tokenization.StateMachine;

namespace Aidan.TextAnalysis.RegularExpressions.Automata;

public class TokenizerCalculator
{
    private Lexeme[] Lexemes { get; }
    private DfaCalculator DfaCalculator { get; }

    public TokenizerCalculator(
        IEnumerable<Lexeme> lexemes,
        IEnumerable<char> ignoredChars)
    {
        Lexemes = lexemes.ToArray();
        DfaCalculator = new DfaCalculator(Lexemes, ignoredChars);
    }

    public TokenizerTable ComputeTokenizerTable()
    {
        var dfa = DfaCalculator.ComputeDfa();

        var initialState = dfa.States
            .First();

        var builder = new TokenizerDfaBuilder(initialState.Name);

        builder.SetCharset(dfa.Alphabet.ToArray());

        foreach (var state in dfa.States)
        {
            foreach (var transition in state.Transitions)
            {
                var derivativeIsAccepting = transition.Derivative.IsEpsilon();
                var sourceContainsEpsilon = state.Regex.ContainsEpsilon;
                var stateName = transition.Derivative.ToString();

                if (derivativeIsAccepting)
                {
                    var lexeme = transition.Derivative.GetLexeme();

                    if (lexeme is null)
                    {
                        throw new InvalidOperationException("Lexeme is null");
                    }

                    if (sourceContainsEpsilon)
                    {
                        builder
                            .FromState(state.Name)
                            .OnCharacter(transition.Character)
                            .Accept(lexeme.Name)
                            ;
                    }
                    else
                    {
                        var intermediateName = Guid.NewGuid().ToString();

                        builder
                            .FromState(state.Name)
                            .OnCharacter(transition.Character)
                            .GoTo(intermediateName)
                            ;

                        builder
                            .FromState(intermediateName)
                            .OnAnyCharacter()
                            .Accept(lexeme.Name)
                            ;
                    }
                }
                else
                {
                    builder
                        .FromState(state.Name)
                        .OnCharacter(transition.Character)
                        .GoTo(transition.NextState);
                }

            }
        }

        return builder.BuildTable();
    }

    public TokenizerMachine ComputeTokenizer()
    {
        return new TokenizerMachine(ComputeTokenizerTable());
    }

    public char[] GetAlphabet()
    {
        return DfaCalculator.GetAlphabet();
    }

}
