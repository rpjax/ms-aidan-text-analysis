namespace Aidan.TextAnalysis.Tokenization.Components;

/// <summary>
/// Represents the position of a token in the source text. 
/// </summary>
public struct TokenPosition
{
    /// <summary>
    /// Gets the character index position in the source text where the token starts. (0-based)
    /// </summary>
    public int Start { get; }

    /// <summary>
    /// Gets the character index position in the source text where the token ends. (0-based)
    /// </summary>
    public int End { get; }

    /// <summary>
    /// Gets the line number in which the token is located in the source text. (0-based)
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// Gets the column number in which the token is located in the source text. (0-based)
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenPosition"/> struct.
    /// </summary>
    /// <param name="start">The character index position in the source text where the token starts.</param>
    /// <param name="end">The character index position in the source text where the token ends.</param>
    /// <param name="line">The line number in which the token is located in the source text.</param>
    /// <param name="column">The column number in which the token is located in the source text.</param>
    public TokenPosition(
        int start,
        int end,
        int line,
        int column)
    {
        Start = start;
        End = end;
        Line = line;
        Column = column;
    }

    /// <summary>
    /// Gets an empty <see cref="TokenPosition"/> instance with all properties set to -1.
    /// </summary>
    public static TokenPosition Empty => new TokenPosition(-1, -1, -1, -1);

}
