using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Tokenization.StateMachine.Components;

namespace Aidan.TextAnalysis.Tokenization.StateMachine.Builders;

/// <summary>
/// Provides a base class for building transitions in a DFA (Deterministic Finite Automaton) tokenizer table.
/// <br/> This class includes methods for adding, excluding, and modifying the characters that trigger transitions for a given state.
/// </summary>
/// <typeparam name="TBuilder">The specific implementation of the builder that inherits from this base class.</typeparam>
public class TableTransitionBuilderBase<TBuilder> where TBuilder : TableTransitionBuilderBase<TBuilder>
{
    /// <summary>
    /// The current state being configured in the DFA.
    /// </summary>
    protected TokenizerState CurrentState { get; }

    /// <summary>
    /// The charset of the DFA, representing all valid characters the DFA can process.
    /// </summary>
    protected Charset Charset { get; }

    /// <summary>
    /// The list of characters that currently trigger transitions for the specified state.
    /// </summary>
    protected List<char> Characters { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableTransitionBuilderBase{TBuilder}"/> class.
    /// </summary>
    /// <param name="currentState">The current state being configured.</param>
    /// <param name="charset">The charset of the DFA.</param>
    public TableTransitionBuilderBase(
        TokenizerState currentState,
        Charset charset)
    {
        CurrentState = currentState;
        Charset = charset;
        Characters = new();
    }

    /// <summary>
    /// Adds a single character as a trigger for a transition.
    /// </summary>
    /// <param name="character">The character to add.</param>
    /// <returns>The current builder instance for further configuration.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the character has already been added.</exception>
    public TBuilder OnCharacter(char character)
    {
        AddCharacter(character);
        return (TBuilder)this;
    }

    /// <summary>
    /// Adds a range of characters as triggers for transitions.
    /// </summary>
    /// <param name="start">The starting character of the range.</param>
    /// <param name="end">The ending character of the range.</param>
    /// <returns>The current builder instance for further configuration.</returns>
    public TBuilder OnCharacterRange(char start, char end)
    {
        for (var i = start; i <= end; i++)
        {
            AddCharacter(i);
        }

        return (TBuilder)this;
    }

    /// <summary>
    /// Adds multiple characters as triggers for transitions.
    /// </summary>
    /// <param name="characters">The characters to add.</param>
    /// <returns>The current builder instance for further configuration.</returns>
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
    /// <returns>The current builder instance for further configuration.</returns>
    public TBuilder OnAnyCharacter()
    {
        foreach (var character in Charset)
        {
            AddCharacter(character);
        }

        return (TBuilder)this;
    }

    /// <summary>
    /// Adds all characters in the charset except the specified ones as triggers for transitions.
    /// </summary>
    /// <param name="characters">The characters to exclude.</param>
    /// <returns>The current builder instance for further configuration.</returns>
    public TBuilder OnAnyCharacterExcept(params char[] characters)
    {
        foreach (var character in Charset)
        {
            if (!characters.Contains(character))
            {
                AddCharacter(character);
            }
        }

        return (TBuilder)this;
    }

    /// <summary>
    /// Adds all characters in the charset except those within the specified range as triggers for transitions.
    /// </summary>
    /// <param name="start">The starting character of the range to exclude.</param>
    /// <param name="end">The ending character of the range to exclude.</param>
    /// <returns>The current builder instance for further configuration.</returns>
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
    /// Removes the specified characters from the list of triggers for transitions.
    /// </summary>
    /// <param name="characters">The characters to remove.</param>
    /// <returns>The current builder instance for further configuration.</returns>
    public TBuilder Except(params char[] characters)
    {
        foreach (var character in characters)
        {
            Characters.Remove(character);
        }

        return (TBuilder)this;
    }

    /// <summary>
    /// Removes characters in the specified range from the list of triggers for transitions.
    /// </summary>
    /// <param name="start">The starting character of the range to remove.</param>
    /// <param name="end">The ending character of the range to remove.</param>
    /// <returns>The current builder instance for further configuration.</returns>
    public TBuilder ExceptRange(char start, char end)
    {
        for (var i = start; i <= end; i++)
        {
            Characters.Remove(i);
        }

        return (TBuilder)this;
    }

    /// <summary>
    /// Removes whitespace characters from the list of triggers for transitions.
    /// </summary>
    /// <returns>The current builder instance for further configuration.</returns>
    public TBuilder ExceptWhitespace()
    {
        return Except(' ');
    }

    /// <summary>
    /// Adds the end-of-input character as a trigger for transitions.
    /// </summary>
    /// <returns>The current builder instance for further configuration.</returns>
    public TBuilder OnEoi()
    {
        return OnCharacter(Tokenizer.EoiChar);
    }

    /// <summary>
    /// Adds whitespace as a trigger for transitions.
    /// </summary>
    /// <returns>The current builder instance for further configuration.</returns>
    public TBuilder OnWhitespace()
    {
        return OnCharacter(' ');
    }

    /// <summary>
    /// Adds digit characters ('0'-'9') as triggers for transitions.
    /// </summary>
    /// <returns>The current builder instance for further configuration.</returns>
    public TBuilder OnDigit()
    {
        return OnCharacterRange('0', '9');
    }

    /// <summary>
    /// Adds the espace bar <c>'\'</c> as a trigger for transitions.
    /// </summary>
    /// <returns>The current builder instance for further configuration.</returns>
    public TBuilder OnEscapeBar()
    {
        return OnCharacter('\\');
    }

    /// <summary>
    /// Adds a character to the list of triggers for transitions, ensuring no duplicates.
    /// </summary>
    /// <param name="character">The character to add.</param>
    /// <exception cref="InvalidOperationException">Thrown if the character is already in the list of triggers.</exception>
    protected void AddCharacter(char character)
    {
        if (Characters.Contains(character))
        {
            throw new InvalidOperationException($"Character '{character}' already added.");
        }

        Characters.Add(character);
    }
}
