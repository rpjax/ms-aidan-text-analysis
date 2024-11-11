using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;

namespace Aidan.TextAnalysis.Tokenization.StateMachine;

/// <summary>
/// A state machine for tokenizing strings.
/// </summary>
public class TokenizerMachine : IStringLexer
{
    /// <summary>
    /// The end of input character.
    /// </summary>
    public const char EoiChar = '\0';

    /// <summary>
    /// The default name for the end of input token.
    /// </summary>
    public const string EoiDefaultName = "\0";

    /* Cached values */
    private static ReadOnlyMemory<char> EoiReadOnlyMemory { get; } = $"{EoiChar}".AsMemory();

    /* Dependencies */
    private ITokenizerTable Table { get; }

    /* Settings */
    private bool UseDebugger { get; set; }
    private bool IncludeEoi { get; set; }
    private string EoiName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenizerMachine"/> class.
    /// </summary>
    /// <param name="table">The tokenizer table.</param>
    /// <param name="useDebugger">Whether to include the debugger.</param>
    /// <param name="includeEoi">Whether to include the end of input token.</param>
    /// <param name="eoiName">The name of the end of input token.</param>
    public TokenizerMachine(
        ITokenizerTable table,
        bool useDebugger = false,
        bool includeEoi = false,
        string eoiName = EoiDefaultName)
    {
        Table = table;
        UseDebugger = useDebugger;
        IncludeEoi = includeEoi;
        EoiName = eoiName;

        if (!includeEoi && eoiName != EoiDefaultName)
        {
            includeEoi = true;
        }
    }

    /// <summary>
    /// Tokenizes the specified input string.
    /// </summary>
    /// <param name="input">The input string to tokenize.</param>
    /// <returns>An enumerable of tokens.</returns>
    public IEnumerable<IToken> Tokenize(string input)
    {
        if (UseDebugger)
        {
            return TokenizeWithDebugger(input);
        }
        else
        {
            return TokenizeWithoutDebugger(input);
        }
        var process = new TokenizationProcess(
            table: Table,
            input: input,
            useDebugger: UseDebugger);

        return process.Tokenize(input);
    }

    /// <summary>
    /// Tokenizes the specified input string and returns an array of tokens.
    /// </summary>
    /// <param name="input">The input string to tokenize.</param>
    /// <returns>An array of tokens.</returns>
    public IToken[] TokenizeToArray(string input)
    {
        return Tokenize(input).ToArray();
    }

    private IEnumerable<IToken> TokenizeWithoutDebugger(string input)
    {
        /* machine variables */
        State initialState = Table.GetInitialState();
        State currentState = initialState;
        int position = 0;
        int line = 1;
        int column = 1;
        int tokenStart = 0;

        /* machine body */
        while (true)
        {
            /* current state declarations */
            char character = ReadCharacter(input, position);
            bool isCharEoi = character == EoiChar;
            bool isCurrentStateInitialState = currentState.Id == initialState.Id;

            /* handle end of input (loop break) */
            if (isCharEoi && isCurrentStateInitialState)
            {
                if (IncludeEoi)
                {
                    yield return CreateEoi(
                        tokenStart: tokenStart,
                        position: position,
                        line: line,
                        column: column);
                }

                break;
            }

            /* lookup next state */
            State? nextState = Table.LookUp(currentState.Id, character);

            if (nextState is null)
            {
                throw CreateUnknownCharacterException(
                    state: currentState,
                    character: character,
                    position: position,
                    line: line,
                    column: column);
            }

            /* next state declarations */
            bool isNextStateAccepting = nextState.IsAccepting;
            bool isNextStateInitialState = nextState.Id == initialState.Id;

            /* state transition */

            /* handle accepting state */
            if (isNextStateAccepting)
            {
                yield return CreateToken(
                    type: nextState.Name,
                    value: input.AsMemory(tokenStart, position - tokenStart),
                    tokenStart: tokenStart,
                    position: position,
                    line: line,
                    column: column);

                /* reset machine state */
                currentState = initialState;
                tokenStart = position;
            }

            /* handle non-accepting state */
            else
            {
                /* transition to next state */
                currentState = nextState;

                /* advances the input stream */
                Advance(
                    input: input,
                    character: character,
                    position: ref position,
                    line: ref line,
                    column: ref column);

                /* handles char skipping */
                if (isNextStateInitialState)
                {
                    tokenStart++;
                }
            }
        }

        yield break;
    }

