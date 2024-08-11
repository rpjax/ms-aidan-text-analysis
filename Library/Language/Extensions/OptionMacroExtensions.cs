using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Language.Extensions;

public static class OptionMacroExtensions
{
    public static string ToSententialNotation(this OptionMacro macro)
    {
        return $"[ {macro.Sentence.ToNotation(NotationType.Sentential)} ]";
    }

    public static string ToBnfNotation(this OptionMacro macro)
    {
        return $"[ {macro.Sentence.ToNotation(NotationType.Bnf)} ]";
    }

    public static string ToEbnfNotation(this OptionMacro macro)
    {
        return $"[ {macro.Sentence.ToNotation(NotationType.Ebnf)} ]";
    }

    public static string ToEbnfKleeneNotation(this OptionMacro macro)
    {
        return $"( {macro.Sentence.ToNotation(NotationType.EbnfKleene)} )?";
    }
}

public static class IOptionMacroExtensions
{
    public static string ToSententialNotation(this IOptionMacro macro)
    {
        return $"[ {macro.Sentence.ToNotation(NotationType.Sentential)} ]";
    }

    public static string ToBnfNotation(this IOptionMacro macro)
    {
        return $"[ {macro.Sentence.ToNotation(NotationType.Bnf)} ]";
    }

    public static string ToEbnfNotation(this IOptionMacro macro)
    {
        return $"[ {macro.Sentence.ToNotation(NotationType.Ebnf)} ]";
    }

    public static string ToEbnfKleeneNotation(this IOptionMacro macro)
    {
        return $"( {macro.Sentence.ToNotation(NotationType.EbnfKleene)} )?";
    }
}

