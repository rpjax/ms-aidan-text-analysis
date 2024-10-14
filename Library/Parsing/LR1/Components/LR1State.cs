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
    /// <summary>
    /// Gets the kernel of the LR(1) state.
    /// </summary>
    public LR1Kernel Kernel { get; }

    /// <summary>
    /// Gets the closure of the LR(1) state.
    /// </summary>
    public LR1Closure Closure { get; }

    /// <summary>
    /// Gets the items of the LR(1) state.
    /// </summary>
    public LR1Item[] Items { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LR1State"/> class.
    /// </summary>
    /// <param name="kernel">The kernel of the LR(1) state.</param>
    /// <param name="closure">The closure of the LR(1) state.</param>
    /// <exception cref="ArgumentException">Thrown when the kernel array is empty.</exception>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="LR1State"/> class.
    /// </summary>
    /// <param name="kernel">The kernel items of the LR(1) state.</param>
    /// <param name="closure">The closure items of the LR(1) state.</param>
    public LR1State(
        LR1Item[] kernel,
        LR1Item[] closure)
        : this(new LR1Kernel(kernel), new LR1Closure(closure))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LR1State"/> class.
    /// </summary>
    /// <param name="kernel">The kernel item of the LR(1) state.</param>
    /// <param name="closure">The closure items of the LR(1) state.</param>
    public LR1State(
        LR1Item kernel,
        LR1Item[] closure)
        : this(new[] { kernel }, closure)
    {
    }

    /// <summary>
    /// Gets the LR(1) item at the specified index.
    /// </summary>
    /// <param name="index">The index of the item.</param>
    /// <returns>The LR(1) item at the specified index.</returns>
    public LR1Item this[int index] => Items[index];

    /// <summary>
    /// Gets the signature of the LR(1) state.
    /// </summary>
    public string Signature => GetSignature();

    /// <summary>
    /// Gets a value indicating whether the LR(1) state is a final state.
    /// </summary>
    public bool IsFinalState => GetIsFinalState();

    /// <summary>
    /// Gets the signature of the LR(1) state.
    /// </summary>
    /// <param name="kernel">The kernel items to get the signature for.</param>
    /// <param name="useLookaheads">A value indicating whether to use lookaheads in the signature.</param>
    /// <returns>The signature of the LR(1) state.</returns>
    public static string GetSignature(IEnumerable<LR1Item> kernel, bool useLookaheads = true)
    {
        var signatures = kernel
            .Select(x => x.GetSignature(useLookaheads))
            .ToArray();

        return string.Join("; ", signatures);
    }

    /// <summary>
    /// Returns a string that represents the current LR(1) state.
    /// </summary>
    /// <returns>A string that represents the current LR(1) state.</returns>
    public override string ToString()
    {
        var kernelStr = string.Join("\n", Kernel.Select(x => x.ToString()));
        var closureStr = string.Join("\n", Closure.Select(x => x.ToString()));

        return $"Kernel:\n{kernelStr}\n Closure:\n{closureStr}";
    }

    /// <summary>
    /// Determines whether the LR(1) state is an accepting state.
    /// </summary>
    /// <param name="set">The production set to check against.</param>
    /// <returns><c>true</c> if the state is an accepting state; otherwise, <c>false</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the production set does not have an augmented production.</exception>
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

    /// <summary>
    /// Returns the hash code for the current LR(1) state.
    /// </summary>
    /// <returns>The hash code for the current LR(1) state.</returns>
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

    /// <summary>
    /// Determines whether the specified LR(1) state is equal to the current LR(1) state.
    /// </summary>
    /// <param name="other">The LR(1) state to compare with the current LR(1) state.</param>
    /// <returns><c>true</c> if the specified LR(1) state is equal to the current LR(1) state; otherwise, <c>false</c>.</returns>
    public bool Equals(LR1State? other)
    {
        return other is not null
            && other.GetSignature(useLookaheads: true) == GetSignature(useLookaheads: true);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current LR(1) state.
    /// </summary>
    /// <param name="obj">The object to compare with the current LR(1) state.</param>
    /// <returns><c>true</c> if the specified object is equal to the current LR(1) state; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as LR1State);
    }

    /*
     * private methods.
     */

    /// <summary>
    /// Gets the signature of the LR(1) state.
    /// </summary>
    /// <param name="useLookaheads">A value indicating whether to use lookaheads in the signature.</param>
    /// <returns>The signature of the LR(1) state.</returns>
    private string GetSignature(bool useLookaheads = true)
    {
        return GetSignature(Kernel, useLookaheads);
    }

    /// <summary>
    /// Gets a value indicating whether the LR(1) state is a final state.
    /// </summary>
    /// <returns><c>true</c> if the state is a final state; otherwise, <c>false</c>.</returns>
    private bool GetIsFinalState()
    {
        return Kernel.Any(item => item.Symbol is null);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the LR(1) items.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the LR(1) items.</returns>
    public IEnumerator<LR1Item> GetEnumerator()
    {
        return ((IEnumerable<LR1Item>)Items).GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the LR(1) items.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the LR(1) items.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
