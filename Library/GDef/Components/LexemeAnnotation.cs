using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.GDef.Components;

public class LexemeAnnotation
{
    public Charset Charset { get; }
    public bool IsIgnored { get; }

    public LexemeAnnotation(Charset charset, bool isIgnored)
    {
        Charset = charset;
        IsIgnored = isIgnored;
    }
}