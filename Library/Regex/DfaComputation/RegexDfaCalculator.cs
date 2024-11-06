using Aidan.TextAnalysis.Regexes.Ast;
using Aidan.TextAnalysis.Regexes.Derivation;

namespace Aidan.TextAnalysis.Regexes.DfaComputation;

/* dfa stuff */
public class Dfa
{

}

public class DfaState : IEquatable<DfaState>
{
    public IRegexNode Node { get; }
    public bool IsFinal { get; set; }

    public DfaState(IRegexNode node)
    {
        Node = node;
        IsFinal = node.Type == RegexNodeType.Epsilon;
    }

    public bool Equals(DfaState other) => other.Node.Equals(Node);

    public override string ToString()
    {
        return Node.ToString();
    }
}

public class DfaStateTransition
{
    public DfaState From { get; }
    public DfaState To { get; }
    public char Symbol { get; }

    public DfaStateTransition(DfaState from, DfaState to, char symbol)
    {
        From = from;
        To = to;
        Symbol = symbol;
    }

    public override string ToString()
    {
        return $"{From} --{Symbol}--> {To}";
    }
}

public class RegexDfaCalculator
{
    private IRegexNode Source { get; }

    public RegexDfaCalculator(IRegexNode source)
    {
        Source = source;
    }

    public Dfa ComputeDfa()
    {
        var states = new List<DfaState>();
        var transitions = new List<DfaStateTransition>();
        var processedStates = new HashSet<DfaState>();

        var initialState = ComputeInitialState();
        var alphabet = GetAlphabet();

        states.Add(initialState);

        while (true)
        {
            var newStatesCounter = 0;
            var statesToProcess = states
                .Except(processedStates)
                .ToArray();

            foreach (var state in statesToProcess)
            {
                if (processedStates.Any(x => x.Equals(state)))
                {
                    continue;
                }

                /* mark the state as processed */
                processedStates.Add(state);

                if (state.IsFinal)
                {
                    //continue;
                }

                foreach (var c in alphabet)
                {
                    var derivative = ComputeDerivativeStates(state, c);

                    if (states.Any(x => x.Equals(derivative)))
                    {
                        continue;
                    }

                    var transition = new DfaStateTransition(state, derivative, c);

                    states.Add(derivative);
                    transitions.Add(transition);
                    newStatesCounter++;
                }
            }

            if (newStatesCounter == 0)
            {
                break;
            }
        }

        return new Dfa();
    }

    private DfaState ComputeInitialState()
    {
        return new DfaState(node: Source);
    }

    private int Counter { get; set; }

    private DfaState ComputeDerivativeStates(DfaState state, char c)
    {
        var calculator = new RegexDerivativeCalculator();

        var derivative = calculator
            .Derive(state.Node, c);

        var history = calculator.History.ToString();
        Counter++;
        //var simplified = derivative
        //    .Simplify();

        var phrase = $"`{state}` with respect to '{c}' = `{derivative}` is it correct ?";

        return new DfaState(
            node: derivative);
    }

    private char[] GetAlphabet()
    {
        var alphabet = new HashSet<char>();

        var stack = new Stack<IRegexNode>();
        stack.Push(Source);

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            switch (node)
            {
                case ILiteralNode literal:
                    alphabet.Add(literal.Literal);
                    break;
                case IConcatenationNode concatenation:
                    stack.Push(concatenation.Left);
                    stack.Push(concatenation.Right);
                    break;
                case IUnionNode union:
                    stack.Push(union.Left);
                    stack.Push(union.Right);
                    break;
                case IStarNode kleeneStar:
                    stack.Push(kleeneStar.Child);
                    break;
            }
        }

        return alphabet
            .Reverse()
            .ToArray();
    }

}