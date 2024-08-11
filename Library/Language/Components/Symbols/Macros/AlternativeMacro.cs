namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents the alternative macro symbol.
/// </summary>
public interface IAlternativeMacro
{
}

/// <summary>
/// Represents the alternative macro symbol.
/// </summary>
public class AlternativeMacro : MacroSymbol
{
    public AlternativeMacro()
    {

    }

    public override MacroType MacroType => MacroType.Alternative;

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ MacroType.GetHashCode();

            return hash;
        }
    }

    public override bool Equals(object? obj)
    {
        return obj is AlternativeMacro;
    }

    public override bool Equals(Symbol? other)
    {
        return other is AlternativeMacro;
    }

    public override bool Equals(ISymbol? other)
    {
        return other is IAlternativeMacro;
    }

    public override IEnumerable<Sentence> Expand(NonTerminal nonTerminal)
    {
        throw new InvalidOperationException();
    }

    public override IEnumerable<Sentence> Expand(INonTerminal nonTerminal)
    {
        throw new InvalidOperationException();
    }

    public override string ToNotation(NotationType notation)
    {
        throw new InvalidOperationException();
    }

    public override string ToString()
    {
        throw new InvalidOperationException();
    }
}
