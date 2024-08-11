using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Tokenization.Tools;
using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.Language.Extensions;

public static class NonTerminalExtensions
{
    public static string ToSententialNotation(this NonTerminal nonTerminal)
    {
        return nonTerminal.Name;
    }

    public static string ToBnfNotation(this NonTerminal nonTerminal)
    {
        return $"<{nonTerminal.Name}>";
    }

    public static string ToEbnfNotation(this NonTerminal nonTerminal)
    {
        return nonTerminal.Name;
    }

    public static string ToEbnfKleeneNotation(this NonTerminal nonTerminal)
    {
        return nonTerminal.Name;
    }


    /// <summary>
    /// Computes the FNV-1a hash of the given non-terminal. 
    /// </summary>
    /// <remarks>
    /// The hash is computed using the name of the non-terminal.
    /// </remarks>
    /// <param name="terminal"></param>
    /// <param name="useValue"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ComputeFnv1aHash(this NonTerminal nonTerminal)
    {
        return TokenHashHelper.ComputeFnvHash(nonTerminal.Name);
    }

}

public static class INonTerminalExtensions
{
    public static string ToSententialNotation(this INonTerminal nonTerminal)
    {
        return nonTerminal.Name;
    }

    public static string ToBnfNotation(this INonTerminal nonTerminal)
    {
        return $"<{nonTerminal.Name}>";
    }

    public static string ToEbnfNotation(this INonTerminal nonTerminal)
    {
        return nonTerminal.Name;
    }

    public static string ToEbnfKleeneNotation(this INonTerminal nonTerminal)
    {
        return nonTerminal.Name;
    }

}

