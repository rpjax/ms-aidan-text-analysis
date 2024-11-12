using Aidan.TextAnalysis.Tokenization.Machine;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.Tokenization.StateMachine;

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

    private TokenizerState InitialState { get; }
    private TokenizerState CurrentState { get; set; }
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
            TokenizerState? nextState = Table.LookUp(CurrentState.Id, Character);

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
                AdvanceInput();
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
                AdvanceInput();

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
            TokenizerState? nextState = Table.LookUp(CurrentState.Id, Character);

            if (nextState is not null)
            {
                GoToState(nextState);
            }
            else
            {
                yield return CreateToken();
                Reset();
            }           
        }
    }

    /* actions */
 
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GoToState(TokenizerState nextState)
    {
        bool isCurrentStateInitialState = CurrentState.Id == InitialState.Id;
        bool isNextStateInitialState = nextState.Id == InitialState.Id;

        /* handles char skipping */
        if (isCurrentStateInitialState && isNextStateInitialState)
        {
            TokenStart++;
        }

        CurrentState = nextState;
        AdvanceInput();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IToken CreateToken()
    {
        var isCharEoi = Character == EoiChar;
        var isCurrentStateAccepting = CurrentState.IsAccepting;

        if (!isCurrentStateAccepting)
        {
            if (isCharEoi)
            {
                throw CreateUnexpectedEndOfInputException();
            }

            throw CreateUnknownCharacterException();
        }

        var type = CurrentState.Name;
        var value = Input.AsMemory(TokenStart, Position - TokenStart);

        return new Token(
            type: type,
            value: value,
            metadata: CreateMetadata(length: value.Length)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Reset()
    {
        CurrentState = InitialState;
        TokenStart = Position;
    }

    /* utils */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AdvanceInput()
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
    private TokenMetadata CreateMetadata(int length)
    {
        return new TokenMetadata(
            position: new TokenPosition(
                start: TokenStart,
                end: Position,
                line: Line,
                column: Column - length
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