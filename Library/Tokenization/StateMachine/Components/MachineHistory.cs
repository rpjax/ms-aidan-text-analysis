using System.Text;

namespace Aidan.TextAnalysis.Tokenization.StateMachine.Components;

internal class MachineHistory
{
    class Entry
    {
        public TokenizerState SourceState { get; }
        public TokenizerState TargetState { get; }
        public char Character { get; }
        public int Position { get; }
        public int Line { get; }
        public int Column { get; }

        public Entry(
            TokenizerState sourceState,
            TokenizerState targetState,
            char character,
            int position,
            int line,
            int column)
        {
            SourceState = sourceState;
            TargetState = targetState;
            Character = character;
            Position = position;
            Line = line;
            Column = column;
        }
    }

    private List<Entry> Entries { get; }

    public MachineHistory()
    {
        Entries = new List<Entry>();
    }

    public void AddEntry(
        TokenizerState sourceState,
        TokenizerState targetState,
        char character,
        int position,
        int line,
        int column)
    {
        Entries.Add(new Entry(
            sourceState: sourceState,
            targetState: targetState,
            character: character,
            position: position,
            line: line,
            column: column));
    }

    public void Clear()
    {
        Entries.Clear();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var entry in Entries)
        {
            sb.AppendLine($"FROM {entry.SourceState.Name} ON '{entry.Character}' GOTO {entry.TargetState.Name}");
        }

        return sb.ToString();
    }
}
