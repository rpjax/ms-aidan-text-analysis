using Aidan.TextAnalysis.Regexes.Ast;
using Aidan.TextAnalysis.Regexes.Ast.Extensions;
using System.Text;

namespace Aidan.TextAnalysis.Regexes.Derivative;

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

/*
 * Author Note:
 * 
 * Maintance of this code should be done with caution. 
 * This is an absolutely critical part of the regex engine, and it's a nightmare to debug.
 * 
 * The calculator uses Brzozowski's algorithm to calculate the derivative of a regular expression with respect to a character.
 * The History property is used to keep track of the steps taken to calculate the derivative, it helps with debugging. 
 * So, if you're changing this code, use and abuse the History property to debug the derivative calculation.
 * You can easily print the history to the console by calling the ToString() method on the History property.
 * 
 * The simplification of the regex seems to be crutial for the correctness of derivative calculation.
 * 
 * I don't really understand the math behind this, but I've tried to make the code as adherent to the theory as possible.
 * If you are a more experienced engineer, feel free to submit any improvements to this code. It would be greatly appreciated.
 */

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
                derivative = DeriveEpsilonNode(node.AsEpsilon(), c);
                break;

            case RegexNodeType.EmptySet:
                derivative = DeriveEmptySetNode(node.AsEmptySet(), c);
                break;

            case RegexNodeType.Literal:
                derivative = DeriveLiteralNode(node.AsLiteral(), c);
                break;

            case RegexNodeType.Union:
                derivative = DeriveUnionNode(node.AsUnion(), c);
                break;

            case RegexNodeType.Concatenation:
                derivative = DeriveConcatenationNode(node.AsConcatenation(), c);
                break;

            case RegexNodeType.Star:
                derivative = DeriveStarNode(node.AsStar(), c);
                break;

            default:
                throw new InvalidOperationException($"Unknown node type: {node.Type}");
        }

        /* todo: add history for debugging */
        History.AddDerivative(node, c, derivative);

        /* simplify the derivative before returning it */
        return Simplify(derivative);
    }

    public IRegexNode Simplify(IRegexNode node)
    {
        IRegexNode simplified;

        switch (node.Type)
        {
            case RegexNodeType.Epsilon:
                simplified = SimplifyEpsilonNode(node.AsEpsilon());
                break;

            case RegexNodeType.EmptySet:
                simplified = SimplifyEmptySetNode(node.AsEmptySet());
                break;

            case RegexNodeType.Literal:
                simplified = SimplifyLiteralNode(node.AsLiteral());
                break;

            case RegexNodeType.Union:
                simplified = SimplifyUnionNode(node.AsUnion());
                break;

            case RegexNodeType.Concatenation:
                simplified = SimplifyConcatenationNode(node.AsConcatenation());
                break;

            case RegexNodeType.Star:
                simplified = SimplifyStarNode(node.AsStar());
                break;

            default:
                throw new InvalidOperationException($"Unknown node type: {node.Type}");
        }

        /* if the simplified node is the same as the original node, we don't need to add it to the history */
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
        var leftDerivative = Derive(node.Left, c);
        var rightDerivative = Derive(node.Right, c);

        return new UnionNode(leftDerivative, rightDerivative);
    }

    private IRegexNode DeriveConcatenationNode(ConcatenationNode node, char c)
    {
        var leftDerivative = Derive(node.Left, c);
        var rightDerivative = Derive(node.Right, c);

        /* the concatenation of an empty set with any node is an empty set */
        if (leftDerivative is EmptySetNode)
        {
            //return new EmptySetNode();
        }
        /* epsilon concatenated with anything is the anything */
        if (leftDerivative is EpsilonNode)
        {
            //return node.Right;
        }

        if (node.Left.ContainsEpsilon)
        {
            // If right derivative is the empty set, skip it in the union.
            if (rightDerivative.IsEmptySet())
            {
                //return new ConcatenationNode(leftDerivative, node.Right);
            }

            // If right derivative is epsilon, return only the left derivative.
            if (rightDerivative.IsEpsilon())
            {
                //return leftDerivative;
            }

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
        var left = node.Left;
        var right = node.Right;
        var simplifiedLeft = Simplify(left);
        var simplifiedRight = Simplify(right);

        if (simplifiedLeft.IsEmptySet())
        {
            return simplifiedRight;
        }
        if (simplifiedRight.IsEmptySet())
        {
            return simplifiedLeft;
        }

        if (left.IsEpsilon() && simplifiedRight.ContainsEpsilon)
        {
            return simplifiedRight;
        }
        if (right.IsEpsilon() && simplifiedLeft.ContainsEpsilon)
        {
            return simplifiedLeft;
        }

        // Avoid duplicate terms (R ∪ R = R)
        if (simplifiedLeft.Equals(simplifiedRight))
        {
            return simplifiedLeft;
        }

        return new UnionNode(simplifiedLeft, simplifiedRight);
    }

    private IRegexNode SimplifyConcatenationNode(ConcatenationNode node)
    {
        var left = node.Left;
        var right = node.Right;
        var simplifiedLeft = Simplify(left);
        var simplifiedRight = Simplify(right);

        // Concatenation with Empty Set (∅ ∘ R or R ∘ ∅ = ∅)
        if (simplifiedLeft.IsEmptySet() || simplifiedRight.IsEmptySet())
        {
            return new EmptySetNode();
        }

        // Concatenation with Epsilon (ε ∘ R = R and R ∘ ε = R)
        if (simplifiedLeft.IsEpsilon())
        {
            return simplifiedRight;
        }
        if (simplifiedRight.IsEpsilon())
        {
            return simplifiedLeft;
        }

        return new ConcatenationNode(simplifiedLeft, simplifiedRight);
    }

    private IRegexNode SimplifyStarNode(StarNode node)
    {
        var simplifiedChild = Simplify(node.Child);

        // Star of Empty Set (∅*) simplifies to Epsilon (ε)
        if (simplifiedChild.IsEmptySet())
        {
            return new EpsilonNode();
        }

        // Star of Epsilon (ε*) simplifies to Epsilon (ε)
        if (simplifiedChild.IsEpsilon())
        {
            return new EpsilonNode();
        }

        // Avoid redundant nested stars (if the child is already a star, return the simplified child)
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