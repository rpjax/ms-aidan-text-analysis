namespace Aidan.TextAnalysis.Tokenization.Machine;

public class FloatNumberState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        if (c is null)
        {
            return new TokenResult(TokenizerState.Initial, TokenType.Float);
        }

        if (char.IsDigit(c.Value))
        {
            return new TransitionResult(TokenizerState.None, TokenizerAction.Read);
        }
       
        return new TokenResult(TokenizerState.Initial, TokenType.Float);
    }
}
