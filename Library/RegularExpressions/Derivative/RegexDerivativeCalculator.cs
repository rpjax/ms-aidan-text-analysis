using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;

namespace Aidan.TextAnalysis.RegularExpressions.Derivative;

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

/// <summary>
/// Provides methods to calculate the derivative of a regular expression with respect to a character
/// and to simplify regular expression nodes.
/// </summary>
public class RegexDerivativeCalculator
{
    /// <summary>
    /// Gets the history of derivative calculations and simplifications.
    /// </summary>
    public CalculatorHistory History { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexDerivativeCalculator"/> class.
    /// </summary>
    public RegexDerivativeCalculator()
    {
        History = new CalculatorHistory();
    }

    /// <summary>
    /// Calculates the derivative of the given regular expression node with respect to the specified character.
    /// </summary>
    /// <param name="node">The regular expression node.</param>
    /// <param name="c">The character with respect to which the derivative is calculated.</param>
    /// <returns>The derivative of the regular expression node.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the node type is unknown.</exception>
    public RegexNode Derive(RegexNode node, char c)
    {
        RegexNode derivative;

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

    /// <summary>
    /// Simplifies the given regular expression node.
    /// </summary>
    /// <param name="node">The regular expression node to simplify.</param>
    /// <returns>The simplified regular expression node.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the node type is unknown.</exception>
    public RegexNode Simplify(RegexNode node)
    {
        RegexNode simplified;

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

    /// <summary>
    /// Returns a string that represents the history of derivative calculations and simplifications.
    /// </summary>
    /// <returns>A string that represents the history.</returns>
    public string GetHistoryString()
    {
        return History.ToString();
    }

    /* derivative calculation */

    private RegexNode DeriveEpsilonNode(EpsilonNode node, char c)
    {
        return new EmptySetNode()
            .PropagateLexeme(node);
    }

    private RegexNode DeriveEmptySetNode(EmptySetNode node, char c)
    {
        return new EmptySetNode()
            .PropagateLexeme(node);
    }

    private RegexNode DeriveLiteralNode(LiteralNode node, char c)
    {
        return c == node.Literal
            ? new EpsilonNode().PropagateLexeme(node)
            : new EmptySetNode().PropagateLexeme(node)
            ;
    }

    private RegexNode DeriveUnionNode(UnionNode node, char c)
    {
        var leftDerivative = Derive(node.Left, c);
        var rightDerivative = Derive(node.Right, c);

        return new UnionNode(leftDerivative, rightDerivative)
            .PropagateLexeme(node);
    }

    private RegexNode DeriveConcatenationNode(ConcatenationNode node, char c)
    {
        var left = node.Left;
        var right = node.Right;
        var leftDerivative = Derive(left, c);
        var rightDerivative = Derive(right, c);

        var concatenation = new ConcatenationNode(leftDerivative, right);

        if (left.ContainsEpsilon)
        {
            return new UnionNode(concatenation, rightDerivative)
                .PropagateLexeme(node);
        }
        else
        {
            return concatenation
                .PropagateLexeme(node);
        }
    }

    private RegexNode DeriveStarNode(StarNode node, char c)
    {
        var child = node.Child;
        var derivative = Derive(child, c);

        if (derivative.IsEmptySet())
        {
            return new EmptySetNode()
                .PropagateLexeme(node);
        }

        return new ConcatenationNode(derivative, new StarNode(child))
            .PropagateLexeme(node);
    }

    /* simplification */

    private RegexNode SimplifyEpsilonNode(EpsilonNode node)
    {
        return node;
    }

    private RegexNode SimplifyEmptySetNode(EmptySetNode node)
    {
        return node;
    }

    private RegexNode SimplifyLiteralNode(LiteralNode node)
    {
        return node;
    }

    private RegexNode SimplifyUnionNode(UnionNode node)
    {
        var left = node.Left;
        var right = node.Right;
        var simplifiedLeft = Simplify(left);
        var simplifiedRight = Simplify(right);

        if (simplifiedLeft.IsEmptySet())
        {
            return simplifiedRight
                .PropagateLexeme(node);
        }
        if (simplifiedRight.IsEmptySet())
        {
            return simplifiedLeft
                .PropagateLexeme(node);
        }

        /*
         * This simplification has been removed because it would eliminate accepting branches.
         * For example, `a|a(b)*` becomes `ε|(b)*` after consuming 'a', and if 'b' is not seen,
         * the regex should still accept.
         */
        if (left.IsEpsilon() && simplifiedRight.ContainsEpsilon)
        {
            // return simplifiedRight
            //     .PropagateLexeme(node);
        }

        /*
         * This simplification has been removed because it would eliminate accepting branches.
         * For example, `a(b)*|a` becomes `(b)*|ε` after consuming 'a', and if 'b' is not seen,
         * the regex should still accept.
         */
        if (right.IsEpsilon() && simplifiedLeft.ContainsEpsilon)
        {
            // return simplifiedLeft
            //     .PropagateLexeme(node);
        }

        // Avoid duplicate terms (R ∪ R = R)
        if (simplifiedLeft.Equals(simplifiedRight))
        {
            return simplifiedLeft
                .PropagateLexeme(node);
        }

        return new UnionNode(simplifiedLeft, simplifiedRight)
            .PropagateLexeme(node);
    }

    private RegexNode SimplifyConcatenationNode(ConcatenationNode node)
    {
        var left = node.Left;
        var right = node.Right;
        var simplifiedLeft = Simplify(left);
        var simplifiedRight = Simplify(right);

        // Concatenation with Empty Set (∅ ∘ R or R ∘ ∅ = ∅)
        if (simplifiedLeft.IsEmptySet() || simplifiedRight.IsEmptySet())
        {
            return new EmptySetNode()
                .PropagateLexeme(node);
        }

        // Concatenation with Epsilon (ε ∘ R = R and R ∘ ε = R)
        if (simplifiedLeft.IsEpsilon())
        {
            return simplifiedRight
                .PropagateLexeme(node);
        }
        if (simplifiedRight.IsEpsilon())
        {
            return simplifiedLeft
                .PropagateLexeme(node);
        }

        return new ConcatenationNode(simplifiedLeft, simplifiedRight)
            .PropagateLexeme(node);
    }

    private RegexNode SimplifyStarNode(StarNode node)
    {
        var simplifiedChild = Simplify(node.Child);

        // Star of Empty Set (∅*) simplifies to Epsilon (ε)
        if (simplifiedChild.IsEmptySet())
        {
            return new EpsilonNode()
                .PropagateLexeme(node);
        }

        // Star of Epsilon (ε*) simplifies to Epsilon (ε)
        if (simplifiedChild.IsEpsilon())
        {
            return new EpsilonNode()
                .PropagateLexeme(node);
        }

        // Avoid redundant nested stars (if the child is already a star, return the simplified child)
        if (simplifiedChild.IsStar())
        {
            return simplifiedChild
                .PropagateLexeme(node);
        }

        return new StarNode(simplifiedChild)
            .PropagateLexeme(node);
    }

}