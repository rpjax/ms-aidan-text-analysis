namespace Aidan.TextAnalysis.Tokenization.Machine;

public interface ITransitionResult
{
    TokenizerState NextState { get; }
    TokenizerAction Action { get; }
}
