namespace Aidan.TextAnalysis.Parsing.Components;

/// <summary>
/// Represents the metadata associated with a node in the concrete syntax tree (CST).
/// </summary>
public class CstNodeMetadata
{
    /// <summary>
    /// Gets the position of the node in the source text.
    /// </summary>
    public SyntaxElementPosition Position { get; }

    ///// <summary>
    ///// Gets the collection of tokens that represents the node in the source text.
    ///// </summary>
    //public Token[] Tokens { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="CstNodeMetadata"/> class.
    /// </summary>
    /// <param name="position">The position of the node in the source text.</param>
    public CstNodeMetadata(SyntaxElementPosition position/*, Token[] tokens*/)
    {
        Position = position;
        //Tokens = tokens;
    }

    /// <summary>
    /// Returns a string representation of the metadata.
    /// </summary>
    /// <returns>A string representation of the metadata.</returns>
    public override string ToString()
    {
        return $"Position: {Position}";
    }
}
