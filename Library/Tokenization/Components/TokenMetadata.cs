namespace Aidan.TextAnalysis.Tokenization;

/// <summary>
/// Represents the position of a token in the source text. 
/// </summary>
public struct TokenPosition
{
    /// <summary>
    /// Gets the character index position in the source text where the token starts. (0-based)
    /// </summary>
    public int StartIndex { get; }

    /// <summary>
    /// Gets the character index position in the source text where the token ends. (0-based)
    /// </summary>
    public int EndIndex { get; }

    /// <summary>
    /// Gets the line number in which the token is located in the source text. (0-based)
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// Gets the column number in which the token is located in the source text. (0-based)
    /// </summary>
    public int Column { get; }

    public TokenPosition(int startIndex, int endIndex, int line, int column)
    {
        StartIndex = startIndex;
        EndIndex = endIndex;
        Line = line;
        Column = column;
    }
}

/// <summary>
/// Represents the set of information associated with a token.
/// </summary>
public struct TokenMetadata
{
    /// <summary>
    /// Gets the position of the token in the source text.
    /// </summary>
    public TokenPosition Position { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenMetadata"/> struct.
    /// </summary>
    /// <param name="position"></param>
    public TokenMetadata(TokenPosition position)
    {
        Position = position;
    }

    /// <summary>
    /// Returns a string representation of the metadata.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"line: {Position.Line}, column: {Position.Column} ({Position.StartIndex} - {Position.EndIndex})";
    }
}
