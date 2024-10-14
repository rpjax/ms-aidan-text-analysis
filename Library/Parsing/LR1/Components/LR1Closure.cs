namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents the closure of LR(1) items.
/// </summary>
public class LR1Closure : LR1ItemCollection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LR1Closure"/> class.
    /// </summary>
    /// <param name="items">The LR(1) items that make up the closure.</param>
    public LR1Closure(LR1Item[] items) : base(items)
    {
    }
}
