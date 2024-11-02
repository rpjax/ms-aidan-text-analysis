namespace Aidan.TextAnalysis.Tokenization.StateMachine;

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
