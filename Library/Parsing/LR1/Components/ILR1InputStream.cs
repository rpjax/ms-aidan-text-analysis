using Aidan.TextAnalysis.Language.Components.Symbols;
using Aidan.TextAnalysis.Tokenization.Components;

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
