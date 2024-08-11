using System.Globalization;
using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.Tokenization.Components;

/// <summary>
/// Represents the lexical alphabet used by the tokenizer.
/// </summary>
public static class TokenizerAlphabet
{
    /// <summary>
    /// The number of characters in the Unicode character set version 15.1.0.
    /// </summary>
    /// <remarks> Ref: https://www.unicode.org/versions/Unicode15.1.0/ </remarks>
    public const int UnicodeCharCount = 149_813;

    public const char Escape = '\\';
    public const char SingleQuote = '\'';
    public const char DoubleQuote = '"';

    public static char Underline { get; } = '_';
    public static char[] StringDelimiters { get; } = new char[] { SingleQuote, DoubleQuote };

    public static CharType[] CharTypeLookupTable { get; } = new CharType[UnicodeCharCount];

    static TokenizerAlphabet()
    {
        InitLookupTable();
    }

    public static void InitLookupTable()
    {
        for (int i = 0; i < UnicodeCharCount; i++)
        {
            var c = (char)i;
            CharTypeLookupTable[i] = ComputeCharType(c);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CharType LookupCharType(char c)
    {
        return CharTypeLookupTable[c];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDigit(char c)
    {
        return char.IsDigit(c);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLetter(char c)
    {
        return LookupCharType(c) == CharType.Letter;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDigitOrLetter(char c)
    {
        var type = LookupCharType(c);

        return type == CharType.Digit 
            || type == CharType.Letter;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPunctuation(char c)
    {
        return LookupCharType(c) == CharType.Punctuation;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidHexadecimal(char c)
    {
        return char.IsDigit(c)
            || (c >= 'A' && c <= 'F')
            || (c >= 'a' && c <= 'f');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUnderline(char c)
    {
        return c == Underline;
    }

    /*
     * private helpers
     */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CharType ComputeCharType(char c)
    {
        if (ComputeIsDigit(c))
        {
            return CharType.Digit;
        }

        if (ComputeIsLetter(c))
        {
            return CharType.Letter;
        }

        /*
         * check for string delimiters must be done before checking for punctuation, because string delimiters are also punctuation.
         */

        if (c == '\'' || c == '"')
        {
            return CharType.StringDelimiter;
        }

        if (ComputeIsPunctuation(c))
        {
            return CharType.Punctuation;
        }

        if (ComputeIsWhiteSpace(c))
        {
            return CharType.Whitespace;
        }

        if (ComputeIsControl(c))
        {
            return CharType.Control;
        }

        return CharType.Unknown;
    }

    private static bool ComputeIsDigit(char c)
    {
        return char.IsDigit(c);
    }

    private static bool ComputeIsLetter(char c)
    {
        return char.IsLetter(c);
    }

    private static bool ComputeIsPunctuation(char c)
    {
        switch (char.GetUnicodeCategory(c))
        {
            // Punctuation categories
            case UnicodeCategory.ConnectorPunctuation:
            case UnicodeCategory.DashPunctuation:
            case UnicodeCategory.OpenPunctuation:
            case UnicodeCategory.ClosePunctuation:
            case UnicodeCategory.InitialQuotePunctuation:
            case UnicodeCategory.FinalQuotePunctuation:
            case UnicodeCategory.OtherPunctuation:
                return true;

            // Symbol categories that are commonly used as operators
            case UnicodeCategory.MathSymbol:
            case UnicodeCategory.CurrencySymbol:
            case UnicodeCategory.ModifierSymbol:
            case UnicodeCategory.OtherSymbol:
                return true;

            default:
                return false;
        }
    }

    private static bool ComputeIsWhiteSpace(char c)
    {
        return char.IsWhiteSpace(c);
    }

    private static bool ComputeIsControl(char c)
    {
        return char.IsControl(c);
    }

    private static bool ComputeIsStringDelimiter(char c)
    {
        return StringDelimiters.Contains(c);
    }

    private static bool ComputeIsEscape(char c)
    {
        return c == Escape;
    }

    private static bool ComputeIsUnderline(char c)
    {
        return c == Underline;
    }

    private static bool ComputeIsHexadecimal(char c)
    {
        return char.IsDigit(c)
            || (c >= 'A' && c <= 'F')
            || (c >= 'a' && c <= 'f');
    }

}
