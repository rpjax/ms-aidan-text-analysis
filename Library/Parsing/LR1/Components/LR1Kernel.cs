namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents a LR(1) item collection that is used as a kernel for a state.
/// </summary>
public class LR1Kernel : LR1ItemCollection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LR1Kernel"/> class with the specified items.
    /// </summary>
    /// <param name="items">The LR(1) items to include in the kernel.</param>
    public LR1Kernel(params LR1Item[] items) : base(items)
    {
    }
}
