using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;
using Aidan.TextAnalysis.RegularExpressions.Automata.Extensions;
using Aidan.TextAnalysis.Tokenization.Experimental.RegexTokenization;
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
        var builder = new TokenizerDfaBuilder("q0");
        var dfa = DfaCalculator.ComputeDfa();
            
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
        var stateNameMap = new Dictionary<AutomatonNode, string>();

        for (int i = 0; i < dfa.States.Count; i++)
        {
            var state = dfa.States[i];
            var isAccepting = state.IsEpsilonState();
            var name = isAccepting
                ? state.Regexes[0].Name
                : $"q{i}";
            stateNameMap[state] = name;
        }

        foreach (var state in dfa.States)
        {
            if(state.IsEpsilonState())
            {
                var epsilonRegex = state.Regexes[0];

                builder.CreateState(epsilonRegex.Name, true);
                continue;
            }

            var currentStateName = stateNameMap[state];

            builder.CreateState(currentStateName, false);

            //foreach (var transition in state.Transitions)
            //{
            //    var nextStateName = stateNameMap[transition.NextState];

            //    builder.AddTransition(
            //        currentState: currentStateName,
            //        character: transition.Character,
            //        nextState: nextStateName);
            //}

            if(!state.CanProduceEpsilon())
            {
                continue;
            }

            var firstEpsilonRegex = state.Regexes
                .First(x => x.Regex.ContainsEpsilon);

            //var gotoCharacters = state.Transitions
            //    .Select(x => x.Character)
            //    .ToArray();

            //var leftoverCharacters = dfa.Alphabet
            //    .Except(gotoCharacters)
            //    .ToArray();

            var epsilonStateName = firstEpsilonRegex.Name;

            builder.CreateState(epsilonStateName, true);

            //foreach (var c in leftoverCharacters)
            //{
            //    builder.AddTransition(
            //        currentState: currentStateName,
            //        character: c,
            //        nextState: epsilonStateName);
            //}
        }
    }

    private void CreateTransitions(RegexDfa dfa, TokenizerDfaBuilder builder)
    {
        var stateNameMap = new Dictionary<AutomatonNode, string>();

        for (int i = 0; i < dfa.States.Count; i++)
        {
            var state = dfa.States[i];
            var isAccepting = state.IsEpsilonState();
            var name = isAccepting
                ? state.Regexes[0].Name
                : $"q{i}";
            stateNameMap[state] = name;
        }

        foreach (var state in dfa.States)
        {
            if (state.IsEpsilonState())
            {
                continue;
            }

            var currentStateName = stateNameMap[state];

            foreach (var transition in state.Transitions)
            {
                var isNextStateEpsilon = transition.NextState.IsEpsilonState();
                var nextStateName = stateNameMap[transition.NextState];

                if(isNextStateEpsilon)
                {
                    var intermediaryStateName = $"{currentStateName}_{transition.Character}";

                    builder.CreateState(intermediaryStateName, false);

                    builder.AddTransition(
                        currentState: currentStateName,
                        character: transition.Character,
                        nextState: intermediaryStateName);

                    foreach (var c in dfa.Alphabet)
                    {
                        builder.AddTransition(
                            currentState: intermediaryStateName,
                            character: c,
                            nextState: nextStateName);
                    }
                }   
                else
                {
                    builder.AddTransition(
                        currentState: currentStateName,
                        character: transition.Character,
                        nextState: nextStateName);
                }
            }

            if (!state.CanProduceEpsilon())
            {
                continue;
            }

            var firstEpsilonRegex = state.Regexes
                .First(x => x.Regex.ContainsEpsilon);

            var gotoCharacters = state.Transitions
                .Select(x => x.Character)
                .ToArray();

            var leftoverCharacters = dfa.Alphabet
                .Except(gotoCharacters)
                .ToArray();

            var epsilonStateName = firstEpsilonRegex.Name;

            foreach (var c in leftoverCharacters)
            {
                builder.AddTransition(
                    currentState: currentStateName,
                    character: c,
                    nextState: epsilonStateName);
            }
        }
    }

}
