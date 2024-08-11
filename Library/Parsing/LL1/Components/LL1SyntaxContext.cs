using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Parsing.Components;
using Aidan.TextAnalysis.Tokenization;

namespace Aidan.TextAnalysis.Parsing.LL1.Components;

public class LL1SyntaxContext
{
    private Stack<CstBuilder> TreeBuilderStack { get; }

    public LL1SyntaxContext()
    {
        TreeBuilderStack = new();
    }

    public void CreateBranch(NonTerminal state)
    {
        TreeBuilderStack.Push(new CstBuilder());
    }

    public void AddAttribute(Token token)
    {
        if (!TreeBuilderStack.TryPeek(out var builder))
        {
            throw new InvalidOperationException("No tree builder on the stack.");
        }

        builder.CreateLeaf(token);
    }

    public void FinilizeBranch()
    {
        //if (TreeBuilderStack.Count == 0)
        //{
        //    throw new InvalidOperationException("The tree builder stack must contain at least one builder.");
        //}

        //var child = TreeBuilderStack.Pop();

        //if (TreeBuilderStack.TryPeek(out var parent))
        //{
        //    parent.Execute(state, child.AccumulatorCount);
        //}
        //else
        //{
        //    TreeBuilderStack.Push(child);
        //}
    }

    public CstNode BuildConcreteSyntaxTree()
    {
        if (TreeBuilderStack.Count != 1)
        {
            throw new InvalidOperationException("The tree builder stack must contain exactly one builder.");
        }

        return TreeBuilderStack.Pop().Build();
    }
}
