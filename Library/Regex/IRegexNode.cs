using System.Globalization;

namespace Aidan.TextAnalysis.Regexes;

public enum RegexNodeType
{
    Epsilon,
    EmptySet,
    Literal,
    Union,
    Concatenation,
    Star,
}

public interface IRegexNode : IEquatable<IRegexNode>
{
    RegexNodeType Type { get; }
    bool ProducesEpsilon { get; } // Can match the empty string
    IRegexNode Derive(char c); // Derives the regex node with respect to a character
    IRegexNode Simplify(); // Simplifies the regex node
    string ToString();
}

public interface ILiteralNode : IRegexNode
{
    char Literal { get; }
}

public interface IConcatenationNode : IRegexNode
{
    IRegexNode Left { get; }
    IRegexNode Right { get; }
}

public interface IUnionNode : IRegexNode
{
    IRegexNode Left { get; }
    IRegexNode Right { get; }
}

public interface IKleeneStarNode : IRegexNode
{
    IRegexNode Child { get; }
}

/* 
 * concrete classes 
 */

/* correct */
public class EpsilonNode : IRegexNode
{
    public RegexNodeType Type => RegexNodeType.Epsilon;
    public bool ProducesEpsilon => true;

    public EpsilonNode()
    {
    }

    public IRegexNode Derive(char c)
    {
        return new EmptySetNode();
    }

    public IRegexNode Simplify()
    {
        return this;
    }

    public bool Equals(IRegexNode other)
    {
        return other is EpsilonNode;
    }

    public override string ToString()
    {
        return "ε";
    }
}

/* correct */
public class EmptySetNode : IRegexNode
{
    public RegexNodeType Type => RegexNodeType.EmptySet;
    public bool ProducesEpsilon => false;

    public EmptySetNode()
    {
    }

    public IRegexNode Derive(char c)
    {
        return new EmptySetNode();
    }

    public IRegexNode Simplify()
    {
        return this;
    }

    public bool Equals(IRegexNode other)
    {
        return other is EmptySetNode;
    }

    public override string ToString()
    {
        return "∅";
    }
}

/* correct */
public class LiteralNode : ILiteralNode
{
    public RegexNodeType Type => RegexNodeType.Literal;
    public bool ProducesEpsilon => false;
    public char Literal { get; }

    public LiteralNode(char literal)
    {
        Literal = literal;
    }

    public IRegexNode Derive(char c)
    {
        return c == Literal
            ? new EpsilonNode()
            : new EmptySetNode()
            ;
    }
    public IRegexNode Simplify()
    {
        return this;
    }

    public bool Equals(IRegexNode other)
    {
        return other is ILiteralNode literal
            && literal.Literal == Literal;
    }

    public override string ToString()
    {
        return Literal.ToString(CultureInfo.InvariantCulture);
    }
}

public class UnionNode : IUnionNode
{
    public RegexNodeType Type => RegexNodeType.Union;
    public bool ProducesEpsilon => Left.ProducesEpsilon || Right.ProducesEpsilon;
    public IRegexNode Left { get; }
    public IRegexNode Right { get; }

    public UnionNode(IRegexNode left, IRegexNode right)
    {
        Left = left;
        Right = right;
    }

    public IRegexNode Derive(char c)
    {
        var leftDeriv = Left.Derive(c);
        var rightDeriv = Right.Derive(c);

        if (leftDeriv is EmptySetNode)
        {
            return rightDeriv;
        }
        if (rightDeriv is EmptySetNode)
        {
            return leftDeriv;
        }

        return new UnionNode(leftDeriv, rightDeriv);
    }

    public IRegexNode Simplify()
    {
        var left = Left.Simplify();
        var right = Right.Simplify();

        // Union with NullNode simplifies to the other node
        if (left.Type == RegexNodeType.EmptySet)
        {
            return right;
        }
        if (right.Type == RegexNodeType.EmptySet)
        {
            return left;
        }

        if (Left.ProducesEpsilon && Right.Type == RegexNodeType.Epsilon)
        {
            return Left;
        }
        if (Right.ProducesEpsilon && Left.Type == RegexNodeType.Epsilon)
        {
            return Right;
        }

        return new UnionNode(left, right);
    }

