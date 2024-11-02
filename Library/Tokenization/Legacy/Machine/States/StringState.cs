using Aidan.TextAnalysis.Tokenization.Components;

namespace Aidan.TextAnalysis.Tokenization.Machine;

public class StringState : IState
{
    char DelimiterChar { get; set; }
    bool CanEmit { get; set; }
    bool IsEscapedState { get; set; }

    public StringState(char delimiter)
    {
        DelimiterChar = delimiter;
    }

    public void Reset()
    {
        CanEmit = false;
        IsEscapedState = false;
    }

    public ITransitionResult GetStateTransition(char? c)
    {
        if (CanEmit)
        {
            return new TokenResult(TokenizerState.Initial, TokenType.String);
        }

        if (c is null)
        {
            return new TransitionResult(TokenizerState.None, TokenizerAction.Error);
        }

        if(c == TokenizerAlphabet.Escape && !IsEscapedState)
        {
            IsEscapedState = true; 
            return new TransitionResult(TokenizerState.None, TokenizerAction.Skip);
        }

        if (IsEscapedState)
        {
            IsEscapedState = false;
            return new TransitionResult(TokenizerState.None, TokenizerAction.Read);
        }

        if (c == DelimiterChar)
        {
            CanEmit = true;
            return new TransitionResult(TokenizerState.None, TokenizerAction.Read);
        }
        else
        {
            return new TransitionResult(TokenizerState.None, TokenizerAction.Read);
        }
    }
}

public static class EscapeSequenceHelper
{
    private static Dictionary<string, char> escapeSequences = new Dictionary<string, char>
    {
        { @"\n", '\n' },
        { @"\t", '\t' },
        { @"\\", '\\' },
        { @"\""", '"' },
        { @"\'", '\'' }
        // Add more escape sequences as needed
    };

    public static bool TryGetEscapedChar(string sequence, out char result)
    {
        return escapeSequences.TryGetValue(sequence, out result);
    }
}