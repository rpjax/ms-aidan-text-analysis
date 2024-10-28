using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.Helpers;

/// <summary>
/// Provides helper methods for computing hash values.
/// </summary>
public static class HashHelper
{
    /// <summary>
    /// The prime number used in the FNV-1a hash algorithm.
    /// </summary>
    public const uint FnvPrime = 16777619;

    /// <summary>
    /// The offset basis value used in the FNV-1a hash algorithm.
    /// </summary>
    public const uint FnvOffsetBasis = 2166136261;

    /// <summary>
    /// Computes the FNV-1a hash of the given string.
    /// </summary>
    /// <param name="value">The input string to hash.</param>
    /// <returns>The computed FNV-1a hash value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ComputeFnvHash(IEnumerable<char> value)
    {
        uint hash = FnvOffsetBasis;

        foreach (char c in value)
        {
            hash ^= c;
            hash *= FnvPrime;
        }

        return hash;
    }
}
