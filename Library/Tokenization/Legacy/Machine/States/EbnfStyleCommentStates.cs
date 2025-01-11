namespace Aidan.TextAnalysis.Tokenization.Machine;

public class EbnfStyleMultiLineCommentStartState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        switch (c)
        {
            //* formed sequence: '(*'
            case '*':
                return new TransitionResult(TokenizerState.EbnfStyleMultiLineComment, TokenizerAction.Read);

            //* formed sequence: '/' (it doesn't read)
            default:
                return new TransitionResult(TokenizerState.Punctuation, TokenizerAction.None);
        }
    }
}

public class EbnfStyleMultiLineCommentState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        switch (c)
        {
            case null:
                return new TransitionResult(TokenizerState.None, TokenizerAction.Error);

            case '*':
                return new TransitionResult(TokenizerState.EbnfStyleMultiLineCommentEndConfirm, TokenizerAction.Read);

            default:
                return new TransitionResult(TokenizerState.None, TokenizerAction.Read);
        }
    }
}

public class EbnfStyleMultiLineCommentEndConfirmState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        switch (c)
        {
            case null:
                return new TransitionResult(TokenizerState.None, TokenizerAction.Error);

            case ')':
                return new TransitionResult(TokenizerState.CommentEnd, TokenizerAction.Read);

            default:
                return new TransitionResult(TokenizerState.EbnfStyleMultiLineComment, TokenizerAction.Read);
        }
    }
}
