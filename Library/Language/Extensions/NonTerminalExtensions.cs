using System.Runtime.CompilerServices;
using Aidan.TextAnalysis.Helpers;
using Aidan.TextAnalysis.Language.Components.Symbols;

namespace Aidan.TextAnalysis.Language.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="INonTerminal"/> interface.
/// </summary>
public static class INonTerminalExtensions
{
    /// <summary>
    /// Computes the FNV-1a hash for the name of the non-terminal symbol.
    /// </summary>
    /// <param name="nonTerminal">The non-terminal symbol to compute the hash for.</param>
    /// <returns>The FNV-1a hash of the non-terminal symbol's name.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ComputeFnv1aHash(this INonTerminal nonTerminal)
    {
        return HashHelper.ComputeFnvHash(nonTerminal.Name);
    }
}
