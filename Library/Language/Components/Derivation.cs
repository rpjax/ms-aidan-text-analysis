using Aidan.TextAnalysis.Language.Extensions;

namespace Aidan.TextAnalysis.Language.Components;

public class Derivation
{
    public ProductionRule Production { get; }
    public NonTerminal NonTerminal { get; }
    public Sentence OriginalSentence { get; }
    public Sentence DerivedSentence { get; }
    public int NonTerminalPosition { get; }

    public Derivation(
        ProductionRule production,
        NonTerminal nonTerminal,
        Sentence originalSentence,
        Sentence derivedSentence)
    {
        Production = production;
        NonTerminal = nonTerminal;
        OriginalSentence = originalSentence;
        DerivedSentence = derivedSentence;
        NonTerminalPosition = originalSentence.IndexOfSymbol(nonTerminal);

        if(NonTerminalPosition == -1)
        {
            throw new InvalidOperationException("The non-terminal does not exist in the original sentence.");
        }
        if (NonTerminalPosition >= OriginalSentence.Length)
        {
            throw new InvalidOperationException("The non-terminal position is out of bounds.");
        }
        if (OriginalSentence[NonTerminalPosition] != production.Head)
        {
            throw new InvalidOperationException("The non-terminal at the specified position does not match the head of the production rule.");
        }
    }

    public override string ToString()
    {
        return $"Derived ({NonTerminal}) from ({OriginalSentence}) using ({Production}) yielding ({DerivedSentence})";
    }

}

