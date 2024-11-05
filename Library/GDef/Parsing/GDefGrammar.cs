using Aidan.TextAnalysis.GDef.Tokenization;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Tokenization;

namespace Aidan.TextAnalysis.GDef;

/// <summary>
/// Represents the grammar for the Grammar Definition Format (GDef).
/// </summary>
public class GDefGrammar : Grammar
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GDefGrammar"/> class.
    /// </summary>
    public GDefGrammar() : base(GetStart(), GetProductions())
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
             *      : [ lexer_settings ] production_list
             *      ;
             */
            new ProductionRule(
                new NonTerminal("grammar"),

                new OptionMacro(
                    new NonTerminal("lexer_settings")
                ),
                new NonTerminal("production_list")
            ),

            /* 
             *   lexer_settings
             *      : lexer_statement { lexer_statement }
             *      ;
             */
            new ProductionRule(
                new NonTerminal("lexer_settings"),

                new NonTerminal("lexer_statement"),
                new RepetitionMacro(
                    new NonTerminal("lexer_statement")
                )
            ),

            /* 
             *   lexer_statement
             *      : use charset $id ';'
             *      | lexeme $id = $string ';'
             *      ;
             */
            new ProductionRule(
                new NonTerminal("lexer_statement"),

                new Terminal("use"),
                new Terminal("charset"),
                new Terminal(GDefLexemes.Identifier),
                new Terminal(";"),
                new PipeMacro(), // pipe
                new Terminal("lexeme"),
                new Terminal(GDefLexemes.Identifier),
                new Terminal("="),
                new Terminal(GDefLexemes.String),
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
                new RepetitionMacro(
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
                new Terminal(GDefLexemes.Identifier),
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
                new RepetitionMacro(
                    new NonTerminal("symbol")
                ),
                new OptionMacro(
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

                new Terminal(GDefLexemes.String),
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
                new Terminal(GDefLexemes.Identifier)
            ),

            /* 
             *   macro
             *      : grouping | option | repetition | alternative
             *      ;
             */
            new ProductionRule(
                new NonTerminal("macro"),

                new NonTerminal("grouping"),
                new PipeMacro(),
                new NonTerminal("option"),
                new PipeMacro(),
                new NonTerminal("repetition"),
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
                new RepetitionMacro(
                    new NonTerminal("symbol")
                ),
                new Terminal(")")
            ),

            /* 
             *   option
             *      : '[' symbol { symbol } ']'
             *      ;
             */
            new ProductionRule(
                new NonTerminal("option"),

                new Terminal("["),
                new NonTerminal("symbol"),
                new RepetitionMacro(
                    new NonTerminal("symbol")
                ),
                new Terminal("]")
            ),

            /* 
             *   repetition
             *      : '{' symbol { symbol } '}'
             *      ;
             */
            new ProductionRule(
                new NonTerminal("repetition"),

                new Terminal("{"),
                new NonTerminal("symbol"),
                new RepetitionMacro(
                    new NonTerminal("symbol")
                ),
                new Terminal("}")
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
                new Terminal(GDefLexemes.Identifier)
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
