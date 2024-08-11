using Aidan.Core.Extensions;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Tokenization;
using Aidan.TextAnalysis.Tokenization.Tools;
using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.Language.Extensions;

public static class TerminalExtensions
{
    public static string ToSententialNotation(this Terminal terminal)
    {
        var typeStr = terminal.Type.ToString();

        if (!string.IsNullOrEmpty(terminal.Value))
        {
            return $"\"{terminal.Value}\"";
        }

        return typeStr.ToCamelCase();
    }

    public static string ToBnfNotation(this Terminal terminal)
    {
        var typeStr = terminal.Type.ToString();

        if (!string.IsNullOrEmpty(terminal.Value))
        {
            return $"\"{terminal.Value}\"";
        }

        return typeStr;
    }

    public static string ToEbnfNotation(this Terminal terminal)
    {
        var typeStr = terminal.Type.ToString();

        if (!string.IsNullOrEmpty(terminal.Value))
        {
            return $"\"{terminal.Value}\"";
        }

        return typeStr;
    }

    public static string ToEbnfKleeneNotation(this Terminal terminal)
    {
        var typeStr = terminal.Type.ToString();

        if (!string.IsNullOrEmpty(terminal.Value))
        {
            return $"\"{terminal.Value}\"";
        }

        return typeStr;
    }

    /// <summary>
    /// Computes the FNV-1a hash of the given terminal. 
    /// </summary>
    /// <remarks>
    /// The hash is computed based on the terminal's <see cref="TokenType"/> or value depending on the <paramref name="useValue"/> parameter.
    /// <br/>
    /// If <paramref name="useValue"/> is <see langword="true"/>, but the terminal's value is <see langword="null"/>, the hash is computed based on the terminal's type.
    /// </remarks>
    /// <param name="terminal"></param>
    /// <param name="useValue"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ComputeFnv1aHash(this Terminal terminal, bool useValue = false)
    {
        var value = useValue
            ? $"{TokenTypeHelper.ToString(terminal.Type)}{terminal.Value?.ToString()}"
            : TokenTypeHelper.ToString(terminal.Type);

        return TokenHashHelper.ComputeFnvHash(value);
    }
}

public static class ITerminalExtensions
{
    public static string ToSententialNotation(this ITerminal terminal)
    {
        var typeStr = terminal.Type.ToString();

        if (!string.IsNullOrEmpty(terminal.Value))
        {
            return $"\"{terminal.Value}\"";
        }

        return typeStr.ToCamelCase();
    }

    public static string ToBnfNotation(this ITerminal terminal)
    {
        var typeStr = terminal.Type.ToString();

        if (!string.IsNullOrEmpty(terminal.Value))
        {
            return $"\"{terminal.Value}\"";
        }

        return typeStr;
    }

    public static string ToEbnfNotation(this ITerminal terminal)
    {
        var typeStr = terminal.Type.ToString();

        if (!string.IsNullOrEmpty(terminal.Value))
        {
            return $"\"{terminal.Value}\"";
        }

        return typeStr;
    }

    public static string ToEbnfKleeneNotation(this ITerminal terminal)
    {
        var typeStr = terminal.Type.ToString();

        if (!string.IsNullOrEmpty(terminal.Value))
        {
            return $"\"{terminal.Value}\"";
        }

        return typeStr;
    }

    public static string GetNormalizedValue(this ITerminal terminal)
    {
        if (terminal.Value is null)
        {
            throw new InvalidOperationException();
        }
        if(terminal.Type is not TokenType.String)
        {
            return terminal.Value;
        }

        return terminal.Value[1..^1];
    }

}

