namespace Aidan.TextAnalysis.Tokenization.Machine;

public enum TokenizerAction
{
    /// <summary>
    /// No action, just transitions to the next state.
    /// </summary>
    None,

    /// <summary>
    /// Appends the current character to the accumulator and transitions to the next state.
    /// </summary>
    Read,

    /// <summary>
    /// Skips the current character and transitions to the next state.
    /// </summary>
    Skip,

    /// <summary>
    /// Emits a token and transitions to the next state.
    /// </summary>
    Emit,

    /// <summary>
    /// Emits an error token and stops the tokenization process.
    /// </summary>
    Error,

    /// <summary>
    /// Emits a token and stops the tokenization process.
    /// </summary>
    End,
}
