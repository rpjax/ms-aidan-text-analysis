using Aidan.Core.Extensions;
using Aidan.Core.Patterns;
using Aidan.TextAnalysis.Regexes.Ast;
using Aidan.TextAnalysis.Regexes.Ast.Extensions;
using Aidan.TextAnalysis.Regexes.Derivative;
using System.Text.RegularExpressions;

namespace Aidan.TextAnalysis.Regexes.DfaComputation;

/* regex dfa structures */

public class RegexDfa
{
    public IReadOnlyList<RegexDfaState> States { get; }
    public IReadOnlyList<char> Alphabet { get; }

    public RegexDfa(IEnumerable<RegexDfaState> states, IReadOnlyList<char> alphabet)
    {
        States = states.ToArray();
        Alphabet = alphabet;
    }
}

public class RegexDfaState : IEquatable<RegexDfaState>
{
    public string Name { get; }
    public RegexNode Regex { get; }
    public IReadOnlyList<RegexDfaTransition> Transitions { get; }
    public bool IsAccepting { get; }
    public RegexLexeme? Lexeme { get; }

    public RegexDfaState(
        string name,
        RegexNode regex,
        IEnumerable<RegexDfaTransition> transitions)
    {
        Name = name;
        Regex = regex;
        Transitions = transitions.ToArray();
        IsAccepting = Regex.IsEpsilon();
        Lexeme = Regex.GetLexeme();
    }

    public static RegexDfaStateBuilder Create() => new RegexDfaStateBuilder();

    public bool Equals(RegexDfaState other)
    {
        var thisLexeme = Regex.GetLexeme();
        var otherLexeme = other.Regex.GetLexeme();
        var isLexemeEqual = thisLexeme?.Equals(otherLexeme);

        return true
            && other.Regex.Equals(Regex)
            && (isLexemeEqual == true || isLexemeEqual == null)
            ;
    }

    public override string ToString()
    {
        return $"{Name}: `{Regex}` {(IsAccepting ? "(accepting)" : "")}";
    }
}

public class RegexDfaStateBuilder : IBuilder<RegexDfaState>
{
    private string? Name { get; set; }
    private RegexNode? Pattern { get; set; }
    private List<RegexDfaTransition> Transitions { get; } = new List<RegexDfaTransition>();

    public RegexDfaStateBuilder WithName(string name)
    {
        Name = name;
        return this;
    }

    public RegexDfaStateBuilder WithPattern(RegexNode pattern)
    {
        Pattern = pattern;
        return this;
    }

    public RegexDfaStateBuilder WithTransition(RegexDfaTransition transition)
    {
        Transitions.Add(transition);
        return this;
    }

    public RegexDfaStateBuilder WithTransitions(IEnumerable<RegexDfaTransition> transitions)
    {
        Transitions.AddRange(transitions);
        return this;
    }

    public RegexDfaState Build()
    {
        return new RegexDfaState(
            name: Name ?? throw new InvalidOperationException("Name is required"),
            regex: Pattern ?? throw new InvalidOperationException("Pattern is required"),
            transitions: Transitions);
    }

}

public class RegexDfaTransition : IEquatable<RegexDfaTransition>
{
    public RegexNode Source { get; }
    public RegexNode Derivative { get; }
    public char Character { get; }

    public RegexDfaTransition(
        RegexNode source,
        RegexNode derivative,
        char character)
    {
        Source = source;
        Derivative = derivative;
        Character = character;
    }

    public override string ToString()
    {
        return $"FROM `{Source}` ON '{Character}' GOTO `{Derivative}`";
    }

    public bool Equals(RegexDfaTransition other)
    {
        return other.Source.Equals(Source) &&
            other.Derivative.Equals(Derivative) &&
            other.Character == Character;
    }
}

/* dfa 

/* calculators */

public class RegexDfaCalculator
{
    private IReadOnlyList<RegexLexeme> Lexemes { get; }
    private RegexNode Regex { get; }
    private IReadOnlyList<char> Alphabet { get; }

