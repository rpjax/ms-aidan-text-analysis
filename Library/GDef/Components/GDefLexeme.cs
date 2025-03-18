using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.RegularExpressions.Ast;

namespace Aidan.TextAnalysis.GDef.Components;

public class GDefLexeme
{
    public bool IsIgnored { get; }
    public Charset Charset { get; }
    public string Name { get; }
    public RegExpr Pattern { get; }

    public GDefLexeme(
        bool isIgnored,
        Charset charset,
        string name,
        RegExpr pattern)
    {
        IsIgnored = isIgnored;
        Charset = charset;
        Name = name;
        Pattern = pattern;
    }
}