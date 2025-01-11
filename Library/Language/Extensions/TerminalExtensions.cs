using Aidan.TextAnalysis.Helpers;
using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Language.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="ITerminal"/> interface.
/// </summary>
public static class ITerminalExtensions
{
    /// <summary>
    /// Computes the FNV-1a hash for the terminal's value.
    /// </summary>
    /// <param name="terminal">The terminal whose value will be hashed.</param>
    /// <param name="useValue">Indicates whether to use the terminal's value for hashing.</param>
    /// <returns>The computed FNV-1a hash.</returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static uint ComputeFnv1aHash(this ITerminal terminal, bool useValue = false)
    //{
    //    return HashHelper.ComputeFnvHash(terminal.Value.ToArray());
    //}
}

