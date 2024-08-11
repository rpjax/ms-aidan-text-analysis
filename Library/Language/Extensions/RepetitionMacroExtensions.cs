using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Language.Extensions;

public static class RepetitionMacroExtensions
{
    public static string ToSententialNotation(this RepetitionMacro macro)
    {
        return $"{{ {macro.Sentence.ToNotation(NotationType.Sentential)} }}";
    }

    public static string ToBnfNotation(this RepetitionMacro macro)
    {
        return $"{{ {macro.Sentence.ToNotation(NotationType.Bnf)} }}";
    }

    public static string ToEbnfNotation(this RepetitionMacro macro)
    {
        return $"{{ {macro.Sentence.ToNotation(NotationType.Ebnf)} }}";
    }

    public static string ToEbnfKleeneNotation(this RepetitionMacro macro)
    {
        return $"( {macro.Sentence.ToNotation(NotationType.EbnfKleene)} )*";
    }
}

public static class IRepetitionMacroExtensions
{
    public static string ToSententialNotation(this IRepetitionMacro macro)
    {
        return $"{{ {macro.Sentence.ToNotation(NotationType.Sentential)} }}";
    }

    public static string ToBnfNotation(this IRepetitionMacro macro)
    {
        return $"{{ {macro.Sentence.ToNotation(NotationType.Bnf)} }}";
    }

    public static string ToEbnfNotation(this IRepetitionMacro macro)
    {
        return $"{{ {macro.Sentence.ToNotation(NotationType.Ebnf)} }}";
    }

    public static string ToEbnfKleeneNotation(this IRepetitionMacro macro)
    {
        return $"( {macro.Sentence.ToNotation(NotationType.EbnfKleene)} )*";
    }
}
