﻿namespace Aidan.TextAnalysis.Tokenization;

/// <summary>
/// Represents a token produced by the lexical Analyzer, using a lexeme production rule.
/// </summary>
public interface ITokenLegacy
{
    /// <summary>
    /// Gets the type of the token. (e.g. Identifier, Number, String, etc.)
    /// </summary>
    TokenType Type { get; }

    /// <summary>
    /// Gets the raw value of the token. (e.g. "if", "123", "Hello World", etc.)
    /// </summary>
    ReadOnlyMemory<char> Value { get; }

    /// <summary>
    /// Gets the information associated with the token. (e.g. line number, column number, etc.)
    /// </summary>
    TokenMetadata Metadata { get; }
}
