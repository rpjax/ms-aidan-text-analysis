using Aidan.TextAnalysis.Tokenization.StateMachine.Components;

namespace Aidan.TextAnalysis.Tokenization.StateMachine.Builders;

public enum CharType
{
    Digit,
    Letter,
    Whitespace,
    Punctuation,
    Control
}

public class TableTransitionBuilderBase<TBuilder> where TBuilder : TableTransitionBuilderBase<TBuilder>
{
    protected TokenizerState CurrentState { get; }
    protected char[] Charset { get; }
    protected List<char> Characters { get; }

    public TableTransitionBuilderBase(
        TokenizerState currentState, 
        char[] charset)
    {
        CurrentState = currentState;
        Charset = charset;
        Characters = new();
    }

    public TBuilder OnCharacter(char character)
    {
        AddCharacter(character);
        return (TBuilder)this;
    }

    /// <summary>
    /// Adds a range of characters that trigger transitions.
    /// </summary>
    /// <param name="start">The starting character of the range.</param>
    /// <param name="end">The ending character of the range.</param>
    /// <returns>The current <see cref="TableTransitionBuilder"/> instance.</returns>
    public TBuilder OnCharacterRange(char start, char end)
    {
        for (var i = start; i <= end; i++)
        {
            AddCharacter(i);
        }

        return (TBuilder)this;
    }

    /// <summary>
    /// Adds multiple characters that trigger transitions.
    /// </summary>
    /// <param name="characters">The characters to add.</param>
    /// <returns>The current <see cref="TableTransitionBuilder"/> instance.</returns>
    public TBuilder OnManyCharacters(params char[] characters)
    {
        foreach (var character in characters)
        {
            AddCharacter(character);
        }

        return (TBuilder)this;
    }

    /// <summary>
    /// Adds all characters in the charset as triggers for transitions.
    /// </summary>
    /// <returns>The current <see cref="TableTransitionBuilder"/> instance.</returns>
    public TBuilder OnAnyCharacter()
    {
        foreach (var character in Charset)
        {
            AddCharacter(character);
        }

        return (TBuilder)this;
    }

    /// <summary>
    /// Adds all characters in the charset, except the specified ones, as triggers for transitions.
    /// </summary>
    /// <param name="characters">The characters to exclude.</param>
    /// <returns>The current <see cref="TableTransitionBuilder"/> instance.</returns>
    public TBuilder OnAnyCharacterExcept(params char[] characters)
    {
        foreach (var character in Charset)
        {
            if (characters.Contains(character))
            {
                continue;
            }

            AddCharacter(character);
        }

        return (TBuilder)this;
    }

    /// <summary>
    /// Adds all characters in the charset, except those in the specified range, as triggers for transitions.
    /// </summary>
    /// <param name="start">The starting character of the range to exclude.</param>
    /// <param name="end">The ending character of the range to exclude.</param>
    /// <returns>The current <see cref="TableTransitionBuilder"/> instance.</returns>
    public TBuilder OnAnyCharacterExceptRange(char start, char end)
    {
        foreach (var character in Charset)
        {
            if (character < start || character > end)
            {
                AddCharacter(character);
            }
        }

        return (TBuilder)this;
    }

    /// <summary>
    /// Removes the specified characters from the list of characters that trigger transitions.
    /// </summary>
    /// <param name="characters">The characters to remove.</param>
    /// <returns>The current <see cref="TableTransitionBuilder"/> instance.</returns>
    public TBuilder Except(params char[] characters)
    {
        foreach (var character in characters)
        {
            Characters.Remove(character);
        }

        return (TBuilder)this;
    }

    /// <summary>
    /// Removes the characters in the specified range from the list of characters that trigger transitions.
    /// </summary>
    /// <param name="start">The starting character of the range to remove.</param>
    /// <param name="end">The ending character of the range to remove.</param>
    /// <returns>The current <see cref="TableTransitionBuilder"/> instance.</returns>
    public TBuilder ExceptRange(char start, char end)
    {
        for (var i = start; i <= end; i++)
        {
            Characters.Remove(i);
        }

        return (TBuilder)this;
    }

    /// <summary>
    /// Removes whitespace characters from the list of characters that trigger transitions.
    /// </summary>
    /// <returns>The current <see cref="TableTransitionBuilder"/> instance.</returns>
    public TBuilder ExceptWhitespace()
    {
        return Except(' ');
    }

    /// <summary>
    /// Adds the end of input character as a trigger for a transition.
    /// </summary>
    /// <returns>The current <see cref="TableTransitionBuilder"/> instance.</returns>
    public TBuilder OnEoi()
    {
        return OnCharacter(Tokenizer.EoiChar);
    }

    /// <summary>
    /// Adds a whitespace character as a trigger for a transition.
    /// </summary>
    /// <returns>The current <see cref="TableTransitionBuilder"/> instance.</returns>
    public TBuilder OnWhitespace()
    {
        return OnCharacter(' ');
    }

    /// <summary>
    /// Adds a digit character as a trigger for a transition.
    /// </summary>
    /// <returns>The current <see cref="TableTransitionBuilder"/> instance.</returns>
    public TBuilder OnDigit()
    {
        return OnCharacterRange('0', '9');
    }

    /* Protected Methods */

    protected void AddCharacter(char character)
    {
        if (Characters.Contains(character))
        {
            throw new InvalidOperationException($"Character '{character}' already added.");
        }

        Characters.Add(character);
    }

}
