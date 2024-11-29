using Aidan.TextAnalysis.Language.Components;
using System.Text;

namespace Aidan.TextAnalysis.RegularExpressions.Tree;

public enum ClassChildType
{
    Literal,
    CharacterRange,
    CharacterSet
}

public abstract class ClassChild : IEquatable<ClassChild>
{
    public ClassChildType Type { get; }

    protected ClassChild(ClassChildType type)
    {
        Type = type;
    }

    public abstract bool Equals(ClassChild? other);

    public abstract IEnumerable<char> GetLiterals();
}

public class ClassNode : RegExpr
{
    public Charset Charset { get; }
    public bool IsNegated { get; }
    public ClassChild[] Children { get; }

    private Charset? ResultingCharsetCache { get; set; }

    public ClassNode(
        Charset charset,
        bool isNegated,
        IEnumerable<ClassChild> children)
        : base(
            type: RegexNodeType.Class,
            containsEpsilon: false,
            children: Array.Empty<RegExpr>(),
            metadata: null)
    {
        Charset = charset;
        IsNegated = isNegated;
        Children = children.ToArray();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append('[');
        
        if (IsNegated)
        {
            sb.Append('^');
        }

        foreach (var child in Children)
        {
            sb.Append(child.ToString());
        }

        sb.Append(']');
        return sb.ToString();
    }

    public override bool Equals(RegExpr? other)
    {
        return other is ClassNode classNode
            && classNode.IsNegated == IsNegated
            && classNode.Children.SequenceEqual(Children)
            && classNode.Charset.Equals(Charset);
    }

    public override IReadOnlyList<RegExpr> GetChildren()
    {
        return Array.Empty<RegExpr>();
    }

    public Charset ComputeResultingCharset()
    {
        if (ResultingCharsetCache is not null)
        {
            return ResultingCharsetCache;
        }

        var chars = new List<char>();

        foreach (var child in Children)
        {
            chars.AddRange(child.GetLiterals());
        }

        if (IsNegated)
        {
            chars = Charset
                .Except(chars)
                .ToList();
        }

        ResultingCharsetCache = new Charset(chars);
        return ResultingCharsetCache;
    }
}
