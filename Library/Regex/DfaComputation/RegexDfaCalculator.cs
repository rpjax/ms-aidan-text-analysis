using Aidan.Core.Extensions;
using Aidan.Core.Patterns;
using Aidan.TextAnalysis.Regexes.Ast;
using Aidan.TextAnalysis.Regexes.Ast.Extensions;
using Aidan.TextAnalysis.Regexes.Derivative;
using Aidan.TextAnalysis.Tokenization.StateMachine;

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
    public IRegexNode Regex { get; }
    public IReadOnlyList<RegexDfaTransition> Transitions { get; }
    public bool IsAccepting { get; }

    public RegexDfaState(
        string name,
        IRegexNode regex,
        IEnumerable<RegexDfaTransition> transitions)
    {
        Name = name;
        Regex = regex;
        Transitions = transitions.ToArray();
        IsAccepting = Regex.ContainsEpsilon;
    }

    public static RegexDfaStateBuilder Create() => new RegexDfaStateBuilder();

    public bool Equals(RegexDfaState other)
    {
        return other.Regex.Equals(Regex);
    }

    public override string ToString()
    {
        return $"{Name}: `{Regex}` {(IsAccepting ? "(accepting)" : "")}";
    }
}

public class RegexDfaStateBuilder : IBuilder<RegexDfaState>
{
    private string? Name { get; set; }
    private IRegexNode? Pattern { get; set; }
    private List<RegexDfaTransition> Transitions { get; } = new List<RegexDfaTransition>();

    public RegexDfaStateBuilder WithName(string name)
    {
        Name = name;
        return this;
    }

    public RegexDfaStateBuilder WithPattern(IRegexNode pattern)
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

public class RegexDfaTransition
{
    public IRegexNode Source { get; }
    public IRegexNode Derivative { get; }
    public char Symbol { get; }

    public RegexDfaTransition(
        IRegexNode source,
        IRegexNode derivative,
        char symbol)
    {
        Source = source;
        Derivative = derivative;
        Symbol = symbol;
    }

    public override string ToString()
    {
        return $"FROM `{Source}` ON '{Symbol}' GOTO `{Derivative}`";
    }
}

/* dfa 

/* calculators */

public class RegexDfaCalculator
{
    private IReadOnlyList<RegexLexeme> Lexemes { get; }
    private IRegexNode Regex { get; }
    private IReadOnlyList<char> Alphabet { get; }

    public RegexDfaCalculator(
        IReadOnlyList<RegexLexeme> lexemes,
        params char[] alphabetComplement)
    {
        if(lexemes.Count == 0)
        {
            throw new ArgumentException("At least one lexeme is required");
        }

        Lexemes = lexemes;

        Regex = lexemes
            .Select(x => x.Pattern)
            .Aggregate((x, y) => x.Union(y));

        Alphabet = ComputeAlphabet(Regex)
            .Concat(alphabetComplement)
            .ToArray();
    }

    private char[] ComputeAlphabet(IRegexNode regex)
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

    private RegexDfaTransition[] ComputeTransitions(IRegexNode source)
    {
        var transitions = new List<RegexDfaTransition>();
        var regexes = new List<IRegexNode>();
        var processedRegexes = new HashSet<IRegexNode>();

        regexes.Add(source);

        while (true)
        {
            var newTransitionCounter = 0;
            var regexesToProcess = regexes
                .Except(processedRegexes)
                .ToArray();

            foreach (var regex in regexesToProcess)
            {
                /* mark the state as processed */
                processedRegexes.Add(regex);

                foreach (var c in Alphabet)
                {
                    var derivative = ComputeDerivative(regex, c);

                    /* the empty set is the sink state */
                    if (derivative.IsEmptySet())
                    {
                        continue;
                    }

                    /* prevents duplicates */
                    if (!derivative.IsEpsilon() && regexes.Any(x => x.Equals(derivative)))
                    {
                        continue;
                    }

                    var transition = new RegexDfaTransition(regex, derivative, c);

                    transitions.Add(transition);
                    newTransitionCounter++;
                }
            }

            if (newTransitionCounter == 0)
            {
                break;
            }
        }

        foreach (var regex in regexes)
        {
            if (regex.IsEpsilon() || !regex.ContainsEpsilon)
            {
                continue;
            }

            var regexTransitions = transitions
                    .Where(x => x.Source.Equals(regex))
                    .ToArray();

            var transitionChars = regexTransitions
                .Select(x => x.Symbol)
                .ToArray();

            foreach (var c in Alphabet.Except(transitionChars))
            {
                var transition = new RegexDfaTransition(regex, new EpsilonNode(), c);
                transitions.Add(transition);
            }
        }
        return transitions.ToArray();
    }

