using Aidan.TextAnalysis.GDef.Tokenization;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Parsing;
using Aidan.TextAnalysis.Parsing.Components;
using Aidan.TextAnalysis.Parsing.LR1;
using Aidan.TextAnalysis.Parsing.Tools;

namespace Aidan.TextAnalysis.GDef;

/// <summary>
/// Reconizes and parses GDF (Grammar Definition Format) files.
/// </summary>
/// <remarks>
/// GDF files are denoted by '<c>.grammar</c>' file extension. E.g. '<c>myLanguage.grammar</c>'.
/// </remarks>
public static class GDefParser
{
    /// <summary>
    /// Gets or sets the instance of the LR1 parser.
    /// </summary>
    private static LR1Parser? ParserInstance { get; set; }

    /// <summary>
    /// Gets the list of constructs that are allowed to be reduced.
    /// </summary>
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

    /// <summary>
    /// Initializes the parser instance if it is not already initialized.
    /// </summary>
    public static void Init()
    {
        if (ParserInstance is not null)
        {
            return;
        }

        GetParser();
    }

    /// <summary>
    /// Gets the instance of the LR1 parser, initializing it if necessary.
    /// </summary>
    /// <returns>The instance of the LR1 parser.</returns>
    public static LR1Parser GetParser()
    {
        if (ParserInstance is null)
        {
            var grammar = new GDefGrammar();
            var grammarLexer = GDefTokenizers.GrammarTokenizer;

            ParserInstance = new LR1Parser(grammar, grammarLexer);
        }

        return ParserInstance;
    }

    /// <summary>
    /// Parses a GDF file and returns a concrete syntax tree (CST).
    /// </summary>
    /// <param name="text">The text of the GDF file to parse.</param>
    /// <returns>The root node of the CST.</returns>
    public static CstRootNode Parse(string text)
    {
        var parser = GetParser();
        var cst = parser.Parse(text);
        var reducer = new CstReducer(cst, ReduceWhitelist);
        var reducedCst = reducer.Execute();

        return reducedCst;
    }

    /// <summary>
    /// Parses a GDF file and returns a grammar object.
    /// </summary>
    /// <param name="text">The text of the GDF file to parse.</param>
    /// <returns>The parsed grammar object.</returns>
    public static Grammar ParseGrammar(string text)
    {
        var cst = Parse(text);
        var reducer = new CstReducer(cst, ReduceWhitelist);
        var reducedCst = reducer.Execute();

        return GDefTranslator.TranslateGrammar(reducedCst);
    }
}
