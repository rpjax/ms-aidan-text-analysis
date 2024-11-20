using System.Collections;

namespace Aidan.TextAnalysis.Language.Components;

public enum CharsetType
{
    Ascii,
    ExtendedAscii,
    Unicode,
    Utf8,
    Utf16,
}

public class Charset : IReadOnlyList<char>, IEquatable<Charset>
{
    public IReadOnlyList<char> Chars { get; }
    public int Count { get; }

    private HashSet<char> HashSet { get; }

    public Charset(IEnumerable<char> chars)
    {
        Chars = chars
            .Distinct()
            .OrderBy(c => c)
            .ToArray();
        Count = Chars.Count;
        HashSet = new HashSet<char>(Chars);
    }

    public char this[int index] => Chars[index];

    public static Charset Compute(CharsetType charset)
    {
        char[] chars;

        switch (charset)
        {
            case CharsetType.Ascii:
                chars = Enumerable.Range(0, 128)
                    .Select(x => (char)x)
                    .ToArray();
                break;

            case CharsetType.ExtendedAscii:
                chars = Enumerable.Range(0, 256)
                    .Select(x => (char)x)
                    .ToArray();
                break;

            case CharsetType.Unicode:
                chars = Enumerable.Range(0, 0xFFFF + 1) // Generates BMP (Basic Multilingual Plane) characters only
                    .Select(x => (char)x)
                    .ToArray();
                break;

            case CharsetType.Utf8:
                chars = Enumerable.Range(0, 0xFFFF + 1) // BMP characters, as UTF-8 is variable-length but this is limited by char's 16-bit size
                    .Select(x => (char)x)
                    .ToArray();
                break;

            case CharsetType.Utf16:
                chars = Enumerable.Range(0, 0xFFFF + 1) // BMP characters in UTF-16 representation
                    .Select(x => (char)x)
                    .ToArray();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(charset));
        }

        return new Charset(chars);
    }

    public override string ToString()
    {
        return string.Join("", Chars);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;

            foreach (var c in Chars)
            {
                hash = hash * 23 + c.GetHashCode();
            }
            return hash;
        }
    }

    public IEnumerator<char> GetEnumerator()
    {
        return Chars.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Equals(Charset? other)
    {
        return other is not null
            && other.Chars.SequenceEqual(Chars);
    }

    public bool Contains(char item)
    {
        return HashSet.Contains(item);
    }

    public bool Contains(IEnumerable<char> items)
    {
        return items.All(Chars.Contains);
    }

}
