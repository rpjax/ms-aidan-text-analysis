namespace Aidan.TextAnalysis.RegularExpressions.Automata;

public class AutomatonTransition
{
    public char Character { get; }
    public AutomatonNode NextState { get; }

    public AutomatonTransition(char character, AutomatonNode nextState)
    {
        Character = character;
        NextState = nextState;
    }

    public override string ToString()
    {
        return $"ON '{Character}' GOTO {NextState}";
    }

}
