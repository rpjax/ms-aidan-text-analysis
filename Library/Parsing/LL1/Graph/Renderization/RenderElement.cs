using System.Text;

namespace Aidan.TextAnalysis.Language.Graph.Renderization;

public class RenderElement
{
    public virtual Canvas Render(RenderContext context)
    {
        var canvas = new Canvas();
        var childrenNodes = context.Node.GetChildren().ToArray();

        if (childrenNodes.Length == 0)
        {
            var bodyCanvas = RenderBody(context);
            var bodyWidth = bodyCanvas.GetWidth();
            var bodyHeight = bodyCanvas.GetHeight();
            var bodyBox = RenderBox(bodyWidth + 2, bodyHeight + 2);

            bodyCanvas.Move(1, 1);

            bodyCanvas
                .Draw(bodyBox)
                ;

            return bodyCanvas;
        }

        var children = new List<Canvas>();
        var childrenSpacing = 1;
        var childrenConnectionPositions = new List<Vec2>();

        foreach (var childNode in childrenNodes)
        {
            var childContext = new RenderContext(childNode);
            var childElement = new RenderElement();

            var childCanvas = childElement.Render(childContext);
            var upConntection = RenderUpConnection(childCanvas);
            var upConnectionPos = childCanvas.GetUpCenter();

            childCanvas
                .Draw(upConntection)
                ;

            children.Add(childCanvas);
            childrenConnectionPositions.Add(upConnectionPos);
        }

        //var horizontalConnections = RenderHorizontalConnections(children);

        var previousChild = null as Canvas;

        foreach (var child in children)
        {
            if (previousChild != null)
            {
                child.Move(previousChild.GetWidth() + 1, 0);
            }

            previousChild = child;
            canvas.Draw(child);
        }

        //canvas.Move(0, 1);
        //canvas.Draw(horizontalConnections);

        return canvas;
    }

    private Canvas RenderBody(RenderContext context)
    {
        var node = context.Node;
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
        var canvas = new Canvas();

        var x = 0;
        var y = 0;

        foreach (var @char in str)
        {
            canvas.DrawChar(x++, y, @char);

            if (@char == '\n')
            {
                x = 0;
                y++;
            }
        }

        /*
            Ensures that the body is always impar width, this is important for drawing the connections with proper alignment. This comes at the cost of adding a trailing space to the body, making the node content disaligned, but it preserves the connections alignment.
        */

        var width = canvas.GetWidth();

        if (width % 2 == 0)
        {
            canvas.DrawChar(width, 0, ' ');
        }

        return canvas;
    }

    private Canvas RenderBox(int width, int height)
    {
        var canvas = new Canvas();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (y == 0 && x == 0)
                {
                    canvas.DrawChar(x, y, '┌');
                }
                else if (y == 0 && x == width - 1)
                {
                    canvas.DrawChar(x, y, '┐');
                }
                else if (y == height - 1 && x == 0)
                {
                    canvas.DrawChar(x, y, '└');
                }
                else if (y == height - 1 && x == width - 1)
                {
                    canvas.DrawChar(x, y, '┘');
                }
                else if (y == 0 || y == height - 1)
                {
                    canvas.DrawChar(x, y, '─');
                }
                else if (x == 0 || x == width - 1)
                {
                    canvas.DrawChar(x, y, '│');
                }
            }
        }

        return canvas;
    }

    private Canvas RenderUpConnection(Canvas canvas)
    {
        var _canvas = new Canvas();
        var center = canvas.GetUpCenter();

        return _canvas.DrawChar(center.X, 0, '┴');
    }

    /// <summary>
    /// Generates the horizontal connections between the children nodes. 
    /// </summary>
    /// <remarks>
    /// Ex: 
    /// </remarks>
    /// <param name="children"></param>
    /// <returns></returns>
    private Canvas RenderHorizontalConnections(List<Canvas> children)
    {
        var canvas = new Canvas();

        if (children.Count == 0)
        {
            return canvas;
        }

        var positions = new List<Vec2>();
        var previousChild = null as Canvas;

        foreach (var child in children)
        {
            if (previousChild != null)
            {
                child.Move(previousChild.GetWidth() + 1, 0);
            }

            positions.Add(child.GetUpCenter());
            previousChild = child;
        }

        var pos_x_min = positions.Min(p => p.X);
        var pos_x_max = positions.Max(p => p.X);
        var isSingleConnection = positions.Count == 1;

        for (int xi = pos_x_min; xi < pos_x_max + 1; xi++)
        {
            var isConnection = positions.Any(p => p.X == xi);

            if (!isConnection)
            {
                canvas.DrawChar(xi, 0, '─');
                continue;
            }

            if (isSingleConnection)
            {
                canvas.DrawChar(xi, 0, '│');
                continue;
            }

            var isLeft = xi == pos_x_min;

            if (isLeft)
            {
                canvas.DrawChar(xi, 0, '┌');
                continue;
            }

            var isRight = xi == pos_x_max;

            if (isRight)
            {
                canvas.DrawChar(xi, 0, '┐');
                continue;
            }

            canvas.DrawChar(xi, 0, '┬');
        }

        return canvas;
    }

}

