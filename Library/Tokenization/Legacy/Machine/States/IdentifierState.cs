using Aidan.TextAnalysis.Tokenization.Components;
using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.Tokenization.Machine;

public class IdentifierState : IState
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ITransitionResult GetStateTransition(char? c)
    {
        if (c is null)
        {
            return new TokenResult(TokenizerState.Initial, TokenType.Identifier);
        }

        var isValidChar = false 
            || TokenizerAlphabet.IsDigitOrLetter(c.Value) 
            || TokenizerAlphabet.IsUnderline(c.Value)
            || c == '′'
            ;

        if (isValidChar)
        {
            return new TransitionResult(TokenizerState.None, TokenizerAction.Read);
        }

        return new TokenResult(TokenizerState.Initial, TokenType.Identifier);
    }
}