    private IRegexNode ComputeDerivative(IRegexNode regex, char c)
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

/*
 */

public class TokenizerCalculator
{
    private RegexLexeme[] Lexemes { get; }
    private RegexDfaCalculator DfaCalculator { get; }

    public TokenizerCalculator(params RegexLexeme[] lexemes)
    {
        Lexemes = lexemes.ToArray();
        DfaCalculator = new RegexDfaCalculator(Lexemes, ' ', '\0');
    }

    public static void Test()
    {
        var fooRegex = new ConcatenationNode(
            new LiteralNode('f'),
            new ConcatenationNode(
                new LiteralNode('o'),
                new LiteralNode('o')
            )
        );

        var foobarRegex = new ConcatenationNode(
            new LiteralNode('f'),
            new ConcatenationNode(
                new LiteralNode('o'),
                new ConcatenationNode(
                    new LiteralNode('o'),
                    new ConcatenationNode(
                        new LiteralNode('b'),
                        new ConcatenationNode(
                            new LiteralNode('a'),
                            new LiteralNode('r')
                        )
                    )
                )
            )
        );

        var useRegex = new ConcatenationNode(
            new LiteralNode('u'),
            new ConcatenationNode(
                new LiteralNode('s'),
                new LiteralNode('e')
            )
        );

        var lexemes = new[]
        {
            new RegexLexeme("foo_statement", fooRegex),
            new RegexLexeme("foobar_statement", foobarRegex),
            new RegexLexeme("use_statement", useRegex)
        };

        var tokenizer = new TokenizerCalculator(lexemes)
            .CreateTokenizer();

        var tokens = tokenizer
            .Tokenize(" foo foo foobar foo foobar use")
            .ToArray();
    }

    public TokenizerMachine CreateTokenizer()
    {
        var dfa = DfaCalculator.ComputeRegexDfa();
        var builder = new TokenizerDfaBuilder(dfa.States.First().Name);

        builder.SetCharset(dfa.Alphabet.ToArray());

        builder.FromInitialState()
            .OnWhitespace()
            .OnEoi()
            .Recurse();

        foreach (var state in dfa.States)
        {
            foreach (var transition in state.Transitions)
            {
                var derivativeIsAccepting = transition.Derivative.IsEpsilon();
                var sourceContainsEpsilon = transition.Source.ContainsEpsilon;
                var stateName = transition.Derivative.ToString();

                if (derivativeIsAccepting)
                {
                    var acceptName = Guid.NewGuid().ToString();

                    if (sourceContainsEpsilon)
                    {
                        builder
                            .FromState(transition.Source.ToString())
                            .OnCharacter(transition.Symbol)
                            .Accept(Guid.NewGuid().ToString())
                            ;
                    }
                    else
                    {
                        var intermediateName = Guid.NewGuid().ToString();

                        builder
                            .FromState(transition.Source.ToString())
                            .OnCharacter(transition.Symbol)
                            .GoTo(intermediateName)
                            ;

                        builder
                            .FromState(intermediateName)
                            .OnAnyCharacter()
                            .Accept(acceptName)
                            ;
                    }      
                }
                else
                {
                    builder
                        .FromState(transition.Source.ToString())
                        .OnCharacter(transition.Symbol)
                        .GoTo(transition.Derivative.ToString());
                }
                
            }
        }

        return builder.Build();
    }

}

