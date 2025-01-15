using Aidan.TextAnalysis.Language.Components.Symbols;

namespace Aidan.TextAnalysis.Tokenization.Components;

/// <summary>
/// Represents a new token produced by the lexical Analyzer, using a lexeme production rule.
/// </summary>
public class Token : IToken
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
    /// Creates a new instance of the <see cref="Token"/> class.
    /// </summary>
    /// <param name="type"> The type of the token. </param>
    /// <param name="value"> The value of the token. </param>
    /// <param name="metadata"> The information associated with the token. </param>
    /// <exception cref="ArgumentException"> Thrown when the value of the token is empty. </exception>
    public Token(
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
    /// Creates a new instance of the <see cref="Token"/> class.
    /// </summary>
    /// <param name="type"> The type of the token. </param>
    /// <param name="value"> The value of the token. </param>
    /// <param name="metadata"> The information associated with the token. </param>
    /// <exception cref="ArgumentException"> Thrown when the value of the token is empty. </exception>
    public Token(
        string type,
        string value,
        TokenMetadata metadata) : this(type, value.AsMemory(), metadata)
    {

    }

    /// <summary>
    /// Returns a string that represents the current token.
    /// </summary>
    /// <returns> A string that represents the current token. </returns>
    public override string ToString()
    {
        if (Type == Eoi.EoiString)
        {
            return "EOI";
        }

        return $"{Type} »{FormatTokenForDisplay(Value.ToString())}«";
    }

    private string FormatTokenForDisplay(string tokenValue)
    {
        return tokenValue
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t")
            .Replace("\b", "\\b")
            .Replace("\f", "\\f");
    }

}
