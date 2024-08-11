namespace Aidan.TextAnalysis.Grammars;

public static class CanonicalGrammars
{
    public static GdefGrammar Gdef { get; } = new GdefGrammar();
    public static JsonGrammar Json { get; } = new JsonGrammar();
}