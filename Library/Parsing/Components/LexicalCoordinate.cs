namespace Aidan.TextAnalysis.Parsing.Components;

/// <summary>
/// Represents a lexical coordinate in a text document. <br/>
/// It includes the index, line number, and column number of the coordinate. Eg. (Index: 0, Line: 1, Column: 1)
/// </summary>
public readonly struct LexicalCoordinate
{
    /// <summary>
    /// Gets the index of the coordinate.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the line number of the coordinate.
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// Gets the column number of the coordinate.
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LexicalCoordinate"/> struct.
    /// </summary>
    /// <param name="index">The index of the coordinate.</param>
    /// <param name="line">The line number of the coordinate.</param>
    /// <param name="column">The column number of the coordinate.</param>
    public LexicalCoordinate(int index, int line, int column)
    {
        Index = index;
        Line = line;
        Column = column;
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="LexicalCoordinate"/> object.
    /// </summary>
    /// <returns>A string that represents the current <see cref="LexicalCoordinate"/> object.</returns>
    public override string ToString()
    {
        return $"Index: {Index}, Line: {Line}, Char: {Column}";
    }
}