    private IEnumerable<IToken> TokenizeWithDebugger(string input)
    {
        /* machine variables */
        State initialState = Table.GetInitialState();
        State currentState = initialState;
        int position = 0;
        int line = 1;
        int column = 1;
        int tokenStart = 0;
        MachineHistory history = new MachineHistory();

        /* machine body */
        while (true)
        {
            /* current state declarations */
            char character = ReadCharacter(input, position);
            bool isCharEoi = character == EoiChar;
            bool isCurrentStateInitialState = currentState.Id == initialState.Id;

            /* handle end of input (loop break) */
            if (isCharEoi && isCurrentStateInitialState)
            {
                if (IncludeEoi)
                {
                    yield return CreateEoi(
                        tokenStart: tokenStart,
                        position: position,
                        line: line,
                        column: column);
                }

                break;
            }

            /* lookup next state */
            State? nextState = Table.LookUp(currentState.Id, character);

            if (nextState is null && currentState.IsRecursiveOnNoTransition)
            {
                /* advances the input stream */
                Advance(
                    input: input,
                    character: character,
                    position: ref position,
                    line: ref line,
                    column: ref column);
                continue;
            }

            if (nextState is null)
            {
                throw CreateUnknownCharacterException(
                    state: currentState,
                    character: character,
                    position: position,
                    line: line,
                    column: column,
                    history: history);
            }

            history.AddEntry(
                sourceState: currentState,
                targetState: nextState,
                character: character,
                position: position,
                line: line,
                column: column);

            /* next state declarations */
            bool isNextStateAccepting = nextState.IsAccepting;
            bool isNextStateInitialState = nextState.Id == initialState.Id;

            /* state transition */

            /* handle accepting state */
            if (isNextStateAccepting)
            {
                yield return CreateToken(
                    type: nextState.Name,
                    value: input.AsMemory(tokenStart, position - tokenStart),
                    tokenStart: tokenStart,
                    position: position,
                    line: line,
                    column: column);

                /* reset machine state */
                currentState = initialState;
                tokenStart = position;
                history.Clear();
            }

            /* handle non-accepting state */
            else
            {
                /* transition to next state */
                currentState = nextState;

                /* advances the input stream */
                Advance(
                    input: input,
                    character: character,
                    position: ref position,
                    line: ref line,
                    column: ref column);

                /* handles char skipping */
                if (isNextStateInitialState)
                {
                    tokenStart++;
                }
            }
        }

        yield break;
    }

    /// <summary>
    /// Reads a character from the input string at the specified position.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="position">The position to read the character from.</param>
    /// <returns>The character at the specified position, or the end of input character if the position is out of bounds.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private char ReadCharacter(string input, int position)
    {
        return position >= input.Length
            ? EoiChar
            : input[position];
    }

