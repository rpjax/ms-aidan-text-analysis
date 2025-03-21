﻿namespace Aidan.TextAnalysis.Tokenization.Components;

/// <summary>
/// Represents the set of information associated with a token.
/// </summary>
public struct TokenMetadata : ITokenMetadata
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
        return $"line: {Position.Line}, column: {Position.Column} ({Position.Start} - {Position.End})";
    }

    /// <summary>
    /// Gets an empty <see cref="TokenMetadata"/> instance.
    /// </summary>
    public static TokenMetadata Empty => new TokenMetadata(
        position: TokenPosition.Empty);

}
