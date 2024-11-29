using Aidan.TextAnalysis.Parsing.Tree;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

public class LR1ParserContext
{
    public ILR1InputStream InputStream { get; }
    public ILR1Stack Stack { get; }
    public CstBuilder CstBuilder { get; }

    public LR1ParserContext(
        ILR1InputStream inputStream,
        ILR1Stack stack,
        CstBuilder cstBuilder)
    {
        InputStream = inputStream;
        Stack = stack;
        CstBuilder = cstBuilder;
    }
}
