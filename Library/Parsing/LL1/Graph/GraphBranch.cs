using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Language.Graph;

public class GraphBranch
{
    public IReadOnlyList<LL1GraphNode> Nodes { get; }

    public GraphBranch(IEnumerable<LL1GraphNode> nodes)
    {
        Nodes = nodes.ToArray();

        if(Nodes.Count == 0)
        {
            throw new InvalidOperationException("The list of nodes is empty.");
        }
    }

    public LL1GraphNode Root => Nodes.First();

    public ProductionSet ToProductionSet()
    {
        var builder = new ProductionSetBuilder();

        foreach (var node in Nodes)
        {
            if (node.Production is not null)
            {
                builder.Add(node.Production.Value);
            }
        }

        return builder.Build();
    }
}
