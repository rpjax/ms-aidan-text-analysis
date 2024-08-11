namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents the end-of-input symbol($).
/// </summary>
public interface IEoi : ISymbol
{
}

/// <summary>
/// Represents the end-of-input symbol($).
/// </summary>
public sealed class Eoi : Terminal, IEoi
{
    public const string SententialRepresentation = "$";

    /// <inheritdoc/>
    public override bool IsEoi => true;

    /// <summary>
    /// Initializes a new instance of the <see cref="Eoi"/> class.
    /// </summary>
    public Eoi() : base(Tokenization.TokenType.Eoi, SententialRepresentation)
    {
    }

    public static Eoi Instance { get; } = new Eoi();

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
        return other is Eoi;
    }

    public override bool Equals(ISymbol? other)
    {
        return other is IEoi;
    }

    /// <summary>
    /// Returns a string representation of the end-of-input symbol.
    /// </summary>
    /// <returns>A string representation of the end-of-input symbol.</returns>
    public override string ToString()
    {
        return SententialRepresentation;
    }

    public override string ToNotation(NotationType notation)
    {
        return SententialRepresentation;
    }

}