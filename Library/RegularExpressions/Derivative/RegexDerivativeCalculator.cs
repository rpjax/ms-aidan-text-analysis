using Aidan.TextAnalysis.RegularExpressions.Tree;
using Aidan.TextAnalysis.RegularExpressions.Tree.Extensions;
using System.Runtime.CompilerServices;

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RegExpr Derive(RegExpr node, char c)
    {
        RegExpr derivative;

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

            case RegexNodeType.Anything:
                derivative = DeriveAnythingNode(node.AsAnything(), c);
                break;

            case RegexNodeType.Class:
                derivative = DeriveClassNode(node.AsClass(), c);
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RegExpr Simplify(RegExpr node)
    {
        RegExpr simplified;

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

            case RegexNodeType.Anything:
                simplified = SimplifyAnythingNode(node.AsAnything());
                break;

            case RegexNodeType.Class:
                simplified = SimplifyClassNode(node.AsClass());
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr DeriveEpsilonNode(EpsilonNode node, char c)
    {
        return new EmptySetNode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr DeriveEmptySetNode(EmptySetNode node, char c)
    {
        return new EmptySetNode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr DeriveLiteralNode(LiteralNode node, char c)
    {
        return c == node.Character
            ? new EpsilonNode()
            : new EmptySetNode()
            ;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr DeriveUnionNode(UnionNode node, char c)
    {
        var leftDerivative = Derive(node.Left, c);
        var rightDerivative = Derive(node.Right, c);

        return new UnionNode(leftDerivative, rightDerivative);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr DeriveConcatenationNode(ConcatenationNode node, char c)
    {
        var left = node.Left;
        var right = node.Right;
        var leftDerivative = Derive(left, c);
        var rightDerivative = Derive(right, c);

        /* Derivative of `a.b` if `a` does not generate ε:
         * Simply derive `a` and concatenate with `b`: `derivative(a).b`.
         */
        if (!left.ContainsEpsilon)
        {
            return new ConcatenationNode(leftDerivative, right);
        }

        /* Derivative of `a.b` if `a` generates ε:
         * Combine both possible branches: `derivative(a).b | derivative(b)`.
         */
        return new UnionNode(
            new ConcatenationNode(leftDerivative, right), // `derivative(a).b`
            rightDerivative                              // `derivative(b)`
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr DeriveStarNode(StarNode node, char c)
    {
        var child = node.Child;
        var derivative = Derive(child, c);

        if (derivative.IsEmptySet())
        {
            return new EmptySetNode();
        }

        return new ConcatenationNode(derivative, new StarNode(child));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr DeriveAnythingNode(AnythingNode node, char c)
    {
        return node.Charset.Contains(c)
            ? new EpsilonNode()
            : new EmptySetNode()
            ;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr DeriveClassNode(ClassNode node, char c)
    {
        var charset = node.ComputeResultingCharset();

        return charset.Contains(c)
            ? new EpsilonNode()
            : new EmptySetNode()
            ;
    }

    /* simplification */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr SimplifyEpsilonNode(EpsilonNode node)
    {
        return node;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr SimplifyEmptySetNode(EmptySetNode node)
    {
        return node;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr SimplifyLiteralNode(LiteralNode node)
    {
        return node;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr SimplifyUnionNode(UnionNode node)
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

        /*
         * This simplification has been removed because it would eliminate accepting branches.
         * For example, `a|a(b)*` becomes `ε|(b)*` after consuming 'a', and if 'b' is not seen,
         * the regex should still accept.
         */
        if (left.IsEpsilon() && simplifiedRight.ContainsEpsilon)
        {
            // return simplifiedRight
            //     ;
        }

        /*
         * This simplification has been removed because it would eliminate accepting branches.
         * For example, `a(b)*|a` becomes `(b)*|ε` after consuming 'a', and if 'b' is not seen,
         * the regex should still accept.
         */
        if (right.IsEpsilon() && simplifiedLeft.ContainsEpsilon)
        {
            // return simplifiedLeft
            //     ;
        }

        // Avoid duplicate terms (R ∪ R = R)
        if (simplifiedLeft.Equals(simplifiedRight))
        {
            return simplifiedLeft;
        }

        return new UnionNode(simplifiedLeft, simplifiedRight);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr SimplifyConcatenationNode(ConcatenationNode node)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr SimplifyStarNode(StarNode node)
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
        if (simplifiedChild.IsStar())
        {
            return simplifiedChild;
        }

        return new StarNode(simplifiedChild);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr SimplifyAnythingNode(AnythingNode node)
    {
        return node;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr SimplifyClassNode(ClassNode node)
    {
        return node;
    }

}