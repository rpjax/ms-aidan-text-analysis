using Aidan.TextAnalysis.Regexes.Ast.Extensions;
using System.Globalization;

namespace Aidan.TextAnalysis.Regexes.Ast;

/// <summary>
/// Represents the type of a regex node.
/// </summary>
public enum RegexNodeType
{
    /// <summary>
    /// Represents an epsilon node, matching an empty string.
    /// </summary>
    Epsilon,

    /// <summary>
    /// Represents an empty set node, matching nothing.
    /// </summary>
    EmptySet,

    /// <summary>
    /// Represents a literal node, matching a specific character.
    /// </summary>
    Literal,

    /// <summary>
    /// Represents a union node, matching one of two patterns.
    /// </summary>
    Union,

    /// <summary>
    /// Represents a concatenation node, matching a sequence of patterns.
    /// </summary>
    Concatenation,

    /// <summary>
    /// Represents a star node, matching zero or more repetitions of a pattern.
    /// </summary>
    Star,
}

/// <summary>
/// Represents a regex node in the abstract syntax tree of a regular expression.
/// </summary>
public interface IRegexNode : IEquatable<IRegexNode>
{
    /// <summary>
    /// Gets the type of the regex node.
    /// </summary>
    RegexNodeType Type { get; }

    /// <summary>
    /// Gets a value indicating whether the node can match the empty string.
    /// </summary>
    bool ContainsEpsilon { get; }

    /// <summary>
    /// Returns a string representation of the regex node.
    /// </summary>
    /// <returns>A string representation of the regex node.</returns>
    string ToString();
}

/// <summary>
/// Represents a literal node in a regex, matching a specific character.
/// </summary>
public interface ILiteralNode : IRegexNode
{
    /// <summary>
    /// Gets the literal character matched by this node.
    /// </summary>
    char Literal { get; }
}

/// <summary>
/// Represents a concatenation node in a regex, matching a sequence of patterns.
/// </summary>
public interface IConcatenationNode : IRegexNode
{
    /// <summary>
    /// Gets the left child node in the concatenation.
    /// </summary>
    IRegexNode Left { get; }

    /// <summary>
    /// Gets the right child node in the concatenation.
    /// </summary>
    IRegexNode Right { get; }
}

/// <summary>
/// Represents a union node in a regex, matching one of two patterns.
/// </summary>
public interface IUnionNode : IRegexNode
{
    /// <summary>
    /// Gets the left child node in the union.
    /// </summary>
    IRegexNode Left { get; }

    /// <summary>
    /// Gets the right child node in the union.
    /// </summary>
    IRegexNode Right { get; }
}

/// <summary>
/// Represents a star node in a regex, matching zero or more repetitions of a pattern.
/// </summary>
public interface IStarNode : IRegexNode
{
    /// <summary>
    /// Gets the child node of the star operation.
    /// </summary>
    IRegexNode Child { get; }
}

/*
 * Concrete implementations of the regex nodes.
 */

/// <summary>
/// Represents an epsilon node in a regex, which matches the empty string ε.
/// </summary>
public class EpsilonNode : IRegexNode
{
    /// <inheritdoc />
    public RegexNodeType Type { get; }

    /// <inheritdoc />
    public bool ContainsEpsilon { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EpsilonNode"/> class.
    /// </summary>
    public EpsilonNode()
    {
        Type = RegexNodeType.Epsilon;
        ContainsEpsilon = true;
    }

    /// <inheritdoc />
    public bool Equals(IRegexNode? other)
    {
        return other?.IsEpsilon() == true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return "ε";
    }
}

/// <summary>
/// Represents an empty set node in a regex, which matches nothing (∅).
/// </summary>
public class EmptySetNode : IRegexNode
{
    /// <inheritdoc />
    public RegexNodeType Type { get; }

    /// <inheritdoc />
    public bool ContainsEpsilon { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmptySetNode"/> class.
    /// </summary>
    public EmptySetNode()
    {
        Type = RegexNodeType.EmptySet;
        ContainsEpsilon = false;
    }

    /// <inheritdoc />
    public bool Equals(IRegexNode? other)
    {
        return other?.IsEmptySet() == true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return "∅";
    }
}

/// <summary>
/// Represents a literal node in a regex, matching a specific character.
/// </summary>
public class LiteralNode : ILiteralNode
{
    /// <inheritdoc />
    public RegexNodeType Type { get; }

    /// <inheritdoc />
    public bool ContainsEpsilon { get; }

    /// <inheritdoc />
    public char Literal { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LiteralNode"/> class.
    /// </summary>
    /// <param name="literal">The literal character to match.</param>
    public LiteralNode(char literal)
    {
        Type = RegexNodeType.Literal;
        ContainsEpsilon = false;
        Literal = literal;
    }

    /// <inheritdoc />
    public bool Equals(IRegexNode? other)
    {
        return other is ILiteralNode literal 
            && literal.Literal == Literal;
    }      

    /// <inheritdoc />
    public override string ToString()
    {
        return Literal.ToString(CultureInfo.InvariantCulture);
    }
}

/// <summary>
/// Represents a union node in a regex, matching either of two patterns.
/// </summary>
public class UnionNode : IUnionNode
{
    /// <inheritdoc />
    public RegexNodeType Type { get; }

    /// <inheritdoc />
    public bool ContainsEpsilon { get; }

    /// <inheritdoc />
    public IRegexNode Left { get; }

    /// <inheritdoc />
    public IRegexNode Right { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnionNode"/> class.
    /// </summary>
    /// <param name="left">The left operand of the union.</param>
    /// <param name="right">The right operand of the union.</param>
    public UnionNode(IRegexNode left, IRegexNode right)
    {
        Type = RegexNodeType.Union;
        ContainsEpsilon = left.ContainsEpsilon || right.ContainsEpsilon;
        Left = left;
        Right = right;
    }

    /// <inheritdoc />
    public bool Equals(IRegexNode? other)
    {
        return other is IUnionNode union 
            && union.Left.Equals(Left)
            && union.Right.Equals(Right);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Left}|{Right}";
    }
}

/// <summary>
/// Represents a concatenation node in a regex, matching a sequence of patterns.
/// </summary>
public class ConcatenationNode : IConcatenationNode
{
    /// <inheritdoc />
    public RegexNodeType Type { get; }

    /// <inheritdoc />
    public bool ContainsEpsilon { get; }

    /// <inheritdoc />
    public IRegexNode Left { get; }

    /// <inheritdoc />
    public IRegexNode Right { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcatenationNode"/> class.
    /// </summary>
    /// <param name="left">The left operand in the concatenation.</param>
    /// <param name="right">The right operand in the concatenation.</param>
    public ConcatenationNode(IRegexNode left, IRegexNode right)
    {
        Type = RegexNodeType.Concatenation;
        ContainsEpsilon = left.ContainsEpsilon && right.ContainsEpsilon;
        Left = left;
        Right = right;
    }

    /// <inheritdoc />
    public bool Equals(IRegexNode? other)
    {
        return other is IConcatenationNode concatenation
            && concatenation.Left.Equals(Left)
            && concatenation.Right.Equals(Right);
    }
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Left}{Right}";
    }
}

/// <summary>
/// Represents a star (Kleene star) node in a regex, matching zero or more repetitions of a pattern.
/// </summary>
public class StarNode : IStarNode
{
    /// <inheritdoc />
    public RegexNodeType Type { get; }

    /// <inheritdoc />
    public bool ContainsEpsilon { get; }

    /// <inheritdoc />
    public IRegexNode Child { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StarNode"/> class.
    /// </summary>
    /// <param name="child">The child pattern to repeat zero or more times.</param>
    public StarNode(IRegexNode child)
    {
        Type = RegexNodeType.Star;
        ContainsEpsilon = true;
        Child = child;
    }

    /// <inheritdoc />
    public bool Equals(IRegexNode? other)
    {
        return other is IStarNode star
            && star.Child.Equals(Child);
    }
        
    /// <inheritdoc />
    public override string ToString()
    {
        return $"({Child})*";
    }
}
