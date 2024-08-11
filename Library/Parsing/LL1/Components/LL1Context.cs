using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Parsing.Components;

namespace Aidan.TextAnalysis.Parsing.LL1.Components;

public class LL1Context
{
    public LL1Grammar Grammar { get; }
    public InputStream InputStream { get; }
    public LL1Stack Stack { get; }
    public LL1ParsingTable ParsingTable { get; }
    public LL1SyntaxContext SyntaxContext { get; }

    private bool IsInit { get; set; }

    public LL1Context(LL1Grammar grammar, InputStream inputStream, LL1Stack stack)
    {
        Grammar = grammar;
        InputStream = inputStream;
        Stack = stack;
        ParsingTable = grammar.GetParsingTable();
        SyntaxContext = new();
    }

    public void Init()
    {
        if (IsInit)
        {
            throw new InvalidOperationException();
        }

        IsInit = true;
        Stack.Push(Eoi.Instance);
        Stack.Push(Grammar.Start);
    }
}

