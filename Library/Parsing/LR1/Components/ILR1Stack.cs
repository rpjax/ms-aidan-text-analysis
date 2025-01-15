using System.Runtime.CompilerServices;
using Aidan.TextAnalysis.Language.Components;

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

/// <summary>
/// Represents an implementation of <see cref="ILR1Stack"/>.
/// </summary>
public class LR1ParserStack : ILR1Stack
{
    private Stack<uint> StateStack { get; }
    private Stack<ISymbol> SymbolStack { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LR1ParserStack"/> class.
    /// </summary>
    public LR1ParserStack()
    {
        StateStack = new Stack<uint>();
        SymbolStack = new Stack<ISymbol>();
    }

    /// <summary>
    /// Gets the current state from the stack.
    /// </summary>
    /// <returns>The current state.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint GetCurrentState()
    {
        return StateStack.Peek();
    }

    /// <summary>
    /// Pushes a state onto the stack.
    /// </summary>
    /// <param name="state">The state to push.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PushState(uint state)
    {
        StateStack.Push(state);
    }

    /// <summary>
    /// Pushes a symbol onto the stack.
    /// </summary>
    /// <param name="symbol">The symbol to push.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PushSymbol(ISymbol symbol)
    {
        SymbolStack.Push(symbol);
    }

    /// <summary>
    /// Pops a state from the stack.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PopState()
    {
        StateStack.Pop();
    }

    /// <summary>
    /// Pops a symbol from the stack.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PopSymbol()
    {
        SymbolStack.Pop();
    }
}
