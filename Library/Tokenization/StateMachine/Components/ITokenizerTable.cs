﻿namespace Aidan.TextAnalysis.Tokenization.StateMachine.Components;

/// <summary>
/// Represents a tokenizer table for managing states and transitions in a tokenizer DFA.
/// </summary>
public interface ITokenizerTable
{
    /// <summary>
    /// Gets the initial state of the tokenizer.
    /// </summary>
    /// <returns>The initial state.</returns>
    TokenizerState GetInitialState();

    /// <summary>
    /// Looks up the next state based on the current state and input character.
    /// </summary>
    /// <param name="state">The current state.</param>
    /// <param name="character">The input character.</param>
    /// <returns>The next state if found; otherwise, <c>null</c>.</returns>
    TokenizerState? LookUp(uint state, char character);

    /// <summary>
    /// Gets the entries of the tokenizer table.
    /// </summary>
    /// <returns>A dictionary containing the states and their corresponding transitions.</returns>
    Dictionary<TokenizerState, TokenizerTransition[]> GetEntries();
}
