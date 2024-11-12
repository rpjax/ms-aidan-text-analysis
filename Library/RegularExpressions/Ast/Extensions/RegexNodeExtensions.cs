using Aidan.TextAnalysis.RegularExpressions.Automata;
using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;

/// <summary>
/// Provides extension methods for <see cref="RegexNode"/>.
/// </summary>
public static class RegexNodeExtensions
{
    /// <summary>
    /// Determines whether the specified node is an epsilon node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns><c>true</c> if the node is an epsilon node; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEpsilon(this RegexNode node)
    {
        return node.Type == RegexNodeType.Epsilon;
    }

    /// <summary>
    /// Determines whether the specified node is an empty set node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns><c>true</c> if the node is an empty set node; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmptySet(this RegexNode node)
    {
        return node.Type == RegexNodeType.EmptySet;
    }

    /// <summary>
    /// Determines whether the specified node is a literal node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns><c>true</c> if the node is a literal node; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLiteral(this RegexNode node)
    {
        return node.Type == RegexNodeType.Literal;
    }

    /// <summary>
    /// Determines whether the specified node is a union node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns><c>true</c> if the node is a union node; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUnion(this RegexNode node)
    {
        return node.Type == RegexNodeType.Union;
    }

    /// <summary>
    /// Determines whether the specified node is a concatenation node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns><c>true</c> if the node is a concatenation node; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsConcatenation(this RegexNode node)
    {
        return node.Type == RegexNodeType.Concatenation;
    }

    /// <summary>
    /// Determines whether the specified node is a star node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns><c>true</c> if the node is a star node; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsStar(this RegexNode node)
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
    public static EpsilonNode AsEpsilon(this RegexNode node)
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
    public static EmptySetNode AsEmptySet(this RegexNode node)
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
    public static LiteralNode AsLiteral(this RegexNode node)
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
    public static UnionNode AsUnion(this RegexNode node)
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
    public static ConcatenationNode AsConcatenation(this RegexNode node)
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
    public static StarNode AsStar(this RegexNode node)
    {
        return (StarNode)node ?? throw new InvalidCastException($"Invalid cast from {node?.Type} to StarNode");
    }

    /* utility methods */

    /// <summary>
    /// Sets the parent of the specified regex node.
    /// </summary>
    /// <param name="node">The regex node whose parent is to be set.</param>
    /// <param name="parent">The parent regex node.</param>
    public static void SetParent(
        this RegexNode node,
        RegexNode parent)
    {
        node.Parent = parent;
    }

    /// <summary>
    /// Creates a union of the current regex node with the specified other nodes.
    /// </summary>
    /// <param name="self">The current regex node.</param>
    /// <param name="others">The other regex nodes to union with the current node.</param>
    /// <returns>A new <see cref="RegexNode"/> representing the union of the current node and the specified nodes.</returns>
    public static RegexNode Union(
        this RegexNode self,
        params RegexNode[] others)
    {
        RegexNode regex = self;

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
    public static char[] ComputeAlphabet(
        this RegexNode regex,
        params char[]? extraChars)
    {
        var alphabet = new HashSet<char>(extraChars ?? Array.Empty<char>());
        var stack = new Stack<RegexNode>();

        stack.Push(regex);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            var children = node.GetChildren();

            if (node is LiteralNode literalNode)
            {
                alphabet.Add(literalNode.Character);
            }

            foreach (var child in children)
            {
                stack.Push(child);
            }
        }

        /* it's reversed to make it easier to read, there's no practical effect */
        return alphabet
            .Reverse()
            .ToArray();
    }

    /* metadata manipulation */

    /// <summary>
    /// Sets the metadata for the specified regex node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <param name="key">The key of the metadata.</param>
    /// <param name="value">The value of the metadata.</param>
    public static void SetMetadata(
        this RegexNode node,
        string key,
        object value)
    {
        node.Metadata[key] = value;
    }

    /// <summary>
    /// Gets the metadata for the specified regex node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <param name="key">The key of the metadata.</param>
    /// <returns>The value of the metadata if found; otherwise, <c>null</c>.</returns>
    public static object? GetMetadata(
        this RegexNode node,
        string key)
    {
        if (node.Metadata.TryGetValue(key, out var value))
        {
            return value;
        }

        return null;
    }

    /// <summary>
    /// Propagates the metadata to the specified regex node and its children.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <param name="key">The key of the metadata.</param>
    /// <param name="value">The value of the metadata.</param>
    public static void PropagateMetadata(
        this RegexNode node,
        string key,
        object value)
    {
        node.SetMetadata(key, value);

        foreach (var child in node.GetChildren())
        {
            child.PropagateMetadata(key, value);
        }
    }

    /* lexeme related methods */

    /// <summary>
    /// Gets the lexeme associated with the specified regex node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns>The lexeme if found; otherwise, <c>null</c>.</returns>
    /// <exception cref="InvalidCastException">Thrown when the metadata cannot be cast to <see cref="Lexeme"/>.</exception>
    public static Lexeme? GetLexeme(
        this RegexNode node)
    {
        var lexeme = node.GetMetadata("lexeme");

        if (lexeme is null)
        {
            return null;
        }

        if (lexeme is not Lexeme cast)
        {
            throw new InvalidCastException($"Invalid cast from {lexeme?.GetType()} to RegexLexeme");
        }

        return cast;
    }

    /// <summary>
    /// Sets the lexeme for the specified regex node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <param name="lexeme">The lexeme to set.</param>
    public static void SetLexeme(
        this RegexNode node,
        Lexeme lexeme)
    {
        node.SetMetadata("lexeme", lexeme);
    }

    /// <summary>
    /// Propagates the lexeme from the source node to the specified regex node and its children.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <param name="source">The source regex node.</param>
    /// <returns>The regex node with the propagated lexeme.</returns>
    public static RegexNode PropagateLexeme(
        this RegexNode node,
        RegexNode source)
    {
        var lexeme = source.GetLexeme();

        if (lexeme is null)
        {
            return node;
        }

        return node.PropagateLexeme(lexeme);
    }

    /// <summary>
    /// Propagates the lexeme to the specified regex node and its children.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <param name="lexeme">The lexeme to propagate.</param>
    /// <returns>The regex node with the propagated lexeme.</returns>
    public static RegexNode PropagateLexeme(
        this RegexNode node,
        Lexeme lexeme)
    {
        node.SetLexeme(lexeme);

        foreach (var child in node.GetChildren())
        {
            child.PropagateLexeme(lexeme);
        }

        return node;
    }

    /// <summary>
    /// Gets the epsilon branches of the specified regex node.
    /// </summary>
    /// <param name="node">The regex node.</param>
    /// <returns>An array of regex nodes representing the epsilon branches.</returns>
    public static RegexNode[] GetEpsilonBranches(
        this RegexNode node)
    {
        var branches = new List<RegexNode>();

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
