using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;
using Aidan.TextAnalysis.RegularExpressions.Derivative;
using Aidan.TextAnalysis.Tokenization.StateMachine;
using System.Text.RegularExpressions;

namespace Aidan.TextAnalysis.RegularExpressions.Automata;

public class DfaCalculator
{
    public Lexeme[] Lexemes { get; }
    public IReadOnlyList<char> Alphabet { get; }
    public IReadOnlyList<char> IgnoredCharacters { get; }

    public DfaCalculator(
        IEnumerable<Lexeme> lexemes,
        IEnumerable<char> ignoredCharacters)
    {
        Lexemes = lexemes.ToArray();

        Alphabet = lexemes
            .SelectMany(x => x.Pattern.ComputeAlphabet())
            .Concat(ignoredCharacters)
            .Distinct()
            .ToArray();

        IgnoredCharacters = ignoredCharacters.ToArray();
    }

    public RegexDfa ComputeDfa()
    {
        var lexemes = Lexemes
            .Select(x => new LexemeRegex(x.Name, x.Pattern))
            .ToArray();

        var processedStates = new HashSet<AutomatonNode>();
        var statesToProcess = new Queue<AutomatonNode>();

        /* compute the initial state */
        var initialState = new AutomatonNode(lexemes);

        /* add the initial state to the queue */
        statesToProcess.Enqueue(initialState);

        while (statesToProcess.TryDequeue(out var state))
        {
            processedStates.Add(state);
            state.AddChildren(ComputeChildren(state));

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

        /* dfa building */
        var states = new List<DfaState>();
        var stateIdMap = new Dictionary<AutomatonNode, int>();

        for (int i = 0; i < processedStates.Count; i++)
        {
            stateIdMap.Add(processedStates.ElementAt(i), i);
        }

        for (int i = 0; i < processedStates.Count; i++)
        {
            var node = processedStates.ElementAt(i);

            var transitions = new List<DfaTransition>();

            foreach (var item in node.Transitions)
            {
                var nextStateIsAccepting = item.NextState.Regexes.Length == 1
                    && item.NextState.Regexes[0].Regex.IsEpsilon();

                var nextState = nextStateIsAccepting
                    ? item.NextState.Regexes[0].Name
                    : $"q{stateIdMap[item.NextState]}";

                var transition = new DfaTransition(
                    character: item.Character,
                    nextState: nextState);

                transitions.Add(transition);
            }

            var isAccepting = node.Regexes.Length == 1
                && node.Regexes[0].Regex.IsEpsilon();

            if (isAccepting && node.Regexes.Length != 1)
            {
                throw new InvalidOperationException("Invalid accepting state");
            }

            var name = isAccepting
                ? node.Regexes[0].Name
                : $"q{i}";

            var dfaState = new DfaState(
                name: name,
                transitions: transitions,
                isAccepting: isAccepting);

            states.Add(dfaState);
        }

        return new Dfa(states, Alphabet);
    }

    private AutomatonTransition[] ComputeChildren(AutomatonNode node)
    {
        /* if the state has only one lexeme and it is an epsilon, then no transitions are needed */
        if (node.Regexes.Length == 1 && node.Regexes[0].Regex.IsEpsilon())
        {
            return Array.Empty<AutomatonTransition>();
        }

        var lexemes = node.Regexes;
        var transitions = new List<AutomatonTransition>();

        foreach (var c in Alphabet)
        {
            var derivatives = lexemes
                .Select(x => new LexemeRegex(x.Name, ComputeDerivative(x.Regex, c)))
                .ToArray();

            var validDerivatives = derivatives
                .Where(x => !x.Regex.IsEmptySet())
                .ToArray();

            if (validDerivatives.Length == 0)
            {
                continue;
            }

            var nextState = new AutomatonNode(validDerivatives);
            var recursiveState = node.FindRecursiveState(nextState);

            if (recursiveState is not null)
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

    private RegexNode ComputeDerivative(RegexNode regex, char c)
    {
        var calculator = new RegexDerivativeCalculator();

        var derivative = calculator
            .Derive(regex, c);

        return derivative;
    }

}
