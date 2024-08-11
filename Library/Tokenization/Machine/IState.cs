namespace Aidan.TextAnalysis.Tokenization.Machine;

public interface IState
{
    ITransitionResult GetStateTransition(char? c);
}
