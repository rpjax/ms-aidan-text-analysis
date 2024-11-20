using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;
using Aidan.TextAnalysis.RegularExpressions.Parsing;

namespace Aidan.TextAnalysis.RegularExpressions.Ast;

/// <summary>
/// Represents an abstract base class for regex nodes in the abstract syntax tree of a regular expression.
/// </summary>
public abstract class RegExpr : IEquatable<RegExpr>
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
    public RegExpr? Parent { get; internal set; }

    private int? HashCache { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegExpr"/> class.
    /// </summary>
    /// <param name="metadata">The metadata associated with the node.</param>
    /// <param name="children">The children of the regex node.</param>
    /// <param name="type"> 
    protected RegExpr(
        RegexNodeType type,
        bool containsEpsilon,
        RegExpr[] children,
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
    /// Parses a regular expression pattern and returns its abstract syntax tree representation.
    /// </summary>
    /// <param name="pattern">The string representation of the regular expression to parse.</param>
    /// <param name="charser">An optional charset to use during parsing, which can influence how character sets are interpreted.</param>
    /// <returns>A <see cref="RegExpr"/> instance representing the parsed regular expression.</returns>
    public static RegExpr FromPattern(string pattern, Charset? charset = null)
    {
        return RegExprParser.Parse(pattern, charset ?? Charset.Compute(CharsetType.ExtendedAscii));
    }

    /// <summary>
    /// Creates a regex node representing a single character.
    /// </summary>
    /// <param name="c">The character to be represented by the regex node.</param>
    /// <returns>A <see cref="RegExpr"/> representing the specified character.</returns>
    public static RegExpr FromCharacter(char c) => new LiteralNode(c);

    /// <summary>
    /// Creates a regex node representing a union of the specified regex nodes.
    /// </summary>
    /// <param name="characters"> A string of characters to be represented by the regex node.</param>
    /// <returns>A <see cref="RegExpr"/> representing the union of the specified regex nodes.</returns>
    public static RegExpr FromString(string characters)
    {
        var nodes = characters
            .Select(c => new LiteralNode(c))
            .Cast<RegExpr>()
            .ToArray();

        return Concatenation(nodes);
    }

    /// <summary>
    /// Concatenates the specified regex nodes into a single regex node.
    /// </summary>
    /// <param name="regexes">The regex nodes to concatenate.</param>
    /// <returns>A <see cref="RegExpr"/> representing the concatenation of the specified regex nodes.</returns>
    /// <exception cref="ArgumentException">Thrown when no regex nodes are provided.</exception>
    public static RegExpr Concatenation(params RegExpr[] regexes)
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
    public static RegExpr Union(params RegExpr[] regexes)
    {
        if (regexes.Length == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(regexes));
        }
        if (regexes.Length == 1)
        {
            return regexes[0];
        }

        RegExpr regex = regexes[0];

        foreach (var other in regexes.Skip(1))
        {
            regex = new UnionNode(regex, other);
        }

        return regex;
    }

    /// <summary>
    /// Creates a regex node representing a range of characters.
    /// </summary>
    /// <param name="start">The starting character of the range.</param>
    /// <param name="end">The ending character of the range.</param>
    /// <returns>A <see cref="RegExpr"/> representing the range of characters.</returns>
    /// <exception cref="ArgumentException">Thrown when the range does not include any characters.</exception>
    public static RegExpr FromCharacterRange(
        char start,
        char end)
    {
        var chars = new List<char>();
        var _start = (int)start;
        var _end = (int)end;

        for (int i = _start; i <= _end; i++)
        {
            chars.Add((char)i);
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
            .Cast<RegExpr>()
            .ToArray();

        return literals
            .First()
            .Union(literals.Skip(1).ToArray());
    }

    /*
     * Instance methods
     */

    /// <summary>
    /// Returns a string representation of the regex node.
    /// </summary>
    /// <returns>A string representation of the regex node.</returns>
    public abstract override string ToString();

    /// <summary>
    /// Determines whether the specified <see cref="RegExpr"/> is equal to the current <see cref="RegExpr"/>.
    /// </summary>
    /// <param name="other">The <see cref="RegExpr"/> to compare with the current <see cref="RegExpr"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="RegExpr"/> is equal to the current <see cref="RegExpr"/>; otherwise, <c>false</c>.</returns>
    public abstract bool Equals(RegExpr? other);

    /// <summary>
    /// Gets the children of the regex node.
    /// </summary>
    /// <returns>A read-only list of child regex nodes.</returns>
    public abstract IReadOnlyList<RegExpr> GetChildren();

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current <see cref="RegExpr"/>.</returns>
    public override int GetHashCode()
    {
        if (HashCache is not null)
        {
            return HashCache.Value;
        }

        object[] terms;

        switch (Type)
        {
            case RegexNodeType.Epsilon:
                terms = new object[] { Type };
                break;

            case RegexNodeType.EmptySet:
                terms = new object[] { Type };
                break;

            case RegexNodeType.Literal:
                terms = new object[] { Type, this.AsLiteral().Character.ToString() };
                break;

            case RegexNodeType.Union:
            case RegexNodeType.Concatenation:
            case RegexNodeType.Star:
                terms = new object[] { Type }.Concat(GetChildren()).ToArray();
                break;

            case RegexNodeType.Anything:
                terms = new object[] { Type, this.AsAnything().Charset };
                break;

            case RegexNodeType.Class:
                terms = new object[] { Type, this.AsClass().ComputeResultingCharset() };
                break;

            default:
                throw new InvalidOperationException("Unknown node type");
        }

        unchecked
        {
            int hash = 17;

            foreach (var term in terms)
            {
                hash = hash * 23 + term.GetHashCode();
            }

            HashCache = hash;
            return hash;
        }
    }
}
