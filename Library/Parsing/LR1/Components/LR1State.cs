using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;
using System.Collections;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Defines a class that represents an LR(1) state.
/// </summary>
public class LR1State :
    IEquatable<LR1State>,
    IEnumerable<LR1Item>
{
    public LR1Kernel Kernel { get; }
    public LR1Closure Closure { get; }
    public LR1Item[] Items { get; }

    public LR1State(LR1Kernel kernel, LR1Closure closure)
    {
        if (kernel.Length == 0)
        {
            throw new ArgumentException("The kernel array must not be empty.");
        }

        Kernel = kernel;
        Closure = closure;
        Items = kernel
            .Concat(closure)
            .ToArray();
    }

    public LR1State(LR1Item[] kernel, LR1Item[] closure) : this(new LR1Kernel(kernel), new LR1Closure(closure))
    {
    }

    public LR1State(LR1Item kernel, LR1Item[] closure) : this(new[] { kernel }, closure)
    {
    }

    public LR1Item this[int index] => Items[index];

    public string Signature => GetSignature();
    public bool IsFinalState => GetIsFinalState();

    public static string GetSignature(IEnumerable<LR1Item> kernel, bool useLookaheads = true)
    {
        var signatures = kernel
            .Select(x => x.GetSignature(useLookaheads))
            .ToArray();

        return string.Join("; ", signatures);
    }

    public override string ToString()
    {
        var kernelStr = string.Join("\n", Kernel.Select(x => x.ToString()));
        var closureStr = string.Join("\n", Closure.Select(x => x.ToString()));

        return $"Kernel:\n{kernelStr}\n Closure:\n{closureStr}";
    }

    public bool IsAcceptingState(ProductionSet set)
    {
        if (!IsFinalState)
        {
            return false;
        }

        var augmentedProduction = set.TryGetAugmentedStartProduction();

        if (augmentedProduction is null)
        {
            throw new InvalidOperationException("The production set does not have an augmented production.");
        }

        return Kernel[0].Production == augmentedProduction;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;

            foreach (var item in Kernel)
            {
                hash = hash * 23 + item.GetHashCode();
            }

            return hash;
        }
    }

    public bool Equals(LR1State? other)
    {
        return other is not null
            && other.GetSignature(useLookaheads: true) == GetSignature(useLookaheads: true);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as LR1State);
    }

    /*
     * private methods.
     */

    private string GetSignature(bool useLookaheads = true)
    {
        return GetSignature(Kernel, useLookaheads);
    }

    private bool GetIsFinalState()
    {
        return Kernel.Any(item => item.Symbol is null);
    }

    public IEnumerator<LR1Item> GetEnumerator()
    {
        return ((IEnumerable<LR1Item>)Items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
