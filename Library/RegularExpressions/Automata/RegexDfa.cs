namespace Aidan.TextAnalysis.RegularExpressions.Automata;

public class RegexDfa
{
    public IReadOnlyList<char> Alphabet { get; }
    public IReadOnlyList<AutomatonNode> States { get; }
    public AutomatonNode InitialState { get; }

    public RegexDfa(
        IEnumerable<char> alphabet,
        IEnumerable<AutomatonNode> states)
    {
        Alphabet = alphabet.ToArray();
        States = states.ToArray();
        InitialState = States.First();
    }
}
