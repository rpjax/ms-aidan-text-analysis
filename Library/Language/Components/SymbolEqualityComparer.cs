namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Provides a way to compare two <see cref="ISymbol"/> objects for equality.
/// </summary>
public class SymbolEqualityComparer : IEqualityComparer<ISymbol>
{
    /// <summary>
    /// Determines whether the specified <see cref="ISymbol"/> objects are equal.
    /// </summary>
    /// <param name="x">The first <see cref="ISymbol"/> to compare.</param>
    /// <param name="y">The second <see cref="ISymbol"/> to compare.</param>
    /// <returns>true if the specified <see cref="ISymbol"/> objects are equal; otherwise, false.</returns>
    public bool Equals(ISymbol? x, ISymbol? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return x.Equals(y);
    }

    /// <summary>
    /// Returns a hash code for the specified <see cref="ISymbol"/>.
    /// </summary>
    /// <param name="obj">The <see cref="ISymbol"/> for which a hash code is to be returned.</param>
    /// <returns>A hash code for the specified <see cref="ISymbol"/>.</returns>
    public int GetHashCode(ISymbol obj)
    {
        return obj.Name.GetHashCode();
    }
}

/// <summary>
/// Provides a way to compare two <see cref="ITerminal"/> objects for equality.
/// </summary>
public class TerminalEqualityComparer : IEqualityComparer<ITerminal>
{
    /// <summary>
    /// Determines whether the specified <see cref="ITerminal"/> objects are equal.
    /// </summary>
    /// <param name="x">The first <see cref="ITerminal"/> to compare.</param>
    /// <param name="y">The second <see cref="ITerminal"/> to compare.</param>
    /// <returns>true if the specified <see cref="ITerminal"/> objects are equal; otherwise, false.</returns>
    public bool Equals(ITerminal? x, ITerminal? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return x.Equals(y);
    }

    /// <summary>
    /// Returns a hash code for the specified <see cref="ITerminal"/>.
    /// </summary>
    /// <param name="obj">The <see cref="ITerminal"/> for which a hash code is to be returned.</param>
    /// <returns>A hash code for the specified <see cref="ITerminal"/>.</returns>
    public int GetHashCode(ITerminal obj)
    {
        return obj.Name.GetHashCode();
    }
}

/// <summary>
/// Provides a way to compare two <see cref="INonTerminal"/> objects for equality.
/// </summary>
public class NonTerminalEqualityComparer : IEqualityComparer<INonTerminal>
{
    /// <summary>
    /// Determines whether the specified <see cref="INonTerminal"/> objects are equal.
    /// </summary>
    /// <param name="x">The first <see cref="INonTerminal"/> to compare.</param>
    /// <param name="y">The second <see cref="INonTerminal"/> to compare.</param>
    /// <returns>true if the specified <see cref="INonTerminal"/> objects are equal; otherwise, false.</returns>
    public bool Equals(INonTerminal? x, INonTerminal? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return x.Equals(y);
    }

    /// <summary>
    /// Returns a hash code for the specified <see cref="INonTerminal"/>.
    /// </summary>
    /// <param name="obj">The <see cref="INonTerminal"/> for which a hash code is to be returned.</param>
    /// <returns>A hash code for the specified <see cref="INonTerminal"/>.</returns>
    public int GetHashCode(INonTerminal obj)
    {
        return obj.Name.GetHashCode();
    }
}
