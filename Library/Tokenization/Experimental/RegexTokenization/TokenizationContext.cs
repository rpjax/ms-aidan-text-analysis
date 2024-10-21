namespace Aidan.TextAnalysis.Tokenization.Experimental.RegexTokenization;

/// <summary>
/// Representa o contexto de tokenização, incluindo o estado atual do índice, linha e coluna.
/// </summary>
public class TokenizationContext : ITokenizationContext
{
    private string Input { get; }
    private int Index { get; set; }
    private int Line { get; set; }
    private int Column { get; set; }
    private int TokenStartIndex { get; set; }
    private int CurrentState { get; set; }

    public TokenizationContext(string input, int initialState = 0)
    {
        Input = input;
        Index = 0;
        Line = 0;
        Column = 0;
        TokenStartIndex = 0;
        CurrentState = initialState;
    }

    public int GetIndex() => Index;

    public int GetLine() => Line;

    public int GetColumn() => Column;

    public char? GetCurrentChar() => Index < Input.Length ? Input[Index] : null;

    public ReadOnlyMemory<char> GetTokenValue() => Input.AsMemory().Slice(TokenStartIndex, Index - TokenStartIndex);

    public void AdvanceCharacter() => Index++;

    public void SkipCharacter()
    {
        if (Index < Input.Length)
        {
            Index++;
            Column++;
        }
    }

    public int GetCurrentState() => CurrentState;

    public void GotoState(int stateId) => CurrentState = stateId;

    public void BreakLine()
    {
        Line++;
        Column = 0;
        AdvanceCharacter();
    }

    public TokenMetadata GetTokenMetadata()
    {
        var position = new TokenPosition(
            start: TokenStartIndex,
            end: Index,
            line: Line,
            column: Column);

        return new TokenMetadata(position);
    }

    public bool IsInInitialState() => CurrentState == 0;
}
