namespace Aidan.TextAnalysis.Tokenization.StateMachine;

public class State
{
    public int Id { get; }
    public string Name { get; }
    public bool IsAccepting { get; }

    public State(
        int id,
        string name,
        bool isAccepting)
    {
        Id = id;
        Name = name;
        IsAccepting = isAccepting;
    }

    public override string ToString()
    {
        return $"{Name} ({Id}) {(IsAccepting ? "Accepting" : "")}";
    }
}

public class Transition
{
    public char Character { get; }
    public int StateId { get; }

    public Transition(char character, int stateId)
    {
        Character = character;
        StateId = stateId;
    }

    public override string ToString()
    {
        var c = Character == '\0' 
            ? "EOI" 
            : $"'{Character}'"
            ;

        return $"on {c} goto state {StateId}";
    }
}

public class TransitionTable
{
    private Dictionary<char, Transition> Entries { get; }

    public TransitionTable(Dictionary<char, Transition> entries)
    {
        Entries = entries;
    }

}
