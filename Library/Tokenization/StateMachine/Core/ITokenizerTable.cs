namespace Aidan.TextAnalysis.Tokenization.StateMachine;

public interface ITokenizerTable
{
    State GetInitialState();
    State? LookUp(int state, char character);
}
