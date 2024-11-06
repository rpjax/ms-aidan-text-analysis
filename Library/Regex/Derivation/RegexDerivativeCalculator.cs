using Aidan.TextAnalysis.Regexes.Ast;
using System.Text;

namespace Aidan.TextAnalysis.Regexes.Derivation;

public class Derivation
{
    public IRegexNode Regex { get; }
    public char Character { get; }
    public IRegexNode Derivative { get; }

    public Derivation(
        IRegexNode regex,
        char character,
        IRegexNode derivative)
    {
        Regex = regex;
        Character = character;
        Derivative = derivative;
    }

    public override string ToString()
    {
        return $"`{Regex}` with respect to `{Character}` = `{Derivative}`";
    }
}

public class Simplification
{
    public IRegexNode Regex { get; }
    public IRegexNode SimplifiedRegex { get; }

    public Simplification(
        IRegexNode regex,
        IRegexNode simplifiedRegex)
    {
        Regex = regex;
        SimplifiedRegex = simplifiedRegex;

    }

    public override string ToString()
    {
        return $"`{Regex}` simplified to `{SimplifiedRegex}`";
    }
}

public class DerivativeHistory
{
    public List<object> Records { get; }

    public DerivativeHistory()
    {
        Records = new List<object>();
    }

    public void AddDerivative(IRegexNode regex, char character, IRegexNode derivative)
    {
        Records.Add(new Derivation(regex, character, derivative));
    }

    public void AddSimplification(IRegexNode regex, IRegexNode simplifiedRegex)
    {
        Records.Add(new Simplification(regex, simplifiedRegex));
    }

    public void Clear()
    {
        Records.Clear();
    }

    public void Print()
    {
        //foreach (var record in Records)
        //{
        //    Console.WriteLine($"{record.Regex} --{record.Character}--> {record.Derivative}");
        //}
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var record in Records)
        {
            sb.AppendLine(record.ToString());
        }

        return sb.ToString();
    }

}

public class RegexDerivativeCalculator
{
    public DerivativeHistory History { get; }

    public RegexDerivativeCalculator()
    {
        History = new DerivativeHistory();
    }

    public IRegexNode Derive(IRegexNode node, char c)
    {
        IRegexNode derivative;

        switch (node.Type)
        {
            case RegexNodeType.Epsilon:
                derivative = DeriveEpsilonNode((EpsilonNode)node, c);
                break;

            case RegexNodeType.EmptySet:
                derivative = DeriveEmptySetNode((EmptySetNode)node, c);
                break;

            case RegexNodeType.Literal:
                derivative = DeriveLiteralNode((LiteralNode)node, c);
                break;

            case RegexNodeType.Union:
                derivative = DeriveUnionNode((UnionNode)node, c);
                break;

            case RegexNodeType.Concatenation:
                derivative = DeriveConcatenationNode((ConcatenationNode)node, c);
                break;

            case RegexNodeType.Star:
                derivative = DeriveStarNode((StarNode)node, c);
                break;

            default:
                throw new InvalidOperationException($"Unknown node type: {node.Type}");
        }

        /* todo: add history for debugging */
        History.AddDerivative(node, c, derivative);

        var simplified = Simplify(derivative);

        return simplified;
    }

    public IRegexNode Simplify(IRegexNode node)
    {
        IRegexNode simplified;

        switch (node.Type)
        {
            case RegexNodeType.Epsilon:
                simplified = SimplifyEpsilonNode((EpsilonNode)node);
                break;

            case RegexNodeType.EmptySet:
                simplified = SimplifyEmptySetNode((EmptySetNode)node);
                break;

            case RegexNodeType.Literal:
                simplified = SimplifyLiteralNode((LiteralNode)node);
                break;

            case RegexNodeType.Union:
                simplified = SimplifyUnionNode((UnionNode)node);
                break;

            case RegexNodeType.Concatenation:
                simplified = SimplifyConcatenationNode((ConcatenationNode)node);
                break;

            case RegexNodeType.Star:
                simplified = SimplifyStarNode((StarNode)node);
                break;

            default:
                throw new InvalidOperationException($"Unknown node type: {node.Type}");
        }

        if (!simplified.Equals(node))
        {
            History.AddSimplification(node, simplified);
        }

        return simplified;
    }

    /* derivative calculation */

    private IRegexNode DeriveEpsilonNode(EpsilonNode node, char c)
    {
        return new EmptySetNode();
    }

    private IRegexNode DeriveEmptySetNode(EmptySetNode node, char c)
    {
        return new EmptySetNode();
    }

    private IRegexNode DeriveLiteralNode(LiteralNode node, char c)
    {
        return c == node.Literal
            ? new EpsilonNode()
            : new EmptySetNode()
            ;
    }

    private IRegexNode DeriveUnionNode(UnionNode node, char c)
    {
        var leftDeriv = Derive(node.Left, c);
        var rightDeriv = Derive(node.Right, c);

        if (leftDeriv.Type is RegexNodeType.EmptySet)
        {
            return rightDeriv;
        }
        if (rightDeriv.Type is RegexNodeType.EmptySet)
        {
            return leftDeriv;
        }

        return new UnionNode(leftDeriv, rightDeriv);
    }

    private IRegexNode DeriveConcatenationNode(ConcatenationNode node, char c)
    {
        var leftDerivative = Derive(node.Left, c);

        /* the concatenation of an empty set with any node is an empty set */
        if (leftDerivative is EmptySetNode)
        {
            return new EmptySetNode();
        }
        /* epsilon concatenated with anything is the anything */
        if (leftDerivative is EpsilonNode)
        {
            return node.Right;
        }

        if (node.Left.ContainsEpsilon)
        {
            var rightDerivative = Derive(node.Right, c);

            return new UnionNode(
                new ConcatenationNode(leftDerivative, node.Right),
                rightDerivative
            );
        }
        else
        {
            return new ConcatenationNode(leftDerivative, node.Right);
        }
    }

    private IRegexNode DeriveStarNode(StarNode node, char c)
    {
        var derivative = Derive(node.Child, c);

        if (derivative is EmptySetNode)
        {
            return new EmptySetNode();
        }

        return new ConcatenationNode(derivative, new StarNode(node.Child));
    }

    /* simplification */

    private IRegexNode SimplifyEpsilonNode(EpsilonNode node)
    {
        return node;
    }

    private IRegexNode SimplifyEmptySetNode(EmptySetNode node)
    {
        return node;
    }

    private IRegexNode SimplifyLiteralNode(LiteralNode node)
    {
        return node;
    }

    private IRegexNode SimplifyUnionNode(UnionNode node)
    {
        var left = Simplify(node.Left);
        var right = Simplify(node.Right);

        // Union with NullNode simplifies to the other node
        if (left.Type == RegexNodeType.EmptySet)
        {
            return right;
        }
        if (right.Type == RegexNodeType.EmptySet)
        {
            return left;
        }

        if (node.Left.ContainsEpsilon && node.Right.Type == RegexNodeType.Epsilon)
        {
            return node.Left;
        }
        if (node.Right.ContainsEpsilon && node.Left.Type == RegexNodeType.Epsilon)
        {
            return node.Right;
        }

        return new UnionNode(left, right);
    }

    private IRegexNode SimplifyConcatenationNode(ConcatenationNode node)
    {
        var left = Simplify(node.Left);
        var right = Simplify(node.Right);

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

    private IRegexNode SimplifyStarNode(StarNode node)
    {
        var simplifiedChild = Simplify(node.Child);

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

}