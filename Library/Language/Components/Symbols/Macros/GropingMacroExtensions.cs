using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Language.Extensions;

public static class GropingMacroExtensions
{
    public static string ToSententialNotation(this GroupingMacro macro)
    {
        return $"( {macro.Sentence.ToNotation(NotationType.Sentential)} )";
    }

    public static string ToBnfNotation(this GroupingMacro macro)
    {
        return $"( {macro.Sentence.ToNotation(NotationType.Bnf)} )";
    }

    public static string ToEbnfNotation(this GroupingMacro macro)
    {
        return $"( {macro.Sentence.ToNotation(NotationType.Ebnf)} )";
    }

    public static string ToEbnfKleeneNotation(this GroupingMacro macro)
    {
        return $"( {macro.Sentence.ToNotation(NotationType.EbnfKleene)} )";
    }
}

public static class IGropingMacroExtensions
{
    public static string ToSententialNotation(this IGroupingMacro macro)
    {
        return $"( {macro.Sentence.ToNotation(NotationType.Sentential)} )";
    }

    public static string ToBnfNotation(this IGroupingMacro macro)
    {
        return $"( {macro.Sentence.ToNotation(NotationType.Bnf)} )";
    }

    public static string ToEbnfNotation(this IGroupingMacro macro)
    {
        return $"( {macro.Sentence.ToNotation(NotationType.Ebnf)} )";
    }

    public static string ToEbnfKleeneNotation(this IGroupingMacro macro)
    {
        return $"( {macro.Sentence.ToNotation(NotationType.EbnfKleene)} )";
    }
}