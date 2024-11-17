using System.Collections;

namespace Aidan.TextAnalysis.Language.Components;

public enum CharsetType
{
    Ascii,
    Unicode,
    Utf8,
    Utf16,
}

public class Charset : IReadOnlyList<char>
{
    public IReadOnlyList<char> Chars { get; }
    public int Count { get; }

    public Charset(IEnumerable<char> chars)
    {
        Chars = chars.ToArray();
        Count = Chars.Count;
    }

    public char this[int index] => Chars[index];

    public static char[] Compute(CharsetType charset)
    {
        return charset switch
        {
            CharsetType.Ascii => Enumerable.Range(0, 128)
                .Select(x => (char)x)
                .ToArray(),

            CharsetType.Unicode => Enumerable.Range(0, 0xFFFF + 1) // Generates BMP (Basic Multilingual Plane) characters only
                .Select(x => (char)x)
                .ToArray(),

            CharsetType.Utf8 => Enumerable.Range(0, 0xFFFF + 1) // BMP characters, as UTF-8 is variable-length but this is limited by char's 16-bit size
                .Select(x => (char)x)
                .ToArray(),

            CharsetType.Utf16 => Enumerable.Range(0, 0xFFFF + 1) // BMP characters in UTF-16 representation
                .Select(x => (char)x)
                .ToArray(),

            _ => throw new ArgumentOutOfRangeException(nameof(charset))
        };
    }

    public override string ToString()
    {
        return string.Join("", Chars);
    }

    public IEnumerator<char> GetEnumerator()
    {
        return Chars.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Contains(char item)
    {
        return Chars.Contains(item);
    }

    public bool Contains(IEnumerable<char> items)
    {
        return items.All(Chars.Contains);
    }

}
