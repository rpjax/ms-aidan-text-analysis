using Aidan.TextAnalysis.Language.Components.Symbols;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents a stack used in LR(1) parsing.
/// </summary>
public interface ILR1Stack
{
    /// <summary>
    /// Gets the current state from the stack.
    /// </summary>
    /// <returns>The current state.</returns>
    uint GetCurrentState();

    /// <summary>
    /// Pushes a state onto the stack.
    /// </summary>
    /// <param name="state">The state to push.</param>
    void PushState(uint state);

    /// <summary>
    /// Pushes a symbol onto the stack.
    /// </summary>
    /// <param name="symbol">The symbol to push.</param>
    void PushSymbol(ISymbol symbol);

    /// <summary>
    /// Pops a state from the stack.
    /// </summary>
    void PopState();

    /// <summary>
    /// Pops a symbol from the stack.
    /// </summary>
    void PopSymbol();
}
