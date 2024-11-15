using Aidan.TextAnalysis.Tokenization;

namespace Aidan.TextAnalysis.Parsing.Components;

/// <summary>
/// Represents a stream of <see cref="OldToken"/> with one token lookahead. It is used by LL(1) and LR(1) parsers.
/// </summary>
public class InputStream : IDisposable
{
    private IEnumerator<OldToken> TokenStream { get; }
    private bool IsEndReached { get; set; }
    private TokenType[] IgnoreSet { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InputStream"/> class.
    /// </summary>
    /// <param name="input">The input string to tokenize.</param>
    /// <param name="tokenizer">The tokenizer to use for tokenizing the input string.</param>
    /// <param name="ignoreSet">The set of token types to ignore.</param>
    public InputStream(
        string input,
        LegacyTokenizer tokenizer,
        TokenType[]? ignoreSet = null)
    {
        TokenStream = tokenizer.Tokenize(input).GetEnumerator();
        IsEndReached = false;
        IgnoreSet = ignoreSet ?? Array.Empty<TokenType>();

        Init();
    }

    /// <summary>
    /// Gets the lookahead token.
    /// </summary>
    public OldToken? LookaheadToken => Peek();

    /// <summary>
    /// Gets a value indicating whether the end of the input stream has been reached.
    /// </summary>
    public bool IsEoi => IsEndReached;

    /// <summary>
    /// Disposes the input stream.
    /// </summary>
    public void Dispose()
    {
        TokenStream.Dispose();
    }

    /// <summary>
    /// Peeks the next token.
    /// </summary>
    /// <returns>The next token if the end of the input stream has not been reached; otherwise, <c>null</c>.</returns>
    public OldToken? Peek()
    {
        if (IsEndReached)
        {
            return null;
        }

        return TokenStream.Current;
    }

    /// <summary>
    /// Consumes the current token and moves to the next one.
    /// </summary>
    /// <remarks>
    /// It skips the tokens in the ignore set.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when the end of the input stream has been reached.</exception>
    public void Consume()
    {
        if (IsEndReached)
        {
            throw new InvalidOperationException("The end of the input stream has been reached.");
        }

        IsEndReached = !TokenStream.MoveNext();

        while (!IsEndReached && IgnoreSet.Contains(TokenStream.Current.Type))
        {
            IsEndReached = !TokenStream.MoveNext();
        }
    }

    /// <summary>
    /// Initializes the input stream by moving to the first non-ignored token.
    /// </summary>
    private void Init()
    {
        IsEndReached = !TokenStream.MoveNext();

        var ignoreToken = IgnoreSet
            .Any(x => x == TokenStream.Current.Type);

        if (!ignoreToken)
        {
            return;
        }

        while (!IsEndReached && IgnoreSet.Contains(TokenStream.Current.Type))
        {
            IsEndReached = !TokenStream.MoveNext();
        }
    }
}
