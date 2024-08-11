using Aidan.TextAnalysis.Language.Extensions;

namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents an expanded alternative macro. It is used to represent a set of alternatives in a grammar.
/// </summary>
public interface IExpandedAlternativeMacro : IMacroSymbol
{
    /// <summary>
    /// Gets the alternatives of the expanded alternative macro.
    /// </summary>
    Sentence[] Alternatives { get; }
}

/// <summary>
/// Represents an expanded alternative macro. It is used to represent a set of alternatives in a grammar.
/// </summary>
public class ExpandedAlternativeMacro : MacroSymbol, IExpandedAlternativeMacro
{
    /// <inheritdoc/>
    public Sentence[] Alternatives { get; }

    public ExpandedAlternativeMacro(params Sentence[] alternatives)
    {
        Alternatives = alternatives;

        if (alternatives.Length == 0)
        {
            throw new ArgumentException("The alternation macro must contain at least one alternative.");
        }
    }

    /// <inheritdoc/>
    public override MacroType MacroType => MacroType.ExpandedAlternative;

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            foreach (var alternative in Alternatives)
            {
                hash = (hash * 16777619) ^ alternative.GetHashCode();
            }

            return hash;
        }
    }

    public override bool Equals(object? obj)
    {
        return obj is ExpandedAlternativeMacro macro
            && macro.Alternatives.SequenceEqual(Alternatives);
    }

    public override bool Equals(Symbol? other)
    {
        return other is ExpandedAlternativeMacro macro
            && macro.Alternatives.SequenceEqual(Alternatives);
    }

    public override bool Equals(ISymbol? other)
    {
        return other is IExpandedAlternativeMacro macro
            && macro.Alternatives.SequenceEqual(Alternatives);
    }

    public override IEnumerable<Sentence> Expand(NonTerminal nonTerminal)
    {
        return Alternatives;
    }

    public override IEnumerable<Sentence> Expand(INonTerminal nonTerminal)
    {
        return Alternatives;
    }

    public override string ToNotation(NotationType notation)
    {
        switch (notation)
        {
            case NotationType.Sentential:
                return this.ToSententialNotation();

            case NotationType.Bnf:
                return this.ToBnfNotation();

            case NotationType.Ebnf:
                return this.ToEbnfNotation();

            case NotationType.EbnfKleene:
                return this.ToEbnfKleeneNotation();
        }

        throw new InvalidOperationException("Invalid notation type.");
    }

    public override string ToString()
    {
        return ToNotation(NotationType.Sentential);
    }

}
