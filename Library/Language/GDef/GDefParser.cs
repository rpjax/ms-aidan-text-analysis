//using Aidan.TextAnalysis.Language.Components;
//using Aidan.TextAnalysis.Parsing;
//using Aidan.TextAnalysis.Parsing.Components;
//using Aidan.TextAnalysis.Parsing.LR1;
//using Aidan.TextAnalysis.Parsing.Tools;

//namespace Aidan.TextAnalysis.GDef;

///// <summary>
///// Reconizes and parses GDF (Grammar Definition Format)files. 
///// </summary>
///// <remarks>
///// GDF files are denoted by '<c>.grammar</c>' file extension. E.g. '<c>myLanguage.grammar</c>'.
///// </remarks>
//public static class GDefParser
//{
//    private static LR1Parser? ParserInstance { get; set; } 

//    private static string[] ReduceWhitelist { get; } = new string[]
//    {
//        // high order constructs
//        "production",

//        /*
//         * symbols
//         */
//        "terminal",
//        "non_terminal",
//        "macro",

//        // terminal constructs
//        "lexeme",
//        "epsilon",

//        // non-terminal constructs
//        "grouping",
//        "option",
//        "repetition",
//        "alternative",

//        // semantic stuff, not working right now
//        "semantic_action",
//        "semantic_value",
//        "semantic_statement",
//        "reduction",
//        "assignment",
//        "expression",
//        "literal",
//        "reference",
//        "index_expression",
//        "function_call",
//        "parameter"
//    };

//    public static void Init()
//    {
//        if (ParserInstance is not null)
//        {
//            return;
//        }

//        GetParser();
//    }

//    public static LR1Parser GetParser()
//    {
//        if (ParserInstance is null)
//        {
//            ParserInstance = new LR1Parser(new GDefGrammar());
//        }

//        return ParserInstance;
//    }

//    /// <summary>
//    /// Parses a gdef file and returns a CST.
//    /// </summary>
//    /// <param name="text"></param>
//    /// <returns></returns>
//    public static CstRootNode Parse(string text)
//    {
//        return GetParser().Parse(text);
//    }

//    /// <summary>
//    /// Parses a gdef file and returns a grammar object.
//    /// </summary>
//    /// <param name="text"></param>
//    /// <returns></returns>
//    public static Grammar ParseGrammar(string text)
//    {
//        var cst = Parse(text);
//        var reducer = new CstReducer(cst, ReduceWhitelist);
//        var reducedCst = reducer.Execute();

//        return GDefTranslator.TranslateGrammar(reducedCst);
//    }

//}
