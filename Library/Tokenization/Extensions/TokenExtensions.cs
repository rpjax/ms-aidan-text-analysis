using System.Runtime.CompilerServices;
using Aidan.TextAnalysis.Helpers;
using Aidan.TextAnalysis.Tokenization.Helpers;

namespace Aidan.TextAnalysis.Tokenization.Extensions;

public static class TokenExtensions
{
    public static string GetNormalizedStringValue(this OldToken token)
    {
        if (token.Type != TokenType.String)
        {
            throw new ArgumentException("Token is not a string token.");
        }

        return token.Value.ToString()[1..^1];
    }

    /// <summary>
    /// Computes the FNV-1a hash of the given token. 
    /// </summary>
    /// <remarks>
    /// The hash is computed based on the token's type or value depending on the <paramref name="useValue"/> parameter.
    /// </remarks>
    /// <param name="token"></param>
    /// <param name="useValue"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ComputeFnv1aHash(this OldToken token, bool useValue = false)
    {
        var value = useValue
            ? $"{TokenTypeHelper.ToString(token.Type)}{token.Value}"
            : TokenTypeHelper.ToString(token.Type);

        return HashHelper.ComputeFnvHash(value);
    }
}
