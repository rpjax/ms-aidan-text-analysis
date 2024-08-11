using Aidan.TextAnalysis.Grammars;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Parsing;
using Aidan.TextAnalysis.Parsing.Components;
using Aidan.TextAnalysis.Parsing.Tools;

namespace Aidan.TextAnalysis.Gdef;

/// <summary>
/// Reconizes and parses gdef files. (Grammar Definition Format)
/// </summary>
public static class GdefParser
{
    private static LR1Parser? ParserInstance { get; set; } 

    private static string[] ReduceWhitelist { get; } = new string[]
    {
        // high order constructs
        "production",

        /*
         * symbols
         */
        "terminal",
        "non_terminal",
        "macro",

        // terminal constructs
        "lexeme",
        "epsilon",

        // non-terminal constructs
        "grouping",
        "option",
        "repetition",
        "alternative",

        // semantic stuff, not working right now
        "semantic_action",
        "semantic_value",
        "semantic_statement",
        "reduction",
        "assignment",
        "expression",
        "literal",
        "reference",
        "index_expression",
        "function_call",
        "parameter"
    };

    public static void Init()
    {
        if (ParserInstance is not null)
        {
            return;
        }

        GetParser();
    }

    public static LR1Parser GetParser()
    {
        if (ParserInstance is null)
        {
            ParserInstance = new LR1Parser(new GdefGrammar());
        }

        return ParserInstance;
    }

    /// <summary>
    /// Parses a gdef file and returns a CST.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static CstRootNode Parse(string text)
    {
        return GetParser().Parse(text);
    }

    /// <summary>
    /// Parses a gdef file and returns a grammar object.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Grammar ParseGrammar(string text)
    {
        var cst = Parse(text);
        var reducer = new CstReducer(cst, ReduceWhitelist);
        var reducedCst = reducer.Execute();

        return GdefTranslator.TranslateGrammar(reducedCst);
    }

}
