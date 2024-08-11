namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents a LR(1) item collection that is used as a kernel for a state.
/// </summary>
public class LR1Kernel : LR1ItemCollection
{
    public LR1Kernel(params LR1Item[] items) : base(items)
    {
    }
}
