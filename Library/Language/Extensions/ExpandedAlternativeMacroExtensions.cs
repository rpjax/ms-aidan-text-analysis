using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Language.Extensions;

public static class ExpandedAlternativeMacroExtensions
{
    public static string ToSententialNotation(this ExpandedAlternativeMacro macro)
    {
        return string.Join(" | ", macro.Alternatives.Select(x => x.ToNotation(NotationType.Sentential)));
    }

    public static string ToBnfNotation(this ExpandedAlternativeMacro macro)
    {
        return string.Join(" | ", macro.Alternatives.Select(x => x.ToNotation(NotationType.Bnf)));
    }

    public static string ToEbnfNotation(this ExpandedAlternativeMacro macro)
    {
        return string.Join(" | ", macro.Alternatives.Select(x => x.ToNotation(NotationType.Ebnf)));
    }

    public static string ToEbnfKleeneNotation(this ExpandedAlternativeMacro macro)
    {
        return string.Join(" | ", macro.Alternatives.Select(x => x.ToNotation(NotationType.EbnfKleene)));
    }
}

public static class IExpandedAlternativeMacroExtensions
{
    public static string ToSententialNotation(this IExpandedAlternativeMacro macro)
    {
        return string.Join(" | ", macro.Alternatives.Select(x => x.ToNotation(NotationType.Sentential)));
    }

    public static string ToBnfNotation(this IExpandedAlternativeMacro macro)
    {
        return string.Join(" | ", macro.Alternatives.Select(x => x.ToNotation(NotationType.Bnf)));
    }

    public static string ToEbnfNotation(this IExpandedAlternativeMacro macro)
    {
        return string.Join(" | ", macro.Alternatives.Select(x => x.ToNotation(NotationType.Ebnf)));
    }

    public static string ToEbnfKleeneNotation(this IExpandedAlternativeMacro macro)
    {
        return string.Join(" | ", macro.Alternatives.Select(x => x.ToNotation(NotationType.EbnfKleene)));
    }
}
