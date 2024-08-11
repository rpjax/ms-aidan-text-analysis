using System.Collections;
using System.Text;

namespace Aidan.TextAnalysis.Language.Graph.Renderization;

public class CharMatrix : IEnumerable<CharVec2>
{
    public int Length => Chars.Count;

    private List<CharVec2> Chars { get; }
    private bool BoxAdded { get; set; }

    public CharMatrix()
    {
        Chars = new();
    }

    public static CharMatrix operator +(CharMatrix left, CharMatrix right)
    {
        var matrix = new CharMatrix();

        foreach (var cvec in left)
        {
            matrix.SetChar(cvec);
        }

        foreach (var cvec in right)
        {
            matrix.SetChar(cvec);
        }

        return matrix;
    }

    public IEnumerator<CharVec2> GetEnumerator()
    {
        return Chars.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        var matrix = this
            .OrderBy(c => c.Y)
            .ThenBy(c => c.X);

        var builder = new StringBuilder();

        foreach (var @char in matrix)
        {
            builder.Append(@char.Char);
        }

        return builder.ToString();
    }

    public int GetWidth()
    {
        if (Chars.Count == 0)
        {
            return 0;
        }

        return Chars.Max(c => c.X) + 1;
    }

    public int GetHeight()
    {
        if (Chars.Count == 0)
        {
            return 0;
        }

        return Chars.Max(c => c.Y) + 1;
    }

    public CharMatrix SetChar(CharVec2 cvec)
    {
        UnsetChar(cvec.X, cvec.Y);
        Chars.Add(new CharVec2(x: cvec.X, y: cvec.Y, c: cvec.Char));
        return this;
    }

    public CharMatrix SetChar(int x, int y, char @c)
    {
        UnsetChar(x, y);
        Chars.Add(new CharVec2(x, y, c));
        return this;
    }

    public CharMatrix UnsetChar(int x, int y)
    {
        var index = Chars.FindIndex(c => c.X == x && c.Y == y);

        if (index != -1)
        {
            Chars.RemoveAt(index);
        }

        return this;

    }

    public void Offset(int x, int y)
    {
        for (int i = 0; i < Chars.Count; i++)
        {
            var c = Chars[i];

            Chars[i] = new CharVec2(
                x: c.X + x,
                y: c.Y + y,
                c: c.Char
            );
        }
    }

    public CharMatrix AddBox()
    {
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

        // creates an ascii art style box around the body of the node

        if (BoxAdded)
        {
            return this;
        }

        var width = GetWidth();
        var height = GetHeight();

        Offset(1, 1);

        var a = new CharVec2(0, 0, '┌');
        var b = new CharVec2(width + 1, 0, '┐');
        var c = new CharVec2(0, height + 1, '└');
        var d = new CharVec2(width + 1, height + 1, '┘');

        SetChar(a);
        SetChar(b);
        SetChar(c);
        SetChar(d);

        for (int i = 1; i <= width; i++)
        {
            SetChar(new CharVec2(i, 0, '─'));
            SetChar(new CharVec2(i, height + 1, '─'));
        }

        for (int i = 1; i <= height; i++)
        {
            SetChar(new CharVec2(0, i, '│'));
            SetChar(new CharVec2(width + 1, i, '│'));
        }

        /*
            Corners:
               a    b
               ┌────┐
               │    │
               └────┘
               c    d
        */

        BoxAdded = true;
        return this;
    }

    public GraphConnection AddDownConnection()
    {
        var width = GetWidth();
        var height = GetHeight();
        var x = (width - 1) / 2;
        var y = height - 1;

        SetChar(x, y, '┬');

        return new GraphConnection(new Vec2(x, y), ConnectionOrientation.Down);
    }

    public GraphConnection AddUpConnection()
    {
        var width = GetWidth();
        var x = (width - 1) / 2;

        SetChar(x, 0, '┴');

        return new GraphConnection(new Vec2(x, 0), ConnectionOrientation.Up);
    }

    public CharMatrix AddHorizontalConnections(IEnumerable<GraphConnection> connections)
    {
        var positions = connections
            .Select(x => x.Position)
            .ToArray();

        if (positions.Length == 0)
        {
            return this;
        }

        var width = GetWidth();
        var pos_x_min = positions.Min(p => p.X);
        var pos_x_max = positions.Max(p => p.X);
        var isSingleConnection = positions.Length == 1;

        for (int xi = pos_x_min; xi < pos_x_max + 1; xi++)
        {
            var isConnection = positions.Any(p => p.X == xi);

            if (!isConnection)
            {
                SetChar(xi, 0, '─');
                continue;
            }

            if (isSingleConnection)
            {
                SetChar(xi, 0, '│');
                continue;
            }

            var isLeft = xi == pos_x_min;

            if (isLeft)
            {
                SetChar(xi, 0, '┌');
                continue;
            }

            var isRight = xi == pos_x_max;

            if (isRight)
            {
                SetChar(xi, 0, '┐');
                continue;
            }

            SetChar(xi, 0, '┬');
        }

        return this;
    }

    public CharMatrix FillGaps()
    {
        var maxX = Chars.Max(c => c.X);
        var maxY = Chars.Max(c => c.Y);

        for (int yi = 0; yi <= maxY; yi++)
        {
            for (int xi = 0; xi <= maxX; xi++)
            {
                var c = Chars.FirstOrDefault(c => c.X == xi && c.Y == yi);

                if (c == null)
                {
                    SetChar(xi, yi, ' ');
                }
            }

            SetChar(maxX + 1, yi, '\n');
        }

        return this;
    }

}

public enum ConnectionOrientation
{
    Up,
    Down
}