    /// <summary>
    /// Advances the position, line, and column counters based on the current character.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="character">The current character.</param>
    /// <param name="position">The current position.</param>
    /// <param name="line">The current line.</param>
    /// <param name="column">The current column.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Advance(
        string input,
        char character,
        ref int position,
        ref int line,
        ref int column)
    {
        if (position >= input.Length)
        {
            return;
        }

        position++;

        if (IsLineTerminator(character))
        {
            line++;
            column = 1;
        }
        else
        {
            column++;
        }
    }

    /// <summary>
    /// Determines whether the specified character is a line terminator.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns><c>true</c> if the character is a line terminator; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsLineTerminator(char c)
    {
        return false
            || c == '\n'
            || c == '\r'
            || c == '\u2028'
            || c == '\u2029'
            ;
    }

    /// <summary>
    /// Creates an end of input token.
    /// </summary>
    /// <param name="tokenStart">The start position of the token.</param>
    /// <param name="position">The current position.</param>
    /// <param name="line">The current line.</param>
    /// <param name="column">The current column.</param>
    /// <returns>The end of input token.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IToken CreateEoi(
        int tokenStart,
        int position,
        int line,
        int column)
    {
        return new Token(
            type: EoiName,
            value: EoiReadOnlyMemory,
            metadata: CreateMetadata(
                length: EoiReadOnlyMemory.Length,
                tokenStart: tokenStart,
                position: position,
                line: line,
                column: column)
        );
    }

    /// <summary>
    /// Creates a token.
    /// </summary>
    /// <param name="type">The type of the token.</param>
    /// <param name="value">The value of the token.</param>
    /// <param name="tokenStart">The start position of the token.</param>
    /// <param name="position">The current position.</param>
    /// <param name="line">The current line.</param>
    /// <param name="column">The current column.</param>
    /// <returns>The created token.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IToken CreateToken(
        string type,
        ReadOnlyMemory<char> value,
        int tokenStart,
        int position,
        int line,
        int column)
    {
        return new Token(
            type: type,
            value: value,
            metadata: CreateMetadata(
                length: value.Length,
                tokenStart: tokenStart,
                position: position - 1,
                line: line,
                column: column)
        );
    }

    /// <summary>
    /// Creates metadata for a token.
    /// </summary>
    /// <param name="length">The length of the token.</param>
    /// <param name="tokenStart">The start position of the token.</param>
    /// <param name="position">The current position.</param>
    /// <param name="line">The current line.</param>
    /// <param name="column">The current column.</param>
    /// <returns>The created token metadata.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private TokenMetadata CreateMetadata(
        int length,
        int tokenStart,
        int position,
        int line,
        int column)
    {
        return new TokenMetadata(
            position: new TokenPosition(
                start: tokenStart,
                end: position,
                line: line,
                column: column - length
            )
        );
    }

    /* exceptions */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Exception CreateUnknownCharacterException(
        State state,
        char character,
        int position,
        int line,
        int column,
        MachineHistory? history = null)
    {
        if (history is not null)
        {
            return new Exception($"No state found for character '{character}' in state {state.Name}. At position {position}. Line {line}, column {column}.\n\n{history}");
        }

        return new Exception($"No state found for character '{character}' in state {state.Name}. At position {position}. Line {line}, column {column}.");
    }

}

internal class MachineHistory
{
    class Entry
    {
        public State SourceState { get; }
        public State TargetState { get; }
        public char Character { get; }
        public int Position { get; }
        public int Line { get; }
        public int Column { get; }

        public Entry(
            State sourceState,
            State targetState,
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
        State sourceState,
        State targetState,
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
            sb.AppendLine($"FROM {entry.SourceState.Name} TO {entry.TargetState.Name} ON '{entry.Character}' at position {entry.Position}. Line {entry.Line}, column {entry.Column}");
        }

        return sb.ToString();
    }
}

internal class TokenizationProcess
{
    /* Constants */
    private const char EoiChar = TokenizerMachine.EoiChar;

    /* Dependencies */
    private ITokenizerTable Table { get; }
    private string Input { get; }

    /* Settings */
    private bool UseDebugger { get; }

    /* Internal State */

    private State InitialState { get; }
    private State CurrentState { get; set; }
    private char Character { get; set; }
    private int Position { get; set; }
    private int Line { get; set; }
    private int Column { get; set; }
    private int TokenStart { get; set; }

    private MachineHistory History { get; }

    public TokenizationProcess(
        ITokenizerTable table,
        string input,
        bool useDebugger)
    {
        Table = table;
        Input = input;
        UseDebugger = useDebugger;

        InitialState = Table.GetInitialState();
        CurrentState = InitialState;
        Character = input.Length > 0 
            ? input[0] 
            : EoiChar;
        Position = 0;
        Line = 1;
        Column = 1;
        TokenStart = 0;
        History = new MachineHistory();
    }

    /// <summary>
    /// Tokenizes the specified input string.
    /// </summary>
    /// <param name="input">The input string to tokenize.</param>
    /// <returns>An enumerable of tokens.</returns>
    public IEnumerable<IToken> Tokenize(string input)
    {
        if (UseDebugger)
        {
            return TokenizeWithDebugger(input);
        }
        else
        {
            return TokenizeWithoutDebugger(input);
        }
    }

