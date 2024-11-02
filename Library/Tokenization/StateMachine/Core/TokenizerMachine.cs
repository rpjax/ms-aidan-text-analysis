using System.Runtime.CompilerServices;

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
    private bool IncludeEoi { get; set; }
    private string EoiName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenizerMachine"/> class.
    /// </summary>
    /// <param name="table">The tokenizer table.</param>
    /// <param name="includeEoi">Whether to include the end of input token.</param>
    /// <param name="eoiName">The name of the end of input token.</param>
    public TokenizerMachine(
        ITokenizerTable table,
        bool includeEoi = false,
        string eoiName = EoiDefaultName)
    {
        Table = table;
        IncludeEoi = includeEoi;
        EoiName = eoiName;

        if (!includeEoi && eoiName != EoiDefaultName)
        {
            includeEoi = true;
        }
    }

    /// <summary>
    /// Sets whether to include the end of input token.
    /// </summary>
    /// <param name="includeEoi">Whether to include the end of input token.</param>
    public void SetIncludeEoi(bool includeEoi)
    {
        IncludeEoi = includeEoi;
    }

    /// <summary>
    /// Sets the name of the end of input token.
    /// </summary>
    /// <param name="eoiName">The name of the end of input token.</param>
    public void SetEoiName(string eoiName)
    {
        EoiName = eoiName;
    }

    /// <summary>
    /// Tokenizes the specified input string.
    /// </summary>
    /// <param name="input">The input string to tokenize.</param>
    /// <returns>An enumerable of tokens.</returns>
    public IEnumerable<IToken> Tokenize(string input)
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
                throw new Exception($"No state found for character '{character}' in state {currentState.Name}.");
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

}

/// <summary>
/// Represents the context of the tokenization process.
/// </summary>
public class TokenizationContext
{
    public int CurrentState { get; set; }
    public int Position { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }
    public int TokenStart { get; set; }
    public int InitialState { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenizationContext"/> class.
    /// </summary>
    /// <param name="initialState">The initial state of the tokenization process.</param>
    public TokenizationContext(int initialState)
    {
        CurrentState = initialState;
        Position = 0;
        Line = 1;
        Column = 1;
        TokenStart = 0;
        InitialState = initialState;
    }
}
