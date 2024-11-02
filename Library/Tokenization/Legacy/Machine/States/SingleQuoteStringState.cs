using Aidan.TextAnalysis.Tokenization.Components;
using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.Tokenization.Machine;

public class SingleQuoteStringState : IState
{
    public SingleQuoteStringState() 
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ITransitionResult GetStateTransition(char? c)
    {
        if (c is null)
        {
            return new TransitionResult(TokenizerState.None, TokenizerAction.Error);
        }

        if(c == TokenizerAlphabet.Escape)
        {
            //return new TransitionResult(TokenizerState.SingleQuoteStringEscape, TokenizerAction.Skip);
            return new TransitionResult(TokenizerState.SingleQuoteStringEscape, TokenizerAction.Read);
        }

        if (c == TokenizerAlphabet.SingleQuote)
        {
            return new TransitionResult(TokenizerState.StringEnd, TokenizerAction.Read);
        }

        return new TransitionResult(TokenizerState.None, TokenizerAction.Read);
    }
}

public class SingleQuoteStringEscapeState : IState
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ITransitionResult GetStateTransition(char? c)
    {
        if (c is null)
        {
            return new TransitionResult(TokenizerState.None, TokenizerAction.Error);
        }

        return new TransitionResult(TokenizerState.SingleQuoteString, TokenizerAction.Read);
    }
}
