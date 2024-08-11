namespace Aidan.TextAnalysis.Parsing.Components;

public class CstNodeVisitor
{
    public virtual CstNode Visit(CstNode node)
    {
        return node.Accept(this);
    }

    public virtual CstNode VisitRoot(CstRootNode node)
    {
        return node;
    }

    public virtual CstNode VisitNonTerminal(CstInternalNode node)
    {
        return node;
    }

    public virtual CstNode VisitTerminal(CstLeafNode node)
    {
        return node;
    }

}
