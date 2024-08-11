using Aidan.TextAnalysis.Language.Extensions;

namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Represents a repetition macro. It is analoguous to EBNF's "{ }" operator.
/// </summary>
public interface IRepetitionMacro : ISentenceMacro
{
}

/// <summary>
/// Represents a repetition macro. It is analoguous to EBNF's "{ }" operator.
/// </summary>
public class RepetitionMacro : SentenceMacro, IRepetitionMacro
{
    public override MacroType MacroType => MacroType.Repetition;

    public RepetitionMacro(params Symbol[] symbols) : base(symbols)
    {
    }

    public override IEnumerable<Sentence> Expand(NonTerminal nonTerminal)
    {
        yield return Sentence.Add(nonTerminal);
        yield return new Sentence(new Epsilon());
    }

    public override IEnumerable<Sentence> Expand(INonTerminal nonTerminal)
    {
        yield return Sentence.Add((NonTerminal)nonTerminal);
        yield return new Sentence(new Epsilon());
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
