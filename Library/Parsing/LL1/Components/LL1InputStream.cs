using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Tokenization;

namespace Aidan.TextAnalysis.Parsing.LL1.Components;

public class LL1InputStream : IDisposable
{
    private IEnumerator<Token> TokenStream { get; }
    private bool IsEndReached { get; set; }

    public LL1InputStream(string input, Tokenizer tokenizer)
    {
        TokenStream = tokenizer.Tokenize(input).GetEnumerator();
        IsEndReached = !TokenStream.MoveNext();
    }

    public Terminal? Lookahead => Peek();

    public void Dispose()
    {
        TokenStream.Dispose();
    }

    public Terminal? Peek()
    {
        if (IsEndReached)
        {
            return null;
        }

        return new Terminal(TokenStream.Current.Type, TokenStream.Current.Value.ToString());
    }

    public void Consume()
    {
        if (IsEndReached)
        {
            throw new InvalidOperationException("The end of the input stream has been reached.");
        }

        IsEndReached = !TokenStream.MoveNext();
    }
}
