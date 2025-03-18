using System.Runtime.CompilerServices;
using Aidan.TextAnalysis.RegularExpressions.Ast.Enums;
using Aidan.TextAnalysis.RegularExpressions.Derivative;

namespace Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;

/// <summary>
/// Provides extension methods for <see cref="RegExpr"/>.
/// </summary>
public static class RegexNodeExtensions
{
    /// <summary>
    /// Determines whether the specified node is an epsilon node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns><c>true</c> if the node is an epsilon node; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEpsilon(this RegExpr node)
    {
        return node.Type == RegexNodeType.Epsilon;
    }

    /// <summary>
    /// Determines whether the specified node is an empty set node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns><c>true</c> if the node is an empty set node; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmptySet(this RegExpr node)
    {
        return node.Type == RegexNodeType.EmptySet;
    }

    /// <summary>
    /// Determines whether the specified node is a literal node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns><c>true</c> if the node is a literal node; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLiteral(this RegExpr node)
    {
        return node.Type == RegexNodeType.Literal;
    }

    /// <summary>
    /// Determines whether the specified node is a union node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns><c>true</c> if the node is a union node; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUnion(this RegExpr node)
    {
        return node.Type == RegexNodeType.Union;
    }

    /// <summary>
    /// Determines whether the specified node is a concatenation node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns><c>true</c> if the node is a concatenation node; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsConcatenation(this RegExpr node)
    {
        return node.Type == RegexNodeType.Concatenation;
    }

    /// <summary>
    /// Determines whether the specified node is a star node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns><c>true</c> if the node is a star node; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsStar(this RegExpr node)
    {
        return node.Type == RegexNodeType.Star;
    }

    /* conversion methods */

    /// <summary>
    /// Converts the specified node to an <see cref="EpsilonNode"/>.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns>The <see cref="EpsilonNode"/>.</returns>
    /// <exception cref="InvalidCastException">Thrown when the node cannot be cast to <see cref="EpsilonNode"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EpsilonNode AsEpsilon(this RegExpr node)
    {
        return (EpsilonNode)node ?? throw new InvalidCastException($"Invalid cast from {node?.Type} to EpsilonNode");
    }

    /// <summary>
    /// Converts the specified node to an <see cref="EmptySetNode"/>.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns>The <see cref="EmptySetNode"/>.</returns>
    /// <exception cref="InvalidCastException">Thrown when the node cannot be cast to <see cref="EmptySetNode"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EmptySetNode AsEmptySet(this RegExpr node)
    {
        return (EmptySetNode)node ?? throw new InvalidCastException($"Invalid cast from {node?.Type} to EmptySetNode");
    }

    /// <summary>
    /// Converts the specified node to a <see cref="LiteralNode"/>.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns>The <see cref="LiteralNode"/>.</returns>
    /// <exception cref="InvalidCastException">Thrown when the node cannot be cast to <see cref="LiteralNode"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralNode AsLiteral(this RegExpr node)
    {
        return (LiteralNode)node ?? throw new InvalidCastException($"Invalid cast from {node?.Type} to LiteralNode");
    }

    /// <summary>
    /// Converts the specified node to a <see cref="UnionNode"/>.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns>The <see cref="UnionNode"/>.</returns>
    /// <exception cref="InvalidCastException">Thrown when the node cannot be cast to <see cref="UnionNode"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UnionNode AsUnion(this RegExpr node)
    {
        return (UnionNode)node ?? throw new InvalidCastException($"Invalid cast from {node?.Type} to UnionNode");
    }

    /// <summary>
    /// Converts the specified node to a <see cref="ConcatenationNode"/>.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns>The <see cref="ConcatenationNode"/>.</returns>
    /// <exception cref="InvalidCastException">Thrown when the node cannot be cast to <see cref="ConcatenationNode"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConcatenationNode AsConcatenation(this RegExpr node)
    {
        return (ConcatenationNode)node ?? throw new InvalidCastException($"Invalid cast from {node?.Type} to ConcatenationNode");
    }

