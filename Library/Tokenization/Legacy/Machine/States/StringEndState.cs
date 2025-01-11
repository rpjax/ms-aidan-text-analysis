using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.Tokenization.Machine;

public class StringEndState : IState
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ITransitionResult GetStateTransition(char? c)
    {
        return new TokenResult(TokenizerState.Initial, TokenType.String);
    }
}