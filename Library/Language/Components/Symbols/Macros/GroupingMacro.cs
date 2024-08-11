using Aidan.TextAnalysis.Language.Extensions;

namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents a grouping macro. 
/// </summary>
public interface IGroupingMacro : ISentenceMacro
{
}

/// <summary>
/// Represents a grouping macro. 
/// </summary>
public class GroupingMacro : SentenceMacro, IGroupingMacro
{
    public GroupingMacro(params Symbol[] symbols) : base(symbols)
    {
    }

    public override MacroType MacroType => MacroType.Grouping;

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ MacroType.GetHashCode();
            hash = (hash * 16777619) ^ Sentence.GetHashCode();

            return hash;
        }
    }

    public override bool Equals(object? obj)
    {
        return obj is GroupingMacro macro 
            && Sentence.Equals(macro.Sentence);
    }

    public override bool Equals(Symbol? other)
    {
        return other is GroupingMacro macro 
            && Sentence.Equals(macro.Sentence);
    }

    public override bool Equals(ISymbol? other)
    {
        return other is IGroupingMacro macro
            && Sentence.Equals(macro.Sentence);
    }

    public override IEnumerable<Sentence> Expand(NonTerminal nonTerminal)
    {
        yield return Sentence;
    }

    public override IEnumerable<Sentence> Expand(INonTerminal nonTerminal)
    {
        yield return Sentence;
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
