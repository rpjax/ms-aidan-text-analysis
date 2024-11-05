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
    bool ContainsEpsilon { get; } // Can match the empty string
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
    public bool ContainsEpsilon => true;

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
    public bool ContainsEpsilon => false;

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
    public bool ContainsEpsilon => false;
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
    public bool ContainsEpsilon => Left.ContainsEpsilon || Right.ContainsEpsilon;
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

        if (Left.ContainsEpsilon && Right.Type == RegexNodeType.Epsilon)
        {
            return Left;
        }
        if (Right.ContainsEpsilon && Left.Type == RegexNodeType.Epsilon)
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
    public bool ContainsEpsilon => Left.ContainsEpsilon && Right.ContainsEpsilon;
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

        if (Left.ContainsEpsilon)
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
    public bool ContainsEpsilon => true;
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