    public bool Equals(IRegexNode other)
    {
        return other is IUnionNode union
            && union.Left.Equals(Left)
            && union.Right.Equals(Right);
    }

    public override string ToString()
    {
        return $"{Left}|{Right}";
    }
}

public class ConcatenationNode : IConcatenationNode
{
    public RegexNodeType Type => RegexNodeType.Concatenation;
    public bool ProducesEpsilon => Left.ProducesEpsilon && Right.ProducesEpsilon;
    public IRegexNode Left { get; }
    public IRegexNode Right { get; }

    public ConcatenationNode(IRegexNode left, IRegexNode right)
    {
        Left = left;
        Right = right;
    }

    public IRegexNode Derive(char c)
    {
        var leftDerivative = Left.Derive(c);

        // Case 1: If left derivative is the empty set, the entire concatenation fails
        if (leftDerivative is EmptySetNode)
        {
            //return new EmptySetNode();
        }
        if (leftDerivative is EpsilonNode)
        {
            //return new EmptySetNode();
        }

        if (Left.ProducesEpsilon)
        {
            var rightDerivative = Right.Derive(c);

            return new UnionNode(
                new ConcatenationNode(leftDerivative, Right),
                rightDerivative
            );
        }
        else
        {
            return new ConcatenationNode(leftDerivative, Right);
        }
    }

    public IRegexNode Simplify()
    {
        var left = Left.Simplify();
        var right = Right.Simplify();

        var anyIsEmptySet = false
            || left.Type == RegexNodeType.EmptySet
            || right.Type == RegexNodeType.EmptySet;

        /* the concatenation of an empty set with any node is an empty set */
        if (anyIsEmptySet)
        {
            return new EmptySetNode();
        }

        if (left.Type == RegexNodeType.Epsilon)
        {
            return right;
        }
        if (right.Type == RegexNodeType.Epsilon)
        {
            return left;
        }

        return new ConcatenationNode(left, right);
    }

    public bool Equals(IRegexNode other)
    {
        return other is IConcatenationNode concatenation
            && concatenation.Left.Equals(Left)
            && concatenation.Right.Equals(Right);
    }

    public override string ToString()
    {
        return $"{Left}{Right}";
    }
}

public class StarNode : IKleeneStarNode
{
    public RegexNodeType Type => RegexNodeType.Star;
    public bool ProducesEpsilon => true;
    public IRegexNode Child { get; }

    public StarNode(IRegexNode child)
    {
        Child = child;
    }

    public IRegexNode Derive(char c)
    {
        var derivative = Child.Derive(c);

        if (derivative is EmptySetNode)
        {
            return new EmptySetNode();
        }

        return new ConcatenationNode(derivative, new StarNode(Child));
    }

    public IRegexNode Simplify()
    {
        var simplifiedChild = Child.Simplify();

        // If the child is NullNode, Kleene star of NullNode is EmptyNode
        if (simplifiedChild.Type == RegexNodeType.EmptySet)
        {
            Console.WriteLine();
            //return new EpsilonNode();
        }
        if (simplifiedChild.Type == RegexNodeType.Epsilon)
        {
            Console.WriteLine();
        }

        // If the child is already a KleeneStarNode, we avoid unnecessary nesting
        if (simplifiedChild is StarNode)
        {
            return simplifiedChild;
        }
        if (simplifiedChild.Equals(this))
        {
            return simplifiedChild;
        }

        return new StarNode(simplifiedChild);
    }

    public bool Equals(IRegexNode other)
    {
        return other is IKleeneStarNode kleeneStar
            && kleeneStar.Child.Equals(Child);
    }

    public override string ToString()
    {
        return $"({Child})*";
    }
}

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

    private DfaState ComputeDerivativeStates(DfaState state, char c)
    {
        var regexNode = state.Node
            .Derive(c)
            .Simplify();

        return new DfaState(
            node: regexNode);
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
                case IKleeneStarNode kleeneStar:
                    stack.Push(kleeneStar.Child);
                    break;
            }
        }

        return alphabet
            .Reverse()
            .ToArray();
    }

}