using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Components.Symbols;

namespace Aidan.TextAnalysis.Language.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="ISentence"/> interface.
/// </summary>
public static class ISentenceExtensions
{
    /// <summary>
    /// Adds a symbol to the end of the sentence.
    /// </summary>
    /// <param name="self">The sentence to which the symbol will be added.</param>
    /// <param name="symbol">The symbol to add.</param>
    /// <returns>A new sentence with the symbol added.</returns>
    public static ISentence Add(this ISentence self, ISymbol symbol)
    {
        var list = new List<ISymbol>(self.ToArray());
        list.Add(symbol);
        return new Sentence(list.ToArray());
    }

    /// <summary>
    /// Gets a range of symbols from the sentence.
    /// </summary>
    /// <param name="self">The sentence from which to get the range.</param>
    /// <param name="start">The zero-based starting index of the range.</param>
    /// <param name="count">The number of symbols to include in the range.</param>
    /// <returns>A new sentence containing the specified range of symbols.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the start index or count is out of range.
    /// </exception>
    public static ISentence GetRange(this ISentence self, int start, int count)
    {
        if (start < 0 || start >= self.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(start), "The start index is out of range.");
        }
        if (count < 0 || start + count > self.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The count is out of range.");
        }

        var body = self
            .Skip(start)
            .Take(count)
            .ToArray();

        return new Sentence(body);
    }

    /// <summary>
    /// Splits the sentence into an array of sentences based on the specified separator symbol.
    /// </summary>
    /// <param name="self">The sentence to split.</param>
    /// <param name="separator">The symbol to use as a separator.</param>
    /// <returns>An array of sentences split by the separator symbol.</returns>
    public static ISentence[] Split(
        this ISentence self,
        ISymbol separator)
    {
        var sentences = new List<ISentence>();
        var current = new List<ISymbol>();

        foreach (var symbol in self)
        {
            if (symbol.Equals(separator))
            {
                sentences.Add(new Sentence(current.ToArray()));
                current.Clear();
            }
            else
            {
                current.Add(symbol);
            }
        }

        if (current.Count > 0)
        {
            sentences.Add(new Sentence(current.ToArray()));
        }

        return sentences.ToArray();
    }
}