    private IEnumerable<IToken> TokenizeWithDebugger(string input)
    {        
        /* machine body */
        while (true)
        {
            /* current state declarations */
            bool isCharEoi = Character == EoiChar;
            bool isCurrentStateInitialState = CurrentState.Id == InitialState.Id;

            /* handle end of input (loop break) */
            if (isCharEoi && isCurrentStateInitialState)
            {
                yield break;
            }

            /* lookup next state */
            State? nextState = Table.LookUp(CurrentState.Id, Character);

            if (nextState is null && isCharEoi)
            {
                throw CreateUnexpectedEndOfInputException();
            }

            if (nextState is null && CurrentState.IsRecursiveOnNoTransition)
            {
                History.AddEntry(
                    sourceState: CurrentState,
                    targetState: CurrentState,
                    character: Character,
                    position: Position,
                    line: Line,
                    column: Column);
                Advance();
                continue;
            }

            if (nextState is null)
            {
                throw CreateUnknownCharacterException(History);
            }

            History.AddEntry(
                sourceState: CurrentState,
                targetState: nextState,
                character: Character,
                position: Position,
                line: Line,
                column: Column);

            /* next state declarations */
            bool isNextStateAccepting = nextState.IsAccepting;
            bool isNextStateInitialState = nextState.Id == InitialState.Id;

            /* state transition */

            /* handle accepting state */
            if (isNextStateAccepting)
            {
                yield return CreateToken(
                    type: nextState.Name,
                    value: input.AsMemory(TokenStart, Position - TokenStart));

                /* reset machine state */
                CurrentState = InitialState;
                TokenStart = Position;
                History.Clear();
                continue;
            }

            /* handle non-accepting state */
            else
            {
                /* transition to next state */
                CurrentState = nextState;

                /* advances the input stream */
                Advance();

                /* handles char skipping */
                if (isNextStateInitialState)
                {
                    TokenStart++;
                }
                continue;
            }
        }
    }

    private IEnumerable<IToken> TokenizeWithoutDebugger(string input)
    {
        /* machine body */
        while (true)
        {
            bool isCharEoi = Character == EoiChar;
            bool isCurrentStateInitialState = CurrentState.Id == InitialState.Id;

            /* handle end of input (loop break) */
            if (isCharEoi && isCurrentStateInitialState)
            {
                yield break;
            }

            /* lookup next state */
            State? nextState = Table.LookUp(CurrentState.Id, Character);

            if(nextState is null && isCharEoi)
            {
                throw CreateUnexpectedEndOfInputException();
            }

            if (nextState is null && CurrentState.IsRecursiveOnNoTransition)
            {
                Advance();
                continue;
            }

            if (nextState is null)
            {
                throw CreateUnknownCharacterException();
            }

            /* next state declarations */
            bool isNextStateAccepting = nextState.IsAccepting;
            bool isNextStateInitialState = nextState.Id == InitialState.Id;

            /* state transition */

            /* handle accepting state */
            if (isNextStateAccepting)
            {
                yield return CreateToken(
                    type: nextState.Name,
                    value: input.AsMemory(TokenStart, Position - TokenStart));

                /* reset machine state */
                CurrentState = InitialState;
                TokenStart = Position;
                continue;
            }

            /* handle non-accepting state */
            else
            {
                /* transition to next state */
                CurrentState = nextState;

                /* advances the input stream */
                Advance();

                /* handles char skipping */
                if (isNextStateInitialState)
                {
                    TokenStart++;
                }
                continue;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Advance()
    {
        Position++;

        if (Position >= Input.Length)
        {
            Character = EoiChar;
            return;
        }

        if (IsLineTerminator(Character))
        {
            Line++;
            Column = 1;
        }
        else
        {
            Column++;
        }

        Character = Input[Position];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsLineTerminator(char c)
    {
        return false
            || c == '\n'
            || c == '\r'
            || c == '\u2028'
            || c == '\u2029'
            ;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IToken CreateToken(
        string type,
        ReadOnlyMemory<char> value)
    {
        return new Token(
            type: type,
            value: value,
            metadata: CreateMetadata(
                length: value.Length,
                tokenStart: TokenStart,
                position: Position - 1,
                line: Line,
                column: Column)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private TokenMetadata CreateMetadata(
        int length,
        int tokenStart,
        int position,
        int line,
        int column)
    {
        return new TokenMetadata(
            position: new TokenPosition(
                start: tokenStart,
                end: position,
                line: line,
                column: column - length
            )
        );
    }

    /* exceptions */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Exception CreateUnexpectedEndOfInputException()
    {
        return new Exception($"Unexpected end of input. At position {Position}. Line {Line}, column {Column}.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Exception CreateUnknownCharacterException(MachineHistory? history = null)
    {
        if (history is not null)
        {
            return new Exception($"No state found for character '{Character}' in state {CurrentState.Name}. At position {Position}. Line {Line}, column {Column}.\n\n{history}");
        }

        return new Exception($"No state found for character '{Character}' in state {CurrentState.Name}. At position {Position}. Line {Line}, column {Column}.");
    }

}