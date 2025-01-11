namespace Aidan.TextAnalysis.Tokenization.Machine;

public class CppStyleCommentStartState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        switch (c)
        {
            //* formed sequence: '//'
            case '/':
                return new TransitionResult(TokenizerState.CppStyleSingleLineComment, TokenizerAction.Read);

            //* formed sequence: '/*'
            case '*':
                return new TransitionResult(TokenizerState.CppStyleMultiLineComment, TokenizerAction.Read);

            //* formed sequence: '/' (it doesn't read)
            default:
                return new TransitionResult(TokenizerState.Punctuation, TokenizerAction.None);
        }
    }
}

/*
* C++ line comment style 
*/

public class CppStyleSingleLineCommentState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        switch (c)
        {
            case null:
                return new TransitionResult(TokenizerState.CommentEnd, TokenizerAction.None);

            case '\n':
                return new TransitionResult(TokenizerState.CommentEnd, TokenizerAction.Read);

            default:
                return new TransitionResult(TokenizerState.None, TokenizerAction.Read);
        }
    }
}

/*
* C++ multiline comment style 
*/

public class CppStyleMultiLineCommentState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        switch (c)
        {
            case null:
                return new TransitionResult(TokenizerState.None, TokenizerAction.Error);

            case '*':
                return new TransitionResult(TokenizerState.CppStyleMultiLineCommentEndConfirm, TokenizerAction.Read);

            default:
                return new TransitionResult(TokenizerState.None, TokenizerAction.Read);
        }
    }
}

public class CppStyleMultiLineCommentEndConfirmState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        switch (c)
        {
            case null:
                return new TransitionResult(TokenizerState.None, TokenizerAction.Error);

            case '/':
                return new TransitionResult(TokenizerState.CommentEnd, TokenizerAction.Read);

            default:
                return new TransitionResult(TokenizerState.CppStyleMultiLineComment, TokenizerAction.Read);
        }
    }
}
