namespace Aidan.TextAnalysis.Parsing.Components;

/// <summary>
/// Represents the position of a syntatical element in the source code.
/// </summary>
public struct SyntaxElementPosition
{
    /// <summary>
    /// Gets the starting coordinate of the syntax element.
    /// </summary>
    public LexicalCoordinate StartCoordinate { get; }

    /// <summary>
    /// Gets the ending coordinate of the syntax element.
    /// </summary>
    public LexicalCoordinate EndCoordinate { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="SyntaxElementPosition"/> class.
    /// </summary>
    /// <param name="start">The starting coordinate of the syntax element.</param>
    /// <param name="end">The ending coordinate of the syntax element.</param>
    public SyntaxElementPosition(LexicalCoordinate start, LexicalCoordinate end)
    {
        StartCoordinate = start;
        EndCoordinate = end;
    }

    /// <summary>
    /// Returns a string representation of the position.
    /// </summary>
    /// <returns>A string representation of the position.</returns>
    public override string ToString()
    {
        return $"Starts At: ({StartCoordinate}), Ends At: ({EndCoordinate})";
    }
}
