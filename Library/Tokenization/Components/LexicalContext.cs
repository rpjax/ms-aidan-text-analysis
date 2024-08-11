using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Tokenization.Tools;
using System.Runtime.CompilerServices;
using System.Text;

namespace Aidan.TextAnalysis.Tokenization.Components;

/// <summary>
/// Represents the lexical context of the lexer.
/// </summary>
public class LexicalContext : IDisposable
{
    public int Position { get; private set; }
    public char? CurrentChar { get; private set; }
    public bool IsEndOfInput { get; private set; }

    public int Line { get; private set; }
    public int Column { get; private set; }

    private string Source { get; }
    private int AccumulatorLength { get; set; }

    public LexicalContext(string source)
    {
        Position = -1;
        Line = 1;
        Column = 0;

        Source = source;
        AccumulatorLength = 0;
        Advance();
    }

    public ReadOnlyMemory<char> AccumulatorValue => Source.AsMemory(Position - AccumulatorLength, AccumulatorLength);

    public void Dispose()
    {
      
    }

    /// <summary>
    /// Adds the current character to the accumulator and advances the enumerator by one character.
    /// </summary>
    /// <returns><c>true</c> if a character is read successfully; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Consume()
    {
        if (IsEndOfInput)
        {
            return false;
        }

        LoadCurrentCharToAccumulator();
        UpdateMetadata();
        Advance();
        return true;
    }

    /// <summary>
    /// Advances the enumerator by one character without adding it to the accumulator.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Skip()
    {
        //if (AccumulatorLength != 0)
        //{
        //    throw new InvalidOperationException("Invalid skip operation. The lexer character accumulator is not empty.");
        //}
        if (IsEndOfInput)
        {
            return false;
        }

        UpdateMetadata();
        Advance();
        return true;
    }

    /// <summary>
    /// Resets the character accumulator.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResetAccumulator()
    {
        AccumulatorLength = 0;
    }

    /*
     * private methods.
     */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool Advance()
    {
        Position++;
        IsEndOfInput = Position >= Source.Length;
        CurrentChar = IsEndOfInput
            ? null
            : Source[Position];

        return !IsEndOfInput;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateMetadata()
    {
        if (CurrentChar == '\n')
        {
            Line++;
            Column = 1;
        }
        else
        {
            Column++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LoadCurrentCharToAccumulator()
    {
        AccumulatorLength++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal TokenMetadata GetMetadata()
    {
        return new TokenMetadata(
            position: GetTokenPosition()
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal TokenPosition GetTokenPosition()
    {
        return new TokenPosition(
            startIndex: Position - AccumulatorLength,
            endIndex: Position,
            line: Line,
            column: Column
        );
    }

}
