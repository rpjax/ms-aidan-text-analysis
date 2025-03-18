using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.RegularExpressions.Ast.Enums;

namespace Aidan.TextAnalysis.RegularExpressions.Ast;

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
