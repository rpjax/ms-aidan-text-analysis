namespace Aidan.TextAnalysis.Language.Graph.Renderization;

public class GraphConnection
{
    public Vec2 Position { get; }
    public ConnectionOrientation Orientation { get; }

    public GraphConnection(Vec2 position, ConnectionOrientation orientation)
    {
        Position = position;
        Orientation = orientation;
    }

    public void Offset(int x, int y)
    {
        Position.Offset(x, y);
    }
}
