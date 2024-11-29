using Aidan.TextAnalysis.GDef;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Parsing.LR1;
using Aidan.TextAnalysis.Parsing.Tools;
using Aidan.TextAnalysis.RegularExpressions.Tree;

namespace Aidan.TextAnalysis.RegularExpressions.Parsing;

public static class RegExprParser
{
    private static LR1Parser? Parser { get; set; }
    private static string[] ReduceWhitelist { get; } = new string[]
    {
        "regex",
        "union",
        "concatenation",
        "class",
        //"class_child",
        "class_literal",
        "class_range",
        "group",
        "any",
        "quantifier",
        "primary",
    };

    public static RegExpr Parse(string pattern, Charset charset)
    {
        var parser = GetParser(); 
        var cst = parser.Parse(pattern);
        var reducer = new CstReducer(cst, ReduceWhitelist);
        var reducedCst = reducer.Execute();
        var traslator = new RegExprTranslator(charset, fragments: null);
        /* debug */
        //var html = reducedCst.ToHtmlTreeView();
        return traslator.Translate(reducedCst);
    }

    private static LR1Parser GetParser()
    {
        if (Parser is null)
        {
            var grammar = GDefParser.ParseToGrammar(RegexGrammar.RawFile);
            var tokenizer = new RegexTokenizerBuilder().Build();

            Parser = new LR1Parser(grammar, tokenizer);
        }

        return Parser;
    }

}
