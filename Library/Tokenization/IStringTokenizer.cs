namespace Aidan.TextAnalysis.Tokenization;

/// <summary>
/// Represents a string tokenizer.
/// </summary>
public interface IStringTokenizer
{
    /// <summary>
    /// Tokenizes the input string.
    /// </summary>
    /// <param name="input">The input string to tokenize.</param>
    /// <param name="includeEoi">Specifies whether to include the end-of-input token.</param>
    /// <returns>An enumerable collection of tokens.</returns>
    IEnumerable<IToken> Tokenize(string input, bool includeEoi = true);
}
