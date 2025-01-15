using System.Runtime.CompilerServices;
using Aidan.TextAnalysis.Tokenization.Components;
using Aidan.TextAnalysis.Tokenization.StateMachine.Components;

namespace Aidan.TextAnalysis.Tokenization.StateMachine;

internal class TokenizationMachine
{
    /* Constants */
    private const char EoiChar = Tokenizer.EoiChar;

    /* Dependencies */
    private TokenizerTable Table { get; }
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

    public TokenizationMachine(
        TokenizerTable table,
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
    public IEnumerable<IToken> Tokenize()
    {
        if (UseDebugger)
        {
            return TokenizeWithDebugger();
        }
        else
        {
            return TokenizeWithoutDebugger();
        }
    }

    private IEnumerable<IToken> TokenizeWithDebugger()
    {
        /* machine body */
        while (true)
        {
            bool isCharEoi = Character == EoiChar;
            bool isCurrentStateInitialState = CurrentState.Id == InitialState.Id;
            bool isCurrentStateAccepting = CurrentState.IsAccepting;

            /* handle end of input (loop break) */
            if (isCharEoi && isCurrentStateInitialState)
            {
                yield break;
            }

            /* lookup next state */
            TokenizerState? nextState = Table.LookUp(CurrentState.Id, Character);

            if (nextState is null && CurrentState.IsRecursiveOnNoTransition)
            {
                GoToState(CurrentState);
                continue;
            }

            if (nextState is not null)
            {
                History.AddEntry(
                    sourceState: CurrentState,
                    targetState: nextState,
                    character: Character,
                    position: Position,
                    line: Line,
                    column: Column);
                GoToState(nextState);
            }
            else
            {
                if (!isCurrentStateAccepting)
                {
                    if (isCharEoi)
                    {
                        throw CreateUnexpectedEndOfInputException();
                    }

                    throw CreateUnexpectedCharacterException();
                }

                yield return CreateToken();
                Reset();
                History.Clear();
            }
        }
    }

    private IEnumerable<IToken> TokenizeWithoutDebugger()
    {
        /* machine body */
        while (true)
        {
            bool isCharEoi = Character == EoiChar;
            bool isCurrentStateInitialState = CurrentState.Id == InitialState.Id;
            bool isCurrentStateAccepting = CurrentState.IsAccepting;

            /* handle end of input (loop break) */
            if (isCharEoi && isCurrentStateInitialState)
            {
                yield break;
            }

            /* lookup next state */
            TokenizerState? nextState = Table.LookUp(CurrentState.Id, Character);

            if (nextState is null && CurrentState.IsRecursiveOnNoTransition)
            {
                GoToState(CurrentState);
                continue;
            }

            if (nextState is not null)
            {
                GoToState(nextState);
            }
            else
            {
                if (!isCurrentStateAccepting)
                {
                    if (isCharEoi)
                    {
                        throw CreateUnexpectedEndOfInputException();
                    }

                    throw CreateUnexpectedCharacterException();
                }

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
        if (UseDebugger)
        {
            return new Exception($"Unexpected end of input. At position {Position}. Line {Line}, column {Column}.\n\n{History}");
        }

        return new Exception($"Unexpected end of input. At position {Position}. Line {Line}, column {Column}.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Exception CreateUnexpectedCharacterException()
    {
        if (UseDebugger)
        {
            return new Exception($"Unexpected character '{Character}' in state {CurrentState.Name}. At position {Position}. Line {Line}, column {Column}.\n\n{History}");
        }

        return new Exception($"Unexpected character '{Character}'. At position {Position}. Line {Line}, column {Column}.");
    }

}