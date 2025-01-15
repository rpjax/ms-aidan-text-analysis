using System.Text.RegularExpressions;
using Aidan.Core.Patterns;

namespace Aidan.TextAnalysis.Tokenization.Experimental.RegexTokenization;

public class ProductionMatch
{
    public TokenProduction Production { get; }
    public Match Match { get; }

    public ProductionMatch(
        TokenProduction production,
        Match match)
    {
        Production = production;
        Match = match;
    }
}



public enum TransitionAction
{
    Shift,
    Reduce,
    Error
}

public class State
{
    public int Id { get; }

    public State(int id)
    {
        Id = id;
    }
}

public class StateTransition
{
    public char Character { get; }
    public TransitionAction Action { get; }
    public State NextState { get; }
    public int NextStateId { get; }
    public bool IsAcceptingState { get; }

    public StateTransition(
        char character,
        TransitionAction action,
        State nextState)
    {
        Character = character;
        Action = action;
        NextState = nextState;
        NextStateId = nextState.Id;
    }
}

public class TransitionTableEntry
{
    public State State { get; }
    public StateTransition[] Transitions { get; }

    public TransitionTableEntry(
        State state,
        StateTransition[] transitions)
    {
        State = state;
        Transitions = transitions;
    }
}

public class TransitionTableEntryBuilder : IBuilder<TransitionTableEntry>
{
    private State? State { get; set; }
    private List<StateTransition> Transitions { get; } = new();

    public TransitionTableEntryBuilder WithState(State state)
    {
        State = state;
        return this;
    }

    public TransitionTableEntryBuilder WithTransition(
        char character,
        TransitionAction action,
        State nextState)
    {
        Transitions.Add(new StateTransition(character, action, nextState));
        return this;
    }

    public TransitionTableEntry Build()
    {
        if (State == null)
        {
            throw new InvalidOperationException("State must be set.");
        }

        return new TransitionTableEntry(State, Transitions.ToArray());
    }
}

public class TransitionTable
{
    public Dictionary<int, Dictionary<char, StateTransition>> Entries { get; }

    public TransitionTable(
        IEnumerable<TransitionTableEntry> entries)
    {
        Entries = entries.ToDictionary(
            keySelector: entry => entry.State.Id,
            elementSelector: entry => entry.Transitions
                .ToDictionary(TransitionTableEntry => TransitionTableEntry.Character, TransitionTableEntry => TransitionTableEntry));
    }

    public StateTransition? Lookup(int stateId, char character)
    {
        if (Entries.TryGetValue(stateId, out var transitions))
        {
            if (transitions.TryGetValue(character, out var transition))
            {
                return transition;
            }
        }

        return null;
    }
}

public interface ITokenizationContext
{
    char? GetCurrentChar();
    ReadOnlyMemory<char> GetTokenValue();
    TokenMetadata GetTokenMetadata();

    /* Machine state and controls */

    int GetCurrentState();
    bool IsInInitialState();

    void GotoState(int stateId);
    void AdvanceCharacter();
    void SkipCharacter();
    void BreakLine();
}

public class RegexTokenizer : IStringTokenizer
{
    private TransitionTable TransitionTable { get; }

    public RegexTokenizer(TransitionTable transitionTable)
    {
        TransitionTable = transitionTable;
    }

    public IEnumerable<IToken> Tokenize(string input)
    {
        // this is for later encapsulation
        //var context = new TokenizationContext(
        //    input: input, 
        //    index: 0, 
        //    line: 0, 
        //    column: 0);

        ITokenizationContext context = new TokenizationContext(input);

        while (true)
        {
            var currentChar = context.GetCurrentChar();
            var currentState = context.GetCurrentState();
            var isInInitialState = context.IsInInitialState();

            if (currentChar == null)
            {
                // TODO: handle end of input
                break;
            }

            if (isInInitialState && IsLineBreak(currentChar.Value))
            {
                context.BreakLine();
                continue;
            }

            if (isInInitialState && ShouldSkipCharacter(currentChar.Value))
            {
                context.SkipCharacter();
                continue;
            }

            var transition = TransitionTable.Lookup(
                stateId: currentState,
                character: currentChar.Value);

            if (transition == null)
            {
                // TODO: handle error
                break;
            }

            if (transition.IsAcceptingState)
            {
                var value = context.GetTokenValue();
                var metadata = context.GetTokenMetadata();

                yield return new Token(
                    type: "TODO",
                    value: value,
                    metadata: metadata);
            }

            context.AdvanceCharacter();
            context.GotoState(transition.NextStateId);
        }
    }

    /*
     * Those methods are placeholders for now.
     */

    private bool ShouldSkipCharacter(char character)
    {
        return char.IsWhiteSpace(character);
    }

    private bool IsLineBreak(char character)
    {
        return character == '\n';
    }

}
