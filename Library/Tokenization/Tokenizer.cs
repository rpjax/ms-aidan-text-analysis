using Aidan.TextAnalysis.Tokenization.Components;
using Aidan.TextAnalysis.Tokenization.StateMachine;
using Aidan.TextAnalysis.Tokenization.StateMachine.Components;

namespace Aidan.TextAnalysis.Tokenization;

/// <summary>
/// A state machine for tokenizing strings.
/// </summary>
public class Tokenizer : IStringTokenizer
{
    /// <summary>
    /// The end of input character.
    /// </summary>
    public const char EoiChar = '\0';

    /// <summary>
    /// The default name for the end of input token.
    /// </summary>
    public const string EoiDefaultName = "\0";

    /* Dependencies */
    private TokenizerTable Table { get; }

    /* Settings */
    private bool UseDebugger { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tokenizer"/> class.
    /// </summary>
    /// <param name="table">The tokenizer table.</param>
    /// <param name="useDebugger">Whether to include the debugger.</param>
    public Tokenizer(
        TokenizerTable table,
        bool useDebugger = false)
    {
        Table = table;
        UseDebugger = useDebugger;
    }

    /// <summary>
    /// Tokenizes the specified input string.
    /// </summary>
    /// <param name="input">The input string to tokenize.</param>
    /// <returns>An enumerable of tokens.</returns>
    public IEnumerable<IToken> Tokenize(string input)
    {
        var process = new TokenizationMachine(
            table: Table,
            input: input,
            useDebugger: UseDebugger);

        return process.Tokenize();
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

}
