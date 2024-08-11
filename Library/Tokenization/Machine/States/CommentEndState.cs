namespace Aidan.TextAnalysis.Tokenization.Machine;

public class CommentEndState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        return new TokenResult(TokenizerState.Initial, TokenType.Comment);
    }
}
