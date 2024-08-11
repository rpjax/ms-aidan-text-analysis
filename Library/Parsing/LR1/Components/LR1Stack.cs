using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Tokenization;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

public class LR1Stack
{
    private Stack<object> Stack { get; }

    public LR1Stack()
    {
        Stack = new Stack<object>();
    }

    public int Count => Stack.Count;

    public override string ToString()
    {
        return string.Join(" ", Stack.Reverse().Select(x => x.ToString()));
    }

    public int PeekState()
    {
        if(Stack.Count == 0)
        {
            throw new InvalidOperationException("The stack is empty.");
        }

        var state = Stack.Peek();

        if(state is int _int)
        {
            return _int;
        }

        return -1;
    }

    public NonTerminal? PeekSymbol()
    {
        if (Stack.Count == 0)
        {
            throw new InvalidOperationException("The stack is empty.");
        }

        var state = Stack.Peek();

        if (state is NonTerminal nonTerminal)
        {
            return nonTerminal;
        }

        return null;
    }

    public void PushState(int state)
    {
        Stack.Push(state);
    }

    public void PushToken(Token token)
    {
        Stack.Push(token);
    }

    public void PushNonTerminal(NonTerminal nonTerminal)
    {
        Stack.Push(nonTerminal);
    }

    public void Pop()
    {
        if(Stack.Count == 0)
        {
            throw new InvalidOperationException("The stack is empty.");
        }

        Stack.Pop();
    }

}
