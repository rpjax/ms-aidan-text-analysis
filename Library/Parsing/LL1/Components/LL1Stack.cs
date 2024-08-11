using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Parsing.LL1.Components;

public class LL1Stack
{
    private Stack<Symbol> Symbols { get; } = new();

    public LL1Stack()
    {

    }

    public Symbol? Top => Peek();

    public void Push(Symbol symbol)
    {
        Symbols.Push(symbol);
    }

    public void Push(ProductionRule production)
    {
        foreach (var symbol in production.Body.Reverse())
        {
            Symbols.Push(symbol);
        }
    }

    public Symbol Pop()
    {
        return Symbols.Pop();
    }

    public Symbol? Peek()
    {
        if (Symbols.Count == 0)
        {
            return null;
        }

        return Symbols.Peek();
    }
}
