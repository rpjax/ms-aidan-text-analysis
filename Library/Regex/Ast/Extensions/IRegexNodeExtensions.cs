using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.Regexes.Ast.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IRegexNode"/>.
/// </summary>
public static class IRegexNodeExtensions
{
    /// <summary>
    /// Determines whether the specified node is an epsilon node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns><c>true</c> if the node is an epsilon node; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEpsilon(this IRegexNode node)
    {
        return node.Type == RegexNodeType.Epsilon;
    }

    /// <summary>
    /// Determines whether the specified node is an empty set node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns><c>true</c> if the node is an empty set node; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmptySet(this IRegexNode node)
    {
        return node.Type == RegexNodeType.EmptySet;
    }

    /// <summary>
    /// Determines whether the specified node is a literal node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns><c>true</c> if the node is a literal node; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLiteral(this IRegexNode node)
    {
        return node.Type == RegexNodeType.Literal;
    }

    /// <summary>
    /// Determines whether the specified node is a union node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns><c>true</c> if the node is a union node; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUnion(this IRegexNode node)
    {
        return node.Type == RegexNodeType.Union;
    }

    /// <summary>
    /// Determines whether the specified node is a concatenation node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns><c>true</c> if the node is a concatenation node; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsConcatenation(this IRegexNode node)
    {
        return node.Type == RegexNodeType.Concatenation;
    }

    /// <summary>
    /// Determines whether the specified node is a star node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns><c>true</c> if the node is a star node; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsStar(this IRegexNode node)
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
    public static EpsilonNode AsEpsilon(this IRegexNode node)
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
    public static EmptySetNode AsEmptySet(this IRegexNode node)
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
    public static LiteralNode AsLiteral(this IRegexNode node)
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
    public static UnionNode AsUnion(this IRegexNode node)
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
    public static ConcatenationNode AsConcatenation(this IRegexNode node)
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
    public static StarNode AsStar(this IRegexNode node)
    {
        return (StarNode)node ?? throw new InvalidCastException($"Invalid cast from {node?.Type} to StarNode");
    }
}
