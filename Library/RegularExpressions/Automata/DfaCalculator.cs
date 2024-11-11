using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;
using Aidan.TextAnalysis.RegularExpressions.Derivative;
using Aidan.TextAnalysis.Tokenization.StateMachine;
using System.Text.RegularExpressions;

namespace Aidan.TextAnalysis.RegularExpressions.Automata;

/// <summary>
/// Class responsible for calculating the DFA (Deterministic Finite Automaton) from a given set of lexemes and an optional alphabet complement.
/// </summary>
public class DfaCalculator
{
    /* dependencies */
    private IReadOnlyList<Lexeme> Lexemes { get; }
    private RegexNode Regex { get; }
    private IReadOnlyList<char> Alphabet { get; }
    private IReadOnlyList<char> IgnoredCharacters { get; }

    /* computation data */
    private Dictionary<string, string> StateIdMap { get; } = new Dictionary<string, string>();
    private Dictionary<string, DfaState> States { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DfaCalculator"/> class.
    /// </summary>
    /// <param name="lexemes">The list of lexemes to be used in the DFA calculation.</param>
    /// <param name="ignoredCharacters">Optional characters to be added to the alphabet.</param>
    /// <exception cref="ArgumentException">Thrown when no lexemes are provided.</exception>
    public DfaCalculator(
        IEnumerable<Lexeme> lexemes,
        IEnumerable<char> ignoredCharacters)
    {
        Lexemes = lexemes.ToArray();

        if (Lexemes.Count == 0)
        {
            throw new ArgumentException("At least one lexeme is required");
        }

        /* inject the lexemes into the regex ast */
        foreach (var lexeme in lexemes)
        {
            lexeme.Pattern.PropagateLexeme(lexeme);
        }

        Regex = lexemes
            .Select(x => x.Pattern)
            .Aggregate((x, y) => x.Union(y));

        Alphabet = Regex.ComputeAlphabet()
            .Concat(ignoredCharacters)
            .ToArray();

        IgnoredCharacters = ignoredCharacters.ToArray();
    }

    /// <summary>
    /// Computes the DFA from the regex.
    /// </summary>
    /// <returns>The computed DFA.</returns>
    public Dfa ComputeDfa()
    {
        var states = new List<DfaState>();
        var statesToProcess = new Queue<DfaState>();

        var initialState = ComputeInitialState();

        states.Add(initialState);
        statesToProcess.Enqueue(initialState);

        while (statesToProcess.TryDequeue(out var state))
        {
            var nextStates = ComputeNextStates(state);

            foreach (var nextState in nextStates)
            {
                if (nextState.Transitions.Any(x => x.NextState == "q28"))
                {
                    Console.WriteLine();
                }

                if (states.Any(x => x.Equals(nextState)))
                {
                    continue;
                }

                states.Add(nextState);
                statesToProcess.Enqueue(nextState);
            }
        }

        return new Dfa(states, Alphabet);
    }

    /// <summary>
    /// Gets the alphabet of the DFA.
    /// </summary>
    /// <returns>The alphabet of the DFA.</returns>
    public char[] GetAlphabet()
    {
        return Alphabet.ToArray();
    }

    /*
     * refactor
     */

    private void CreateInitialState()
    {
        var initialState = ComputeInitialState();
        States.Add(initialState.Name, initialState);
    }

    /*
     */

    private DfaState ComputeInitialState()
    {
        return new DfaState(
            name: ComputeStateName(Regex),
            regex: Regex,
            transitions: ComputeTransitions(Regex));
    }

    private string ComputeStateName(RegexNode regex)
    {
        Lexeme? lexeme = regex.GetLexeme();
        string regexString = regex.ToString();
        string? lexemeName = lexeme?.Name;

        string key = regexString + lexemeName;
        string? id = null;

        if (StateIdMap.TryGetValue(key, out id))
        {
            return id;
        }
        else
        {
            id = $"q{StateIdMap.Count}";
            StateIdMap.Add(key, id);
            return id;
        }
    }

    private DfaTransition[] ComputeTransitions(RegexNode source)
    {
        var transitions = new List<DfaTransition>();

        foreach (var c in Alphabet)
        {
            var transition = ComputeTransition(source, c);

            if (transition is null)
            {
                continue;
            }
            if (transition.NextState == "q28")
            {
                Console.WriteLine();
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

    private DfaTransition? ComputeTransition(RegexNode source, char c)
    {
        var derivative = ComputeDerivative(source, c);

        var isInitialState = source.Equals(Regex);

        /* skip character transition */
        var isSkipCharTransition = isInitialState
            && derivative.IsEmptySet()
            && IgnoredCharacters.Contains(c);

        /* accept condition */
        var isAcceptFromEpsilonBranch = !source.IsEpsilon()
            && source.ContainsEpsilon
            && derivative.IsEmptySet();

        /* cannot accept from the initial state because it is not a lexeme */
        if (isSkipCharTransition)
        {
            var skipCharTransition = new DfaTransition(
                character: c,
                nextState: ComputeStateName(source),
                derivative: source);

            return skipCharTransition;
        }

        if (isAcceptFromEpsilonBranch)
        {
            RegexNode epsilonBranch;

            switch (source.Type)
            {
                case RegexNodeType.Union:
                    var union = source.AsUnion();

                    if (!union.Left.ContainsEpsilon && !union.Right.ContainsEpsilon)
                    {
                        throw new InvalidOperationException("Union branches do not contain epsilon");
                    }

                    if (union.Left.IsEpsilon() && union.Right.IsEpsilon())
                    {
                        throw new InvalidOperationException("Both union branches are epsilon");
                    }

                    if (union.Left.IsEpsilon())
                    {
                        epsilonBranch = union.Left;
                    }
                    else if (union.Right.IsEpsilon())
                    {
                        epsilonBranch = union.Right;
                    }
                    else if (union.Left.ContainsEpsilon)
                    {
                        epsilonBranch = union.Left;
                    }
                    else
                    {
                        epsilonBranch = union.Right;
                    }

                    break;

                case RegexNodeType.Star:
                    var start = source.AsStar();

                    epsilonBranch = start;

                    break;

                default:
                    throw new InvalidOperationException("Invalid epsilon branch");
            }

            var epsilonDerivative = new EpsilonNode()
                .PropagateLexeme(source: epsilonBranch);

            var lexeme = epsilonDerivative.GetLexeme();

            if (lexeme is null)
            {
                throw new InvalidOperationException("Lexeme is null");
            }

            var epsilonTransition = new DfaTransition(
                character: c,
                nextState: ComputeStateName(epsilonDerivative),
                derivative: epsilonDerivative);

            return epsilonTransition;
        }

        /* the empty set is the sink state */
        if (derivative.IsEmptySet())
        {
            return null;
        }

        var transition = new DfaTransition(
            character: c,
            nextState: ComputeStateName(derivative),
            derivative: derivative);

        return transition;
    }

    private DfaState[] ComputeNextStates(DfaState state)
    {
        var transitions = state.Transitions;
        var nextStates = new List<DfaState>();

        foreach (var transition in transitions)
        {
            var nextState = new DfaState(
                name: ComputeStateName(transition.Derivative),
                regex: transition.Derivative,
                transitions: ComputeTransitions(transition.Derivative));

            if (nextState.Name == "q28")
            {
                Console.WriteLine();
            }

            nextStates.Add(nextState);
        }

        return nextStates.ToArray();
    }

}


public class DfaCalculatorRefactor
{
    private Lexeme[] Lexemes { get; }
    private IReadOnlyList<char> Alphabet { get; }
    private IReadOnlyList<char> IgnoredCharacters { get; }

    public DfaCalculatorRefactor(
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

    public void ComputeDfa()
    {
        var lexemes = Lexemes
            .Select(x => new LexemeDerivative(x.Name, x.Pattern))
            .ToArray();

        var initialState = new BranchNode(lexemes);

        var transitions = ComputeTransitions(initialState);

    }

    private Dictionary<char, BranchNode> ComputeTransitions(BranchNode node)
    {
        var lexemes = node.Lexemes;
        var transitions = new Dictionary<char, BranchNode>();

        foreach (var c in Alphabet)
        {
            var derivatives = lexemes
                .Select(x => new LexemeDerivative(x.Name, ComputeDerivative(x.Derivative, c)))
                .ToArray();

            var validDerivatives = derivatives
                .Where(x => !x.Derivative.IsEmptySet())
                .ToArray();

            transitions.Add(c, new BranchNode(validDerivatives));
        }

        return transitions;
    }

    private RegexNode ComputeDerivative(RegexNode regex, char c)
    {
        var calculator = new RegexDerivativeCalculator();

        var derivative = calculator
            .Derive(regex, c);

        return derivative;
    }

}

public class LexemeDerivative
{
    public string Name { get; }
    public RegexNode Derivative { get; }

    public LexemeDerivative(string name, RegexNode derivative)
    {
        Name = name;
        Derivative = derivative;
    }

}

public class BranchNode
{
    public LexemeDerivative[] Lexemes { get; }
    public Dictionary<char, BranchNode> Transitions { get; }

    public BranchNode(
        LexemeDerivative[] lexemes,
        Dictionary<char, BranchNode>? transitions = null)
    {
        Lexemes = lexemes;
        Transitions = transitions ?? new Dictionary<char, BranchNode>();
    }

    public void SetTransition(char c, BranchNode node)
    {
        Transitions.Add(c, node);
    }

    public void SetTransitions(Dictionary<char, BranchNode> transitions)
    {
        foreach (var transition in transitions)
        {
            Transitions.Add(transition.Key, transition.Value);
        }
    }

}