﻿namespace Aidan.TextAnalysis.Tokenization;

/// <summary>
/// Represents a token produced by the lexical Analyzer, using a lexeme production rule.
/// </summary>
public class Token : ITokenLegacy
{
    /// <summary>
    /// Gets the type of the token. (e.g. Identifier, Number, String, etc.)
    /// </summary>
    public TokenType Type { get; }

    /// <summary>
    /// Gets the raw value of the token. (e.g. "if", "123", "Hello World", etc.)
    /// </summary>
    public ReadOnlyMemory<char> Value { get; }

    /// <summary>
    /// Gets the information associated with the token. (e.g. line number, column number, etc.)
    /// </summary>
    public TokenMetadata Metadata { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="Token"/> class.
    /// </summary>
    /// <param name="type"> The type of the token. </param>
    /// <param name="value"> The value of the token. </param>
    /// <param name="metadata"> The information associated with the token. </param>
    public Token(TokenType type, ReadOnlyMemory<char> value, TokenMetadata metadata)
    {
        Type = type;
        Value = value;
        Metadata = metadata;

        if (value.Length == 0)
        {
            throw new ArgumentException("The value of the token cannot be empty.", nameof(value));
        }
    }

    public override string ToString()
    {
        if(Type == TokenType.Eoi)
        {
            return "EOI";
        }

        return Value.ToString();
    }

    public string ToStringVerbose()
    {
        if (Type == TokenType.String)
        {
            return $"{Type}: {Value} {Metadata}";
        }
        else
        {
            return $"{Type}: \"{Value}\"  {Metadata}";
        }
    }

}

/// <summary>
/// Represents a new token produced by the lexical Analyzer, using a lexeme production rule.
/// </summary>
public class TokenNewApi : IToken
{
    /// <summary>
    /// Gets the type of the token. (e.g. Identifier, Number, String, etc.)
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the raw value of the token. (e.g. "if", "123", "Hello World", etc.)
    /// </summary>
    public ReadOnlyMemory<char> Value { get; }

    /// <summary>
    /// Gets the information associated with the token. (e.g. line number, column number, etc.)
    /// </summary>
    public TokenMetadata Metadata { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="TokenNewApi"/> class.
    /// </summary>
    /// <param name="type"> The type of the token. </param>
    /// <param name="value"> The value of the token. </param>
    /// <param name="metadata"> The information associated with the token. </param>
    /// <exception cref="ArgumentException"> Thrown when the value of the token is empty. </exception>
    public TokenNewApi(
        string type,
        ReadOnlyMemory<char> value,
        TokenMetadata metadata)
    {
        Type = type;
        Value = value;
        Metadata = metadata;

        if (value.Length == 0)
        {
            throw new ArgumentException("The value of the token cannot be empty.", nameof(value));
        }
    }

    /// <summary>
    /// Returns a string that represents the current token.
    /// </summary>
    /// <returns> A string that represents the current token. </returns>
    public override string ToString()
    {
        if (Type.ToLower() == TokenType.Eoi.ToString().ToLower())
        {
            return "EOI";
        }

        return Value.ToString();
    }

    /// <summary>
    /// Returns a verbose string that represents the current token.
    /// </summary>
    /// <returns> A verbose string that represents the current token. </returns>
    public string ToStringVerbose()
    {
        if (Type.ToLower() == TokenType.String.ToString().ToLower())
        {
            return $"{Type}: {Value} {Metadata}";
        }
        else
        {
            return $"{Type}: \"{Value}\"  {Metadata}";
        }
    }
}

