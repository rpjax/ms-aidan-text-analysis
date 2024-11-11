using Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;

namespace Aidan.TextAnalysis.RegularExpressions.Ast;

/// <summary>
/// Represents an abstract base class for regex nodes in the abstract syntax tree of a regular expression.
/// </summary>
public abstract class RegexNode : IEquatable<RegexNode>
{
    /// <summary>
    /// Gets the type of the regex node.
    /// </summary>
    public RegexNodeType Type { get; }

    /// <summary>
    /// Gets a value indicating whether the node can match the empty string.
    /// </summary>
    public bool ContainsEpsilon { get; }

    /// <summary>
    /// Gets the metadata associated with the node.
    /// </summary>
    public Dictionary<string, object> Metadata { get; internal set; }

    /// <summary>
    /// Gets the parent of the regex node.
    /// </summary>
    public RegexNode? Parent { get; internal set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexNode"/> class.
    /// </summary>
    /// <param name="metadata">The metadata associated with the node.</param>
    /// <param name="children">The children of the regex node.</param>
    /// <param name="type"> 
    protected RegexNode(
        RegexNodeType type,
        bool containsEpsilon,
        RegexNode[] children,
        Dictionary<string, object>? metadata)
    {
        Type = type;
        ContainsEpsilon = containsEpsilon;
        Parent = null;
        Metadata = metadata ?? new Dictionary<string, object>();

        foreach (var child in children)
        {
            child.SetParent(this);
        }
    }

    /*
     * Builder methods 
     */

    /// <summary>
    /// Creates a regex node representing a single character.
    /// </summary>
    /// <param name="c">The character to be represented by the regex node.</param>
    /// <returns>A <see cref="RegexNode"/> representing the specified character.</returns>
    public static RegexNode FromCharacter(char c) => new LiteralNode(c);

    /// <summary>
    /// Creates a regex node representing a union of the specified regex nodes.
    /// </summary>
    /// <param name="characters"> A string of characters to be represented by the regex node.</param>
    /// <returns>A <see cref="RegexNode"/> representing the union of the specified regex nodes.</returns>
    public static RegexNode FromString(string characters)
    {
        var nodes = characters
            .Select(c => new LiteralNode(c))
            .Cast<RegexNode>()
            .ToArray();

        return Concatenate(nodes);
    }

    /// <summary>
    /// Concatenates the specified regex nodes into a single regex node.
    /// </summary>
    /// <param name="regexes">The regex nodes to concatenate.</param>
    /// <returns>A <see cref="RegexNode"/> representing the concatenation of the specified regex nodes.</returns>
    /// <exception cref="ArgumentException">Thrown when no regex nodes are provided.</exception>
    public static RegexNode Concatenate(params RegexNode[] regexes)
    {
        if (regexes.Length == 0)
        {
            throw new ArgumentException("At least one regex is required");
        }

        if (regexes.Length == 1)
        {
            return regexes[0];
        }

        var regex = regexes[0];

        for (var i = 1; i < regexes.Length; i++)
        {
            regex = new ConcatenationNode(regex, regexes[i]);
        }

        return regex;
    }

    /// <summary>
    /// Creates a regex node representing a range of characters.
    /// </summary>
    /// <param name="start">The starting character of the range.</param>
    /// <param name="end">The ending character of the range.</param>
    /// <returns>A <see cref="RegexNode"/> representing the range of characters.</returns>
    /// <exception cref="ArgumentException">Thrown when the range does not include any characters.</exception>
    public static RegexNode FromCharacterRange(
        char start,
        char end)
    {
        var chars = new List<char>();

        for (var i = start; i <= end; i++)
        {
            chars.Add(i);
        }

        if (chars.Count == 0)
        {
            throw new ArgumentException("At least one character is required");
        }

        if (chars.Count == 1)
        {
            return new LiteralNode(chars[0]);
        }

        var literals = chars
            .Select(c => new LiteralNode(c))
            .Cast<RegexNode>()
            .ToArray();

        return literals
            .First()
            .Union(literals.Skip(1).ToArray());
    }


    /*
     * Instance methods
     */

    /// <summary>
    /// Determines whether the specified <see cref="RegexNode"/> is equal to the current <see cref="RegexNode"/>.
    /// </summary>
    /// <param name="other">The <see cref="RegexNode"/> to compare with the current <see cref="RegexNode"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="RegexNode"/> is equal to the current <see cref="RegexNode"/>; otherwise, <c>false</c>.</returns>
    public abstract bool Equals(RegexNode? other);

    /// <summary>
    /// Returns a string representation of the regex node.
    /// </summary>
    /// <returns>A string representation of the regex node.</returns>
    public abstract override string ToString();

    /// <summary>
    /// Gets the children of the regex node.
    /// </summary>
    /// <returns>A read-only list of child regex nodes.</returns>
    public abstract IReadOnlyList<RegexNode> GetChildren();
}
