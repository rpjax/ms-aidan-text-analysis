using Aidan.TextAnalysis.GDef.Tokenization;
using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.GDef;

/// <summary>
/// Represents the grammar for the Grammar Definition Format (GDef).
/// </summary>
public class GDefLanguageGrammar : Grammar
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GDefLanguageGrammar"/> class.
    /// </summary>
    public GDefLanguageGrammar() : base(GetStart(), GetProductions())
    {
    }

    /// <summary>
    /// Gets the start non-terminal symbol for the grammar.
    /// </summary>
    /// <returns>The start non-terminal symbol.</returns>
    private static NonTerminal GetStart()
    {
        return new NonTerminal("start");
    }

    /// <summary>
    /// Gets the production rules for the grammar.
    /// </summary>
    /// <returns>An array of production rules.</returns>
    private static ProductionRule[] GetProductions()
    {
        return new ProductionRule[]
        {
            /* 
             *   start
             *      : grammar
             *      ;
             */
            new ProductionRule(
                new NonTerminal("start"),

                new NonTerminal("grammar")
            ),

            /* 
             *   grammar
             *      : lexer_settings? production_list
             *      ;
             */
            new ProductionRule(
                new NonTerminal("grammar"),

                new NullableMacro(
                    new NonTerminal("lexer_settings")
                ),
                new NonTerminal("production_list")
            ),

            /* 
             *   lexer_settings
             *      : lexer_statement+
             *      ;
             */
            new ProductionRule(
                new NonTerminal("lexer_settings"),

                new OneOrMoreMacro(
                    new NonTerminal("lexer_statement")
                )
            ),

            /* 
             *   lexer_statement
             *      : lexeme_declaration
             *      | fragment_declaration
             *      | ignored_chars_declaration
             *      ;
             */ 

            new ProductionRule(
                new NonTerminal("lexer_statement"),

                new NonTerminal("lexeme_declaration"),
                new PipeMacro(),
                new NonTerminal("fragment_declaration"),
                new PipeMacro(),
                new NonTerminal("ignored_chars_declaration")
            ),

            /*
             *   lexeme_declaration
             *      : lexeme_annotation_list? 'lexeme' $id '=' $string ';'
             *      ;
             */
            new ProductionRule(
                new NonTerminal("lexeme_declaration"),

                new NullableMacro(
                    new NonTerminal("lexeme_annotation_list")
                ),
                new Terminal(GDefTokenizerBuilder.LexemeKeyword),
                new Terminal(GDefTokenizerBuilder.Identifier),
                new Terminal("="),
                new Terminal(GDefTokenizerBuilder.String),
                new Terminal(";")
            ),

            /* 
             *   lexeme_annotation_list
             *      : '[' lexeme_annotation (',' lexeme_annotation)* ']'
             *      ;
             */
            new ProductionRule(
                new NonTerminal("lexeme_annotation_list"),

                new Terminal("["),
                new NonTerminal("lexeme_annotation"),
                new ZeroOrMoreMacro(
                    new Terminal(","),
                    new NonTerminal("lexeme_annotation")
                ),
                new Terminal("]")
            ),

            /* 
             *   lexeme_annotation
             *      : 'charset' ':' $string
             *      | 'ignore' ':' ('true' | 'false')
             *      ;
             */
            new ProductionRule(
                new NonTerminal("lexeme_annotation"),

                new Terminal(GDefTokenizerBuilder.CharsetKeyword),
                new Terminal(":"),
                new Terminal(GDefTokenizerBuilder.String),

                new PipeMacro(),

                new Terminal(GDefTokenizerBuilder.IgnoreKeyword),
                new Terminal(":"),
                new AlternativeMacro(
                    new Sentence(new Terminal(GDefTokenizerBuilder.TrueKeyword)),
                    new Sentence(new Terminal(GDefTokenizerBuilder.FalseKeyword))
                )
            ),

            /*
             *   fragment_declaration
             *      :  'fragment' $id '=' $string ';'
             *      ;
             */
            new ProductionRule(
                new NonTerminal("fragment_declaration"),

                new Terminal(GDefTokenizerBuilder.FragmentKeyword),
                new Terminal(GDefTokenizerBuilder.Identifier),
                new Terminal("="),
                new Terminal(GDefTokenizerBuilder.String),
                new Terminal(";")
            ),

            /*
             *   ignored_chars_declaration
             *      :  'ignored-chars' '=' $string ';'
             *      ;
             */
            new ProductionRule(
                new NonTerminal("ignored_chars_declaration"),

                new Terminal(GDefTokenizerBuilder.IgnoredCharsKeyword),
                new Terminal("="),
                new Terminal(GDefTokenizerBuilder.String),
                new Terminal(";")
            ),

            /* 
             *   production_list
             *      : production { production }
             *      ;
             */
            new ProductionRule(
                new NonTerminal("production_list"),
                new NonTerminal("production"),
                new ZeroOrMoreMacro(
                    new NonTerminal("production")
                )
            ),

            /* 
             *   production
             *      : $id ':' production_body ';'
             *      ;
             */
            new ProductionRule(
                new NonTerminal("production"),
                new Terminal(GDefTokenizerBuilder.Identifier),
                new Terminal(":"),
                new NonTerminal("production_body"),
                new Terminal(";")
            ),

            /* 
             *   production_body
             *      : symbol { symbol } [ semantic_action ]
             *      ;
             */
            new ProductionRule(
                new NonTerminal("production_body"),
                new NonTerminal("symbol"),
                new ZeroOrMoreMacro(
                    new NonTerminal("symbol")
                ),
                new NullableMacro(
                    new NonTerminal("semantic_action")
                )
            ),

            /* 
             *   symbol
             *      : terminal | non_terminal | macro
             *      ;
             */
            new ProductionRule(
                new NonTerminal("symbol"),
                new NonTerminal("terminal"),
                new PipeMacro(),
                new NonTerminal("non_terminal"),
                new PipeMacro(),
                new NonTerminal("macro")
            ),

            /* 
             *   terminal
             *      : $string | lexeme 
             *      ;
             */
            new ProductionRule(
                new NonTerminal("terminal"),

                new Terminal(GDefTokenizerBuilder.String),
                new PipeMacro(),
                new NonTerminal("lexeme_reference")
            ),

            /* 
             *   non_terminal
             *      : $id 
             *      ;
             */
            new ProductionRule(
                new NonTerminal("non_terminal"),
                new Terminal(GDefTokenizerBuilder.Identifier)
            ),

            /* 
             *   macro
             *      : grouping | nullable | zero_or_more | one_or_more | alternative
             *      ;
             */
            new ProductionRule(
                new NonTerminal("macro"),

                new NonTerminal("grouping"),
                new PipeMacro(),
                new NonTerminal("nullable"),
                new PipeMacro(),
                new NonTerminal("zero_or_more"),
                new PipeMacro(),
                new NonTerminal("one_or_more"),
                new PipeMacro(),
                new NonTerminal("alternative")
            ),

            /* 
             *   grouping
             *      : '(' symbol { symbol } ')'
             *      ;
             */
            new ProductionRule(
                new NonTerminal("grouping"),

                new Terminal("("),
                new NonTerminal("symbol"),
                new ZeroOrMoreMacro(
                    new NonTerminal("symbol")
                ),
                new Terminal(")")
            ),

            /* 
             *   nullable
             *      : symbol '?'
             *      ;
             */
            new ProductionRule(
                new NonTerminal("nullable"),

                new NonTerminal("symbol"),
                new Terminal("?")
            ),

            /* 
             *   zero_or_more
             *      : symbol '*'
             *      ;
             */
            new ProductionRule(
                new NonTerminal("zero_or_more"),

                new NonTerminal("symbol"),
                new Terminal("*")
            ),

            /* 
             *   one_or_more
             *      : symbol '+'
             *      ;
             */
            new ProductionRule(
                new NonTerminal("one_or_more"),

                new NonTerminal("symbol"),
                new Terminal("+")
            ),

            /* 
             *   alternative
             *      : '|'
             *      ;
             */
            new ProductionRule(
                new NonTerminal("alternative"),
                new Terminal("|")
            ),

            /* 
             *   lexeme
             *      : '$' $id
             *      ;
             */
            new ProductionRule(
                new NonTerminal("lexeme_reference"),

                new Terminal("$"),
                new Terminal(GDefTokenizerBuilder.Identifier)
            ),

            /*
             * Semantic Actions
             */

            /* 
             *   semantic_action
             *      : '=' '>' placeholder
             *      ;
             */
            new ProductionRule(
                new NonTerminal("semantic_action"),
                new Terminal("="),
                new Terminal(">"),
                new Terminal("placeholder")
            ),

        };
    }
}

/*
 foo
    : a b 
    | c d (foo | bar)?
 */