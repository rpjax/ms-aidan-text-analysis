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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComputeHash(params object[] terms)
    {
        if (terms.Length == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(terms));
        }

        int[] termHashes = terms
            .Select(term => term.GetHashCode())
            .OrderBy(hash => hash)
            .ToArray();

        unchecked
        {
            int hash = 17;

            foreach (var termHash in termHashes)
            {
                hash = hash * 23 + termHash;
            }

            return hash;
        }
    }

}
