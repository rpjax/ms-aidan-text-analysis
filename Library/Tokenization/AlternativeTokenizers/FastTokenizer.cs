using Aidan.Core;
using Aidan.TextAnalysis.Tokenization.Components;

namespace Aidan.TextAnalysis.Tokenization.Experimental;

public interface ICharacterStream
{
    char? Peek(int offset);
    CharType? PeekType(int offset);
    bool Advance();
    ICharacterStream Fork();
}

public interface ITokenStream
{
    ref IToken? Peek(int offset);
    bool Advance();
    ITokenStream Fork();
}

public interface ITokenizer
{
    ITokenStream Tokenize(ICharacterStream source);
}

public class CharacterStream : ICharacterStream
{
    private IEnumerable<char> Source { get; }
    private List<char> LookaheadBuffer { get; }

    public CharacterStream(IEnumerable<char> source)
    {
        Source = source;
        LookaheadBuffer = new List<char>();
    }

    public char? Peek(int offset)
    {
        throw new NotImplementedException();
    }
        
    public bool Advance()
    {
        throw new NotImplementedException();
    }

    public ICharacterStream Fork()
    {
        throw new NotImplementedException();
    }

    public CharType? PeekType(int offset)
    {
        throw new NotImplementedException();
    }
}

public enum FastTokenizerAction
{
    ConsumeCharacter,
    SkipCharacter,
    ClearAccumulator,
    EmitToken,
    EmitError,
    ThrowException
}

public interface ITokenizerController
{
    void ConsumeCharacter();
    void SkipCharacter();
    void ClearAccumulator();
    void EmitToken(TokenType type);
    void EmitError(Error error);
    void ThrowException(Exception exception);
}

public interface ITokenizerState
{
    
}

public class FastTokenizer : ITokenizer
{
    public ITokenStream Tokenize(ICharacterStream source)
    {
        throw new NotImplementedException();
    }
}
