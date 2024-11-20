using Aidan.TextAnalysis.GDef;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Parsing.LR1;
using Aidan.TextAnalysis.Parsing.LR1.Debug.Grammars;
using Aidan.TextAnalysis.Parsing.Tools;
using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.Tokenization;

namespace Aidan.TextAnalysis.RegularExpressions.Parsing;

public static class RegExprParser
{
    private static Tokenizer? Tokenizer { get; set; }
    private static Grammar? Grammar { get; set; }
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

    private static Tokenizer GetTokenizer()
    {
        if (Tokenizer is null)
        {
            Tokenizer = new RegexTokenizerBuilder(Charset.Compute(CharsetType.ExtendedAscii))
                .Build();
        }

        return Tokenizer;
    }

    private static Grammar GetGrammar()
    {
        if (Grammar is null)
        {
            Grammar = GDefParser.ParseToGrammar(RegexGrammar.RawFile);
        }

        return Grammar;
    }

    private static LR1Parser GetParser()
    {
        if (Parser is null)
        {
            Parser = new LR1Parser(GetGrammar(), GetTokenizer());
        }

        return Parser;
    }

}
