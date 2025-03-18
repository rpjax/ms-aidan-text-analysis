namespace Aidan.TextAnalysis.Tokenization.Components;

/// <summary>
/// Represents a token produced by the lexical analyzer, using a lexeme production rule.
/// </summary>
public interface IToken
{
    /// <summary>
    /// Gets the type of the token. (e.g. Identifier, Number, String, etc.)
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Gets the raw value of the token. (e.g. "if", "123", "Hello World", etc.)
    /// </summary>
    ReadOnlyMemory<char> Value { get; }

    /// <summary>
    /// Gets the information associated with the token. (e.g. line number, column number, etc.)
    /// </summary>
    TokenMetadata Metadata { get; }
}