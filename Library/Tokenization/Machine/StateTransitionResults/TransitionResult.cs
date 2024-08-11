namespace Aidan.TextAnalysis.Tokenization.Machine;

public struct TransitionResult : ITransitionResult
{
    public TokenizerState NextState { get; }
    public TokenizerAction Action { get; }

    public TransitionResult(TokenizerState nextState, TokenizerAction action)
    {
        NextState = nextState;
        Action = action;
    }
}
