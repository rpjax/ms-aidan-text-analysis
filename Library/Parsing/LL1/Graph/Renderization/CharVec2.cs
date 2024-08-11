namespace Aidan.TextAnalysis.Language.Graph.Renderization;

public class Vec2
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Vec2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static bool operator ==(Vec2 left, Vec2 right)
    {
        return left.X == right.X && left.Y == right.Y;
    }

    public static bool operator !=(Vec2 left, Vec2 right)
    {
        return !(left == right);
    }

    public void Offset(int x, int y)
    {
        X += x;
        Y += y;
    }
}

public class CharVec2
{
    public Vec2 Position { get; }
    public char Char { get; }

    public CharVec2(int x, int y, char c)
    {
        Position = new Vec2(x, y);
        Char = c;
    }

    public int X => Position.X;
    public int Y => Position.Y;

    override public string ToString()
    {
        return $"({X}, {Y}) = {Char}";
    }
}