    /// <summary>
    /// Converts the specified node to a <see cref="StarNode"/>.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns>The <see cref="StarNode"/>.</returns>
    /// <exception cref="InvalidCastException">Thrown when the node cannot be cast to <see cref="StarNode"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StarNode AsStar(this RegExpr node)
    {
        return (StarNode)node ?? throw new InvalidCastException($"Invalid cast from {node?.Type} to StarNode");
    }

    /// <summary>
    /// Converts the specified node to a <see cref="AnythingNode"/>.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns>The <see cref="AnythingNode"/>.</returns>
    /// <exception cref="InvalidCastException">Thrown when the node cannot be cast to <see cref="AnythingNode"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AnythingNode AsAnything(this RegExpr node)
    {
        return (AnythingNode)node ?? throw new InvalidCastException($"Invalid cast from {node?.Type} to AnythingNode");
    }

    /// <summary>
    /// Converts the specified node to a <see cref="ClassNode"/>.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns>The <see cref="ClassNode"/>.</returns>
    /// <exception cref="InvalidCastException">Thrown when the node cannot be cast to <see cref="ClassNode"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ClassNode AsClass(this RegExpr node)
    {
        return (ClassNode)node ?? throw new InvalidCastException($"Invalid cast from {node?.Type} to ClassNode");
    }

    /* utility methods */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RegExpr ComputeDerivative(this RegExpr node, char character)
    {
        return new RegexDerivativeCalculator()
            .Derive(node, character);
    }

    /// <summary>
    /// Sets the parent of the specified regex node.
    /// </summary>
    /// <param name="node">The regex node whose parent is to be set.</param>
    /// <param name="parent">The parent regex node.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetParent(
        this RegExpr node,
        RegExpr parent)
    {
        node.Parent = parent;
    }

    /// <summary>
    /// Creates a union of the current regex node with the specified other nodes.
    /// </summary>
    /// <param name="self">The current regex node.</param>
    /// <param name="others">The other regex nodes to union with the current node.</param>
    /// <returns>A new <see cref="RegExpr"/> representing the union of the current node and the specified nodes.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RegExpr Union(
        this RegExpr self,
        params RegExpr[] others)
    {
        RegExpr regex = self;

        foreach (var other in others)
        {
            regex = new UnionNode(regex, other);
        }

        return regex;
    }

    /// <summary>
    /// Computes the alphabet of the specified regex node.
    /// </summary>
    /// <param name="regex">The regex node.</param>
    /// <param name="extraChars">A set of extra characters to include in the alphabet. 
    /// <br/>Useful for adding whitespace, EOF, control characters, etc.</param>
    /// <returns>An array of characters representing the alphabet of the regex.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char[] ComputeAlphabet(
        this RegExpr regex,
        params char[]? extraChars)
    {
        var alphabet = new List<char>(extraChars ?? Array.Empty<char>());
        var stack = new Stack<RegExpr>();

        stack.Push(regex);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            var children = node.GetChildren();

            if (node is LiteralNode literalNode)
            {
                alphabet.Add(literalNode.Character);
            }
            if (node is AnythingNode anythingNode)
            {
                alphabet.AddRange(anythingNode.Charset);
            }
            if (node is ClassNode classNode)
            {
                alphabet.AddRange(classNode.ComputeResultingCharset());
            }

            foreach (var child in children)
            {
                stack.Push(child);
            }
        }

        return alphabet
            .Distinct()
            /* it's ordered to make it easier to read, there's no practical effect */
            .OrderBy(x => x)
            .ToArray();
    }

    /// <summary>
    /// Gets the epsilon branches of the specified regex node.
    /// </summary>
    /// <param name="node">The regex node.</param>

    /// <returns>An array of regex nodes representing the epsilon branches.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RegExpr[] GetEpsilonBranches(
        this RegExpr node)
    {
        var branches = new List<RegExpr>();

        foreach (var child in node.GetChildren())
        {
            if (child.ContainsEpsilon)
            {
                branches.Add(child);
            }
        }

        return branches.ToArray();
    }

}
