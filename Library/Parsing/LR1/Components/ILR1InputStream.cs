using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Tokenization;
using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents an input stream for LR(1) parsing.
/// </summary>
public interface ILR1InputStream
{
    /// <summary>
    /// Gets the lookahead symbol from the input stream.
    /// </summary>
    /// <returns>The lookahead symbol.</returns>
    ITerminal GetLookaheadSymbol();

    /// <summary>
    /// Gets the lookahead token from the input stream.
    /// </summary>
    /// <returns>The lookahead token.</returns>
    IToken GetLookaheadToken();

    /// <summary>
    /// Advances the input stream to the next token.
    /// </summary>
    void Advance();
}

/// <summary>
/// Represents an implementation of <see cref="ILR1InputStream"/>.
/// </summary>
public class LR1InputStream : ILR1InputStream, IDisposable
{
    private IEnumerator<IToken> Enumerator { get; }
    private bool IsConsumed { get; set; }
    private Dictionary<string, bool> IgnoreDictionary { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LR1InputStream"/> class.
    /// </summary>
    /// <param name="tokens">The tokens to be processed by the input stream.</param>
    /// <param name="ignoreSet">The set of token types to ignore.</param>
    public LR1InputStream(IEnumerable<IToken> tokens, string[]? ignoreSet = null)
    {
        Enumerator = tokens.GetEnumerator();
        IgnoreDictionary = (ignoreSet ?? new string[0]).ToDictionary(x => x, x => true);
        Advance();
    }

    /// <summary>
    /// Disposes the input stream.
    /// </summary>
    public void Dispose()
    {
        Enumerator.Dispose();
    }

    /// <summary>
    /// Advances the input stream to the next token.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance()
    {
        IsConsumed = !Enumerator.MoveNext();

        while (!IsConsumed && IgnoreDictionary.ContainsKey(Enumerator.Current.Type))
        {
            IsConsumed = !Enumerator.MoveNext();
        }
    }

    /// <summary>
    /// Gets the lookahead symbol from the input stream.
    /// </summary>
    /// <returns>The lookahead symbol.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ITerminal GetLookaheadSymbol()
    {
        if (IsConsumed)
        {
            return Eoi.Instance;
        }

        return Convert(Enumerator.Current);
    }

    /// <summary>
    /// Gets the lookahead token from the input stream.
    /// </summary>
    /// <returns>The lookahead token.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the input stream is consumed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IToken GetLookaheadToken()
    {
        if (IsConsumed)
        {
            throw new InvalidOperationException();
        }

        return Enumerator.Current;
    }

    /// <summary>
    /// Converts a token to a terminal symbol.
    /// </summary>
    /// <param name="token">The token to convert.</param>
    /// <returns>The terminal symbol.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ITerminal Convert(IToken token)
    {
        return new Terminal(token.Type);
    }
}
