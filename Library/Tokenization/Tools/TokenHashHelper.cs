using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.Tokenization.Tools;

public static class TokenHashHelper
{
    public const uint FnvPrime = 16777619;
    public const uint FnvOffsetBasis = 2166136261;

    /// <summary>
    /// Computes the FNV-1a hash of the given string.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
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
