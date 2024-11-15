using Aidan.Core.Patterns;
using Aidan.TextAnalysis.Tokenization.StateMachine.Components;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Aidan.TextAnalysis.Tokenization.StateMachine.Builders;

public enum CharsetType
{
    Ascii,
    Unicode,
    Utf8,
    Utf16,
}

public class TokenizerBuilder : IBuilder<Tokenizer>
{
    private Dictionary<uint, TokenizerState> States { get; }
    private Dictionary<uint, List<TokenizerTransition>> Transitions { get; }
    private char[] Charset { get; set; }
    private bool UseDebugger { get; set; }

    public TokenizerBuilder(
        IEnumerable<char>? charset = null,
        bool useDebugger = false)
    {
        States = new();
        Transitions = new();
        Charset = charset?.ToArray() ?? ComputeCharset(CharsetType.Ascii);
    }

    public TokenizerBuilder(
        ITokenizerTable table, 
        IEnumerable<char> charset)
    {
        var entries = table.GetEntries();
        var initialState = table.GetInitialState();
        var states = entries
            .Select(x => x.Key)
            .ToArray();

        States = new();
        Transitions = new();
        Charset = charset.ToArray();

        foreach (var state in states)
        {
            CreateState(
                id: state.Id,
                name: state.Name,
                isAccepting: state.IsAccepting);

            var stateTransitions = entries
                .Where(x => x.Key.Id == state.Id)
                .Select(x => x.Value)
                .First()
                .ToList();

            foreach (var transition in stateTransitions)
            {
                AddTransition(
                    sourceStateId: state.Id,
                    character: transition.Character,
                    nextStateId: transition.StateId);
            }
        }
    }

    /* static methods */

    public static char[] ComputeCharset(CharsetType charset)
    {
        return charset switch
        {
            CharsetType.Ascii => Enumerable.Range(0, 128)
                .Select(x => (char)x)
                .ToArray(),

            CharsetType.Unicode => Enumerable.Range(0, 0xFFFF + 1) // Generates BMP (Basic Multilingual Plane) characters only
                .Select(x => (char)x)
                .ToArray(),

            CharsetType.Utf8 => Enumerable.Range(0, 0xFFFF + 1) // BMP characters, as UTF-8 is variable-length but this is limited by char's 16-bit size
                .Select(x => (char)x)
                .ToArray(),

            CharsetType.Utf16 => Enumerable.Range(0, 0xFFFF + 1) // BMP characters in UTF-16 representation
                .Select(x => (char)x)
                .ToArray(),

            _ => throw new ArgumentOutOfRangeException(nameof(charset))
        };
    }

    /* instace methods */

    /* getter methods */

    public char[] GetCharset()
    {
        return Charset;
    }

    public TokenizerState? FindState(uint id)
    {
        if (!States.TryGetValue(id, out var state))
        {
            return null;
        }

        return state;
    }

    public TokenizerState GetState(uint id)
    {
        if (!States.TryGetValue(id, out var state))
        {
            throw new InvalidOperationException($"State with ID {id} does not exist.");
        }

        return state;
    }

    public TokenizerState GetInitialState()
    {
        if (States.Count == 0)
        {
            throw new InvalidOperationException("No initial state defined.");
        }

        return States.First().Value;
    }

    /* setter methods */

    public TokenizerBuilder SetCharset(params char[] chars)
    {
        if (chars.Length == 0)
        {
            throw new ArgumentException("Charset must contain at least one character.");
        }

        Charset = chars;
        return this;
    }

    public TokenizerBuilder SetCharset(IEnumerable<char> chars)
    {
        return SetCharset(chars.ToArray());
    }

    public TokenizerBuilder SetCharset(CharsetType charset)
    {
        Charset = ComputeCharset(charset);
        return this;
    }

    public TokenizerBuilder EnableDebugger()
    {
        UseDebugger = true;
        return this;
    }

    public TokenizerState CreateState(
        uint id,
        string name,
        bool isAccepting)
    {
        var state = new TokenizerState(
            id: id,
            name: name,
            isAccepting: isAccepting,
            isRecursiveOnNoTransition: false);

        if (States.ContainsKey(id))
        {
            throw new InvalidOperationException($"A state with ID {id} already exists.");
        }
        if (Transitions.ContainsKey(id))
        {
            throw new InvalidOperationException($"A state with ID {id} already has transitions.");
        }

        States.Add(id, state);
        Transitions.Add(id, new List<TokenizerTransition>());

        return state;
    }

    public TokenizerBuilder AddTransition(
        uint sourceStateId,
        char character,
        uint nextStateId)
    {
        EnsureStateIsListed(sourceStateId);

        var transition = new TokenizerTransition(
            character: character,
            stateId: nextStateId);

        GetStateTransitions(sourceStateId)
            .Add(transition);

        return this;
    }

    public TableTransitionBuilder FromInitialState()
    {
        return new TableTransitionBuilder(
            builder: this,
            currentState: GetInitialState());
    }

    public TableTransitionBuilder FromState(uint id)
    {
        EnsureStateIsListed(id);

        return new TableTransitionBuilder(
            builder: this,
            currentState: GetState(id));
    }

    /* builder methods */

    public TokenizerTable BuildTable()
    {
        var entries = new Dictionary<TokenizerState, TokenizerTransition[]>();

        foreach (var state in States.Values)
        {
            entries.Add(state, GetStateTransitions(state.Id).ToArray());
        }

        return new TokenizerTable(entries);
    }

    public Tokenizer Build()
    {
        return new Tokenizer(
            table: BuildTable(),
            useDebugger: UseDebugger);
    }

    /* private methods */

    private void EnsureStateIsListed(uint id)
    {
        if (!States.ContainsKey(id))
        {
            throw new InvalidOperationException($"State with ID {id} does not exist.");
        }
    }

    private List<TokenizerTransition> GetStateTransitions(uint id)
    {
        EnsureStateIsListed(id);
        return Transitions[id];
    }

}
