namespace Aidan.TextAnalysis.Tokenization;

/// <summary>
/// Provides metadata information for a token.
/// </summary>
public interface ITokenMetadata
{
    /// <summary>
    /// Gets the position of the token.
    /// </summary>
    TokenPosition Position { get; }
}
