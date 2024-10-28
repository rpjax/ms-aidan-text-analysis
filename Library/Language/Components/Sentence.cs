using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents an immutable sequence of <see cref="ISymbol"/> that form a sentence.
/// </summary>
public interface ISentence :
    IEnumerable<ISymbol>,
    IEquatable<ISentence>
{
    /// <summary>
    /// Gets the length of the sentence.
    /// </summary>
    int Length { get; }

    /// <summary>
    /// Gets the <see cref="ISymbol"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the symbol to get.</param>
    /// <returns>The <see cref="ISymbol"/> at the specified index.</returns>
    ISymbol this[int index] { get; }

    /// <summary>
    /// Returns a string that represents the current sentence.
    /// </summary>
    /// <returns>A string that represents the current sentence.</returns>
    string ToString();
}

/// <summary>
/// Represents an immutable sequence of <see cref="ISymbol"/> that form a sentence.
/// </summary>
public class Sentence : ISentence
{
    /// <summary>
    /// Gets the length of the sentence.
    /// </summary>
    public int Length => Symbols.Length;

    /// <summary>
    /// Gets the symbols that form the sentence.
    /// </summary>
    private ISymbol[] Symbols { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sentence"/> class that is empty.
    /// </summary>
    public Sentence()
    {
        Symbols = Array.Empty<ISymbol>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sentence"/> class with the specified symbols.
    /// </summary>
    /// <param name="symbols">The symbols that form the sentence.</param>
    public Sentence(params ISymbol[] symbols)
    {
        Symbols = ExpandPipeMacros(symbols);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sentence"/> class with the specified symbols.
    /// </summary>
    /// <param name="symbols">The symbols that form the sentence.</param>
    public Sentence(IEnumerable<ISymbol> symbols)
    {
        Symbols = ExpandPipeMacros(symbols.ToArray());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sentence"/> class with the specified symbol and additional symbols.
    /// </summary>
    /// <param name="symbol">The first symbol of the sentence.</param>
    /// <param name="symbols">The additional symbols that form the sentence.</param>
    public Sentence(ISymbol symbol, params ISymbol[] symbols)
    {
        symbols = Array.Empty<ISymbol>()
            .Append(symbol)
            .Concat(symbols)
            .ToArray();
        Symbols = ExpandPipeMacros(symbols);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sentence"/> class with the specified segments of symbols.
    /// </summary>
    /// <param name="firstSegment">The first segment of symbols.</param>
    /// <param name="secondSegment">The second segment of symbols.</param>
    public Sentence(IEnumerable<ISymbol> firstSegment, IEnumerable<ISymbol> secondSegment)
    {
        var symbols = Array.Empty<Symbol>()
            .Concat(firstSegment)
            .Concat(secondSegment)
            .ToArray();
        Symbols = ExpandPipeMacros(symbols);
    }

    /// <summary>
    /// Gets the <see cref="ISymbol"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the symbol to get.</param>
    /// <returns>The <see cref="ISymbol"/> at the specified index.</returns>
    public ISymbol this[int index]
    {
        get => Symbols[index];
    }

    /// <summary>
    /// Returns a string that represents the current sentence.
    /// </summary>
    /// <returns>A string that represents the current sentence.</returns>
    public override string ToString()
    {
        return string.Join(" ", this.Select(x => x.ToString()));
    }

    /// <summary>
    /// Returns an enumerator that iterates through the symbols in the sentence.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the symbols in the sentence.</returns>
    public IEnumerator<ISymbol> GetEnumerator()
    {
        return Symbols.AsEnumerable().GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the symbols in the sentence.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the symbols in the sentence.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return Symbols.GetEnumerator();
    }

    /// <summary>
    /// Determines whether the specified <see cref="ISentence"/> is equal to the current sentence.
    /// </summary>
    /// <param name="other">The sentence to compare with the current sentence.</param>
    /// <returns><c>true</c> if the specified sentence is equal to the current sentence; otherwise, <c>false</c>.</returns>
    public bool Equals(ISentence? other)
    {
        if (Length != other?.Length)
        {
            return false;
        }

        for (int i = 0; i < Length; i++)
        {
            if (!this[i].Equals(other[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Expands pipe macros in the specified symbols.
    /// </summary>
    /// <param name="symbols">The symbols to expand.</param>
    /// <returns>The expanded symbols.</returns>
    private static ISymbol[] ExpandPipeMacros(ISymbol[] symbols)
    {
        if (symbols.All(x => x is not PipeMacro))
        {
            return symbols;
        }

        var pipeIndexes = symbols
            .Select((x, i) => (x, i))
            .Where(x => x.x is PipeMacro)
            .Select(x => x.i)
            .ToList();

        var alternatives = new List<Sentence>();
        var start = 0;

        pipeIndexes.Add(symbols.Length);

        foreach (var index in pipeIndexes)
        {
            var end = index;
            var length = end - start;

            var alternative = new Sentence(symbols.Skip(start).Take(length).ToArray());
            alternatives.Add(alternative);

            start = end + 1;
        }

        var alternativeMacro = new AlternativeMacro(alternatives.ToArray());

        return new ISymbol[] { alternativeMacro };
    }

    /// <summary>
    /// Creates a copy of the current sentence.
    /// </summary>
    /// <returns>A new <see cref="Sentence"/> that is a copy of the current sentence.</returns>
    public Sentence Copy()
    {
        return new Sentence(Symbols.ToArray());
    }
}
