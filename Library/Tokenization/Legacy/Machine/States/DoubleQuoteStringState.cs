using Aidan.TextAnalysis.Tokenization.Components;
using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.Tokenization.Machine;

public class DoubleQuoteStringState : IState
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ITransitionResult GetStateTransition(char? c)
    {
        if (c is null)
        {
            return new TransitionResult(TokenizerState.None, TokenizerAction.Error);
        }

        if (c == TokenizerAlphabet.Escape)
        {
            //return new TransitionResult(TokenizerState.DoubleQuoteStringEscape, TokenizerAction.Skip);
            return new TransitionResult(TokenizerState.DoubleQuoteStringEscape, TokenizerAction.Read);
        }

        if (c == TokenizerAlphabet.DoubleQuote)
        {
            return new TransitionResult(TokenizerState.StringEnd, TokenizerAction.Read);
        }

        return new TransitionResult(TokenizerState.None, TokenizerAction.Read);
    }
}

public class DoubleQuoteStringEscapeState : IState
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ITransitionResult GetStateTransition(char? c)
    {
        if (c is null)
        {
            return new TransitionResult(TokenizerState.None, TokenizerAction.Error);
        }

        return new TransitionResult(TokenizerState.DoubleQuoteString, TokenizerAction.Read);
    }
}
