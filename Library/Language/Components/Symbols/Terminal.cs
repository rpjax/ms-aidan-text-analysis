using Aidan.TextAnalysis.Language.Extensions;
using Aidan.TextAnalysis.Tokenization;

namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents a terminal symbol in a context-free grammar.
/// </summary>
public interface ITerminal : ISymbol, IComparable<ITerminal>
{
    /// <summary>
    /// Gets the token type associated with the terminal symbol.
    /// </summary>
    TokenType Type { get; }

    /// <summary>
    /// Gets the value associated with the terminal symbol.
    /// </summary>
    string? Value { get; }
}

/// <summary>
/// Represents a terminal symbol in a context-free grammar.
/// </summary>
public class Terminal : Symbol, ITerminal, IComparable<Terminal>
{
    /// <inheritdoc/>
    public override bool IsTerminal => true;

    /// <inheritdoc/>
    public override bool IsNonTerminal => false;

    /// <inheritdoc/>
    public override bool IsEpsilon => false;

    /// <inheritdoc/>
    public override bool IsMacro => false;

    /// <inheritdoc/>
    public override bool IsEoi => false;

    /// <inheritdoc/>
    public TokenType Type { get; }

    /// <inheritdoc/>
    public string? Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Terminal"/> class with the specified token type and value.
    /// </summary>
    /// <param name="tokenType">The token type associated with the terminal symbol.</param>
    /// <param name="value">The value associated with the terminal symbol.</param>
    public Terminal(TokenType tokenType, string? value = null)
    {
        Type = tokenType;
        Value = value;

        if (value is not null && string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException("The value cannot be empty.");
        }
    }

    public Terminal(string value)
    {
        var tokens = Tokenizer.Instance.Tokenize(value, includeEoi: false).ToArray();

        if (tokens.Length != 1)
        {
            throw new InvalidOperationException("The value must be a single token.");
        }
        if (tokens[0] is null)
        {
            throw new InvalidOperationException("The value must be a valid token.");
        }

        Type = tokens[0]!.Type;
        Value = value;
    }

    public static bool operator ==(Terminal left, Terminal right)
    {
        return left.Type == right.Type
            && left.Value == right.Value;
    }

    public static bool operator !=(Terminal left, Terminal right)
    {
        return !(left == right);
    }

    public static Terminal From(string value)
    {
        return new Terminal(TokenType.Unknown, value);
    }

    /*
     * instance methods.
     */

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ Type.GetHashCode();
            hash = (hash * 16777619) ^ (Value?.GetHashCode() ?? 0);
            return hash;
        }
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Symbol);
    }

    public override bool Equals(Symbol? other)
    {
        return other is Terminal terminal
            && terminal.Type == Type
            && terminal.Value == Value;
    }

    public int CompareTo(Terminal? other)
    {
        var thisStr = ToNotation(NotationType.Sentential);
        var otherStr = other?.ToNotation(NotationType.Sentential);

        return string.Compare(thisStr, otherStr, StringComparison.Ordinal);
    }

    /*
     * ISymbol interface transition additions.
     */

    public override bool Equals(ISymbol? other)
    {
        return other is ITerminal terminal
            && terminal.Type == Type
            && terminal.Value == Value;
    }

    public int CompareTo(ITerminal? other)
    {
        var thisStr = ToNotation(NotationType.Sentential);
        var otherStr = other?.ToNotation(NotationType.Sentential);

        return string.Compare(thisStr, otherStr, StringComparison.Ordinal);
    }

    /*
     * Stringification methods.
     */

    public override string ToNotation(NotationType notation)
    {
        switch (notation)
        {
            case NotationType.Sentential:
                return this.ToSententialNotation();

            case NotationType.Bnf:
                return this.ToBnfNotation();

            case NotationType.Ebnf:
                return this.ToEbnfNotation();

            case NotationType.EbnfKleene:
                return this.ToEbnfKleeneNotation();
        }

        throw new InvalidOperationException("Invalid notation type.");
    }

    /// <summary>
    /// Returns a string representation of the terminal symbol.
    /// </summary>
    /// <returns>A string representation of the terminal symbol.</returns>
    public override string ToString()
    {
        return this.ToSententialNotation();
    }

}
