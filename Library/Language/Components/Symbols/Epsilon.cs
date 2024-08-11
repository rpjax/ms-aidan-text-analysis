namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents an epsilon(ε) symbol in a context-free grammar.
/// </summary>
public interface IEpsilon : ISymbol
{
}

/// <summary>
/// Represents an epsilon(ε) symbol in a context-free grammar.
/// </summary>
public class Epsilon : Symbol, IEpsilon
{
    /// <inheritdoc/>
    public override bool IsTerminal => true;

    /// <inheritdoc/>
    public override bool IsNonTerminal => false;

    /// <inheritdoc/>
    public override bool IsEpsilon => true;

    /// <inheritdoc/>
    public override bool IsMacro => false;

    /// <inheritdoc/>
    public override bool IsEoi => false;

    /// <summary>
    /// Initializes a new instance of the <see cref="Epsilon"/> class.
    /// </summary>
    public Epsilon()
    {
    }

    public static Epsilon Instance { get; } = new Epsilon();

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ ToString().GetHashCode();
            return hash;
        }
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Symbol);
    }

    public override bool Equals(Symbol? other)
    {
        return other is Epsilon;
    }

    public override bool Equals(ISymbol? other)
    {
        return other is IEpsilon;
    }

    public override string ToNotation(NotationType notation)
    {
        return GreekLetters.Epsilon.ToString();
    }

    /// <summary>
    /// Returns a string representation of the epsilon symbol.
    /// </summary>
    /// <returns>A string representation of the epsilon symbol.</returns>
    public override string ToString()
    {
        return GreekLetters.Epsilon.ToString();
    }

}
