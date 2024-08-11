namespace Aidan.TextAnalysis.Language.Components;

public interface ISentenceMacro : IMacroSymbol
{
    Sentence Sentence { get; }
}

public abstract class SentenceMacro : MacroSymbol, ISentenceMacro
{
    public Sentence Sentence { get; }

    public SentenceMacro(Sentence sentence)
    {
        Sentence = sentence;

        if (Sentence.Length == 0)
        {
            throw new ArgumentException("The production macro must contain at least one symbol.");
        }
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            foreach (var symbol in Sentence)
            {
                hash = (hash * 16777619) ^ symbol.GetHashCode();
            }

            return hash;
        }
    }

    public override bool Equals(object? obj)
    {
        return obj is SentenceMacro macro
            && macro.Sentence.SequenceEqual(Sentence);
    }

    public override bool Equals(Symbol? other)
    {
        return other is SentenceMacro macro
            && macro.Sentence.SequenceEqual(Sentence);
    }

    public override bool Equals(ISymbol? other)
    {
        return other is SentenceMacro macro
            && macro.MacroType == MacroType
            && macro.Sentence.SequenceEqual(Sentence);
    }

}