    public RegexDfaCalculator(
        IReadOnlyList<RegexLexeme> lexemes,
        params char[] alphabetComplement)
    {
        if(lexemes.Count == 0)
        {
            throw new ArgumentException("At least one lexeme is required");
        }

        /* inject the lexemes into the regex ast */
        foreach (var lexeme in lexemes)
        {
            lexeme.Pattern.PropagateLexeme(lexeme);
        }

        Lexemes = lexemes;

        Regex = lexemes
            .Select(x => x.Pattern)
            .Aggregate((x, y) => x.Union(y));

        Alphabet = ComputeAlphabet(Regex)
            .Concat(alphabetComplement)
            .ToArray();
    }

    private char[] ComputeAlphabet(RegexNode regex)
    {
        return regex.ComputeAlphabet();
    }

    public RegexDfa ComputeRegexDfa()
    {
        var states = new List<RegexDfaState>();
        var initialState = ComputeInitialState();
        var queue = new Queue<RegexDfaState>();

        states.Add(initialState);
        queue.Enqueue(initialState);

        while (queue.TryDequeue(out var state))
        {
            var nextStates = ComputeNextStates(state);

            foreach (var nextState in nextStates)
            {
                if (states.Any(x => x.Equals(nextState)))
                {
                    continue;
                }

                states.Add(nextState);
                queue.Enqueue(nextState);
            }
        }

        return new RegexDfa(states, Alphabet);
    }

    private RegexDfaState ComputeInitialState()
    {
        var transitions = ComputeTransitions(Regex);

        return new RegexDfaState(
            name: Regex.ToString(),
            regex: Regex,
            transitions: transitions);
    }

    private RegexDfaTransition[] ComputeTransitions(RegexNode source)
    {
        var transitions = new List<RegexDfaTransition>();

        foreach (var c in Alphabet)
        {
            var derivative = ComputeDerivative(source, c);

            /* accept condition */
            var isBranchEpsilon = !source.IsEpsilon() 
                && source.ContainsEpsilon 
                && derivative.IsEmptySet();

            if (isBranchEpsilon)
            {
                var epsilonBranches = source.GetEpsilonBranches();

                if (epsilonBranches.Length == 0)
                {
                    throw new InvalidOperationException("Epsilon branches are required");
                }
                if (epsilonBranches.Length > 1)
                {
                    throw new InvalidOperationException("Only one epsilon branch is allowed");
                }

                var epsilonBranch = epsilonBranches[0];

                var epsilonDerivative = new EpsilonNode()
                    .PropagateLexeme(source: epsilonBranch);

                var epsilonTransition = new RegexDfaTransition(
                    source: source,
                    derivative: epsilonDerivative,
                    character: c);

                transitions.Add(epsilonTransition);
                continue;
            }

            /* the empty set is the sink state */
            if (derivative.IsEmptySet())
            {
                continue;
            }

            if(derivative.IsEpsilon())
            {
                //Console.WriteLine();
                // this is also an accept transition
            }

            var transition = new RegexDfaTransition(
                source: source, 
                derivative: derivative, 
                character: c);

            /* it seems this isn't necessary */
            if (transitions.Any(x => x.Equals(transition)))
            {
                continue;
            }

            transitions.Add(transition);
        }

        return transitions.ToArray();
    }

    private RegexNode ComputeDerivative(RegexNode regex, char c)
    {
        var calculator = new RegexDerivativeCalculator();

        var derivative = calculator
            .Derive(regex, c);

        /* debugging */
        //var history = calculator.GetHistoryString();
        //var phrase = $"`{state}` with respect to '{c}' = `{derivative}` is it correct ?";

        return derivative;
    }

    private RegexDfaState[] ComputeNextStates(RegexDfaState state)
    {
        var transitions = state.Transitions;
        var nextStates = new List<RegexDfaState>();

        foreach (var transition in transitions)
        {
            var nextState = new RegexDfaState(
                name: $"{transition.Derivative}",
                regex: transition.Derivative,
                transitions: ComputeTransitions(transition.Derivative));

            nextStates.Add(nextState);
        }

        return nextStates.ToArray();
    }

}

