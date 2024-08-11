namespace Aidan.TextAnalysis.Language.Graph;

public class GraphVisitor
{
    protected virtual LL1GraphNode Visit(LL1GraphNode node)
    {
        if (node.IsRecursive)
        {
            return node;
        }
        
        for (int i = 0; i < node.ChildrenCount; i++)
        {
            node[i] = Visit(node[i]);
        }

        return node;
    }

}
