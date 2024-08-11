using System.Text;

namespace Aidan.TextAnalysis.Language.Graph.Renderization;

public class Canvas
{
    public Vec2 Position { get; } = new(0, 0);
    public CharMatrix Chars { get; } = new();

    public override string ToString()
    {
        var max_x = GetWidth();
        var max_y = GetHeight();
        var builder = new StringBuilder();

        for (var y = 0; y < max_y; y++)
        {
            for (var x = 0; x < max_x; x++)
            {
                var c = TryGetChar(x, y);

                if (c is null)
                {
                    builder.Append(' ');
                }
                else
                {
                    builder.Append(c.Value);
                }
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }

    public void Move(int x, int y)
    {
        Chars.Offset(x, y);
    }

    public int GetMinX()
    {
        if (Chars.Length == 0)
        {
            return 0;
        }

        return Chars.Min(x => x.X);
    }

    public int GetMinY()
    {
        if (Chars.Length == 0)
        {
            return 0;
        }

        return Chars.Min(x => x.Y);
    }

    public int GetMaxX()
    {
        if (Chars.Length == 0)
        {
            return 0;
        }

        return Chars.Max(x => x.X);
    }

    public int GetMaxY()
    {
        if (Chars.Length == 0)
        {
            return 0;
        }

        return Chars.Max(x => x.Y);
    }

    public int GetWidth()
    {
        if (Chars.Length == 0)
        {
            return 0;
        }

        return GetMaxX() - GetMinX() + 1;
    }

    public int GetHeight()
    {
        if (Chars.Length == 0)
        {
            return 0;
        }

        return GetMaxY() - GetMinY() + 1;
    }

    public Vec2 GetUpCenter()
    {
        var width = GetWidth();

        if (width % 2 == 0)
        {
            throw new InvalidOperationException("The width of the canvas must be an odd number to get the center.");
        }

        return new Vec2((width - 1) / 2, 0);
    }

    public Canvas DrawChar(int x, int y, char c)
    {
        Chars.SetChar(x + Position.X, y + Position.Y, c);
        return this;
    }

    public Canvas Draw(Canvas canvas)
    {
        foreach (var c in canvas.Chars)
        {
            DrawChar(c.X, c.Y, c.Char);
        }

        return this;
    }

    public char? TryGetChar(int x, int y)
    {
        return Chars
            .Where(c => c.X == x && c.Y == y)
            .Select(c => c.Char)
            .FirstOrDefault();
    }

}
