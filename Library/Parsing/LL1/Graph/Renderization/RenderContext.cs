namespace Aidan.TextAnalysis.Language.Graph.Renderization;

public class RenderContext
{
    public LL1GraphNode Node { get; }

    public RenderContext(LL1GraphNode node)
    {
        Node = node;
    }

    public LL1GraphNode? Parent => Node.Parent;

    public RenderContext CreateChildContext(LL1GraphNode node)
    {
        if(Node.GetChildren().All(n => n != node))
        {
            throw new InvalidOperationException("The node is not a child of the current node.");
        }

        return new RenderContext(node);
    }
}
