using Aidan.TextAnalysis.GDef.Components;
using Aidan.TextAnalysis.GDef.Tokenization;
using Aidan.TextAnalysis.Parsing.LR1;
using Aidan.TextAnalysis.Parsing.Tools;
using Aidan.TextAnalysis.Parsing.Tree;

namespace Aidan.TextAnalysis.GDef.Parsing;

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
        // lex
        "lexer_settings",
        //"lexer_statement",
        "lexeme_declaration",
        "fragment_declaration",
        "ignored_chars_declaration",
        "lexeme_annotation_list",
        "lexeme_annotation",

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
        "nullable",
        "zero_or_more",
        "one_or_more",
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
        GetParser();
    }

    /// <summary>
    /// Parses the given text and returns a reduced or non-reduced 
    /// concrete syntax tree (CST) based on the specified option.
    /// </summary>
    /// <param name="text">The input text to parse.</param>
    /// <param name="reduce">
    /// A boolean value indicating whether the resulting CST should be reduced.
    /// Defaults to <c>true</c>.
    /// </param>
    /// <returns>
    /// A <see cref="CstRootNode"/> representing the parsed concrete syntax tree.
    /// If <paramref name="reduce"/> is <c>true</c>, the CST is reduced based on
    /// a predefined whitelist.
    /// </returns>
    /// <exception cref="ParseException">
    /// Thrown if the input text cannot be successfully parsed.
    /// </exception>
    public static CstRootNode Parse(string text, bool reduce = true)
    {
        var parser = GetParser();
        var cst = parser.Parse(text);

        if (!reduce)
        {
            return cst;
        }

        var reducer = new CstReducer(cst, ReduceWhitelist);
        var reducedCst = reducer.Execute();

        return reducedCst;
    }

    /// <summary>
    /// Parses a GDF file and returns a grammar object.
    /// </summary>
    /// <param name="text">The text of the GDF file to parse.</param>
    /// <returns>The parsed grammar object.</returns>
    public static GdefGrammar ParseToGrammar(string text)
    {
        var cst = Parse(text);
        var reducer = new CstReducer(cst, ReduceWhitelist);
        var reducedCst = reducer.Execute();
        return GDefTranslator.TranslateGrammar(reducedCst);
    }

    /// <summary>
    /// Gets the instance of the LR1 parser, initializing it if necessary.
    /// </summary>
    /// <returns>The instance of the LR1 parser.</returns>
    private static LR1Parser GetParser()
    {
        if (ParserInstance is null)
        {
            var grammar = new GDefLanguageGrammar();
            var tokenizer = new GDefTokenizerBuilder().Build();
            var ignoredTokenTypes = new string[]
            {
                GDefTokenizerBuilder.Comment
            };

            ParserInstance = new LR1Parser(
                grammar: grammar,
                tokenizer: tokenizer,
                ignoredTokenTypes: ignoredTokenTypes);
        }

        return ParserInstance;
    }
}
