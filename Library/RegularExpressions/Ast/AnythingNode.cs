using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.RegularExpressions.Tree;

public class AnythingNode : RegExpr
{
    public Charset Charset { get; }

    public AnythingNode(Charset charset)
        : base(
            type: RegexNodeType.Anything,
            containsEpsilon: false,
            children: Array.Empty<RegExpr>(),
            metadata: null)
    {
        Charset = charset;
    }

    public override string ToString()
    {
        return ".";
    }

    public override bool Equals(RegExpr? other)
    {
        return other is AnythingNode anythingNode
            && Charset.Equals(anythingNode.Charset);
    }

    public override IReadOnlyList<RegExpr> GetChildren()
    {
        return Array.Empty<RegExpr>();
    }
}
