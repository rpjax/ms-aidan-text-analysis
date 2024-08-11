using System.Text;

namespace Aidan.TextAnalysis.Language.Graph.Renderization;

public static class GraphNodeRenderExtensions
{
    public static CharMatrix Render(this LL1GraphNode node, bool isRoot = false)
    {
        var body = node.RenderBody();

        var children = node.GetChildren()
            .Select(c => c.Render(false))
            .ToArray();

        if (children.Length == 0)
        {
            if (isRoot)
            {
                return body.AddBox().FillGaps();
            }

            return body;
        }

        // double check that the body is not empty. It should not happen, since the body render method ensures that there is at least one character, even if it's a placeholder.
        if (body.Count() == 0)
        {
            throw new InvalidOperationException("The body of the node is empty.");
        }

        var matrix = new CharMatrix();
        var xOffset = 0;
        var xSpacing = 1;
        var childrenConnections = new List<GraphConnection>();

        foreach (var child in children)
        {
            if (!isRoot)
            {
                child.AddBox();
            }

            var childWidth = child.GetWidth();

            var childUpConnection = 
                child.AddUpConnection();

            childrenConnections.Add(childUpConnection);

            childUpConnection.Offset(xOffset, 0);
            child.Offset(xOffset, 0);

            xOffset += childWidth + xSpacing;
            matrix += child;
        }

        matrix.Offset(0, 1);
        matrix.AddHorizontalConnections(childrenConnections);

        var bodyConnection = matrix.AddUpConnection();

        /*
                    Full example:
                       ┌─────┐
                       │ABCD │
                       └──┬──┘
                      ┌───┴───┐
                    ┌─┴─┐   ┌─┴─┐
                    │ABC│   │ABC│
                    └───┘   └───┘
        */

        body.AddBox();
        body.AddDownConnection();

        var childrenWidth = matrix.GetWidth();
        var bodyHeight = body.GetHeight();
        var bodyOffset = (childrenWidth - body.GetWidth()) / 2;

        body.Offset(bodyOffset, 0);
        matrix.Offset(0, bodyHeight);

        matrix += body;

        return matrix.FillGaps();
    }

    private static CharMatrix RenderBody(this LL1GraphNode node)
    {
        var builder = new StringBuilder();

        // builds the content of the body.
        if (node.IsRecursive)
        {
            // denotes a recursive node. 
            // this notaion has to reviewed to make it more clear, i don't like the star, it's not expressive enough.
            builder.Append("*");
        }

        builder.Append(node.Symbol.ToString());

        // this ensures that the body is never empty.
        // if the body is empty, enumeration of CharMatrix will throw an exception. 
        // there has to be at least one character. The tree dots are used as a placeholder.
        if (builder.Length == 0)
        {
            builder.Append("...");
        }

        // copies the string to a char matrix.
        var str = builder.ToString();
        var matrix = new CharMatrix();

        var x = 0;
        var y = 0;

        foreach (var @char in str)
        {
            matrix.SetChar(x++, y, @char);

            if (@char == '\n')
            {
                x = 0;
                y++;
            }
        }

        /*
            Ensures that the body is always impar width, this is important for drawing the connections with proper alignment. This comes at the cost of adding a trailing space to the body, making the node content disaligned, but it preserves the connections alignment.
        */

        var width = matrix.GetWidth();

        if (width % 2 == 0)
        {
            matrix.SetChar(width, 0, ' ');
        }

        return matrix;
    }

}
