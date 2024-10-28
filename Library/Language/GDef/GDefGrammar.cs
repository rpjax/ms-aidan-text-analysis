//using Aidan.TextAnalysis.Language.Components;
//using Aidan.TextAnalysis.Tokenization;

//namespace Aidan.TextAnalysis.GDef;

///// <summary>
///// Represents the grammar for the Grammar Definition Format (GDef).
///// </summary>
//public class GDefGrammar : Grammar
//{
//    /// <summary>
//    /// Initializes a new instance of the <see cref="GDefGrammar"/> class.
//    /// </summary>
//    public GDefGrammar() : base(GetStart(), GetProductions())
//    {
//    }

//    /// <summary>
//    /// Gets the start non-terminal symbol for the grammar.
//    /// </summary>
//    /// <returns>The start non-terminal symbol.</returns>
//    private static NonTerminal GetStart()
//    {
//        return new NonTerminal("start");
//    }

//    /// <summary>
//    /// Gets the production rules for the grammar.
//    /// </summary>
//    /// <returns>An array of production rules.</returns>
//    private static ProductionRule[] GetProductions()
//    {
//        return new ProductionRule[]
//        {
//            new ProductionRule(
//                new NonTerminal("start"),
//                new NonTerminal("grammar")
//            ),

//            new ProductionRule(
//                new NonTerminal("grammar"),
//                new OptionMacro(
//                    new NonTerminal("lexer_settings")
//                ),
//                new NonTerminal("production_list")
//            ),

//            new ProductionRule(
//                new NonTerminal("lexer_settings"),
//                new Terminal("<"),
//                new Terminal("lexer"),
//                new Terminal(">"),

//                new RepetitionMacro(
//                    new NonTerminal("lexer_statement")
//                ),

//                new Terminal("<"),
//                new Terminal("/"),
//                new Terminal("lexer"),
//                new Terminal(">")
//            ),

//            new ProductionRule(
//                new NonTerminal("lexer_statement"),
//                new Terminal("use"),
//                new Terminal(TokenType.Identifier),
//                new Terminal(";"),
//                new AlternativeMacro(),
//                new Terminal("lexeme"),
//                new Terminal(TokenType.Identifier),
//                new NonTerminal("regex"),
//                new Terminal(";")
//            ),

//            new ProductionRule(
//                new NonTerminal("regex"),
//                new Terminal("regex")
//            ),

//            new ProductionRule(
//                new NonTerminal("production_list"),
//                new NonTerminal("production"),
//                new RepetitionMacro(
//                    new NonTerminal("production")
//                )
//            ),

//            new ProductionRule(
//                new NonTerminal("production"),
//                new Terminal(TokenType.Identifier),
//                new Terminal(":"),
//                new NonTerminal("production_body"),
//                new Terminal(";")
//            ),

//            new ProductionRule(
//                new NonTerminal("production_body"),
//                new NonTerminal("symbol"),
//                new RepetitionMacro(
//                    new NonTerminal("symbol")
//                ),
//                new OptionMacro(
//                    new NonTerminal("semantic_action")
//                )
//            ),

//            new ProductionRule(
//                new NonTerminal("symbol"),
//                new NonTerminal("terminal"),
//                new AlternativeMacro(),
//                new NonTerminal("non_terminal"),
//                new AlternativeMacro(),
//                new NonTerminal("macro")
//            ),

//            new ProductionRule(
//                new NonTerminal("terminal"),
//                new Terminal(TokenType.String),
//                new AlternativeMacro(),
//                new NonTerminal("lexeme"),
//                new AlternativeMacro(),
//                new NonTerminal("epsilon")
//            ),

//            new ProductionRule(
//                new NonTerminal("non_terminal"),
//                new Terminal(TokenType.Identifier)
//            ),

//            new ProductionRule(
//                new NonTerminal("epsilon"),
//                new Terminal("ε")
//            ),

//            new ProductionRule(
//                new NonTerminal("macro"),
//                new NonTerminal("grouping"),
//                new AlternativeMacro(),
//                new NonTerminal("option"),
//                new AlternativeMacro(),
//                new NonTerminal("repetition"),
//                new AlternativeMacro(),
//                new NonTerminal("alternative")
//            ),

//            new ProductionRule(
//                new NonTerminal("grouping"),
//                new Terminal("("),
//                new NonTerminal("symbol"),
//                new RepetitionMacro(
//                    new NonTerminal("symbol")
//                ),
//                new Terminal(")")
//            ),

//            new ProductionRule(
//                new NonTerminal("option"),
//                new Terminal("["),
//                new NonTerminal("symbol"),
//                new RepetitionMacro(
//                    new NonTerminal("symbol")
//                ),
//                new Terminal("]")
//            ),

//            new ProductionRule(
//                new NonTerminal("repetition"),
//                new Terminal("{"),
//                new NonTerminal("symbol"),
//                new RepetitionMacro(
//                    new NonTerminal("symbol")
//                ),
//                new Terminal("}")
//            ),

//            new ProductionRule(
//                new NonTerminal("alternative"),
//                new Terminal("|")
//            ),

//            new ProductionRule(
//                new NonTerminal("lexeme"),
//                new Terminal("$"),
//                new Terminal(TokenType.Identifier)
//            ),

//            /*
//             * Semantic Actions
//             */

//            //* semantic_action
//            new ProductionRule(
//                new NonTerminal("semantic_action"),
//                new Terminal("="),
//                new Terminal(">"),
//                new NonTerminal("action_block")
//            ),

//            //* action_block
//            new ProductionRule(
//                new NonTerminal("action_block"),
//                new Terminal("{"),
//                new NonTerminal("semantic_statement"),
//                new RepetitionMacro(
//                    new Terminal(","),
//                    new NonTerminal("semantic_statement")
//                ),
//                new Terminal("}")
//            ),

//            //* semantic_statement
//            new ProductionRule(
//                new NonTerminal("semantic_statement"),
//                new NonTerminal("reduction")
//            ),
//            new ProductionRule(
//                new NonTerminal("semantic_statement"),
//                new NonTerminal("assignment")
//            ),

//            //* reduction
//            new ProductionRule(
//                new NonTerminal("reduction"),
//                new Terminal("$"),
//                new Terminal(":"),
//                new NonTerminal("expression")
//            ),

//            //* assignment
//            new ProductionRule(
//                new NonTerminal("assignment"),
//                new Terminal(TokenType.Identifier),
//                new Terminal(":"),
//                new NonTerminal("expression")
//            ),

//            //* expression
//            new ProductionRule(
//                new NonTerminal("expression"),
//                new NonTerminal("literal")
//            ),
//            new ProductionRule(
//                new NonTerminal("expression"),
//                new NonTerminal("reference")
//            ),
//            new ProductionRule(
//                new NonTerminal("expression"),
//                new NonTerminal("index_expression")
//            ),
//            new ProductionRule(
//                new NonTerminal("expression"),
//                new NonTerminal("function_call")
//            ),
//            new ProductionRule(
//                new NonTerminal("expression"),
//                new NonTerminal("expression"),
//                new Terminal("."),
//                new NonTerminal("function_call")
//            ),

//            //* literal
//            new ProductionRule(
//                new NonTerminal("literal"),
//                new Terminal(TokenType.String)
//            ),
//            new ProductionRule(
//                new NonTerminal("literal"),
//                new Terminal(TokenType.Integer)
//            ),
//            new ProductionRule(
//                new NonTerminal("literal"),
//                new Terminal(TokenType.Float)
//            ),

//            //* reference
//            new ProductionRule(
//                new NonTerminal("reference"),
//                new Terminal(TokenType.Identifier)
//            ),

//            //* index_expression
//            new ProductionRule(
//                new NonTerminal("index_expression"),
//                new Terminal("["),
//                new Terminal(TokenType.Integer),
//                new Terminal("]")
//            ),

//            //* function_call
//            new ProductionRule(
//                new NonTerminal("function_call"),
//                new Terminal(TokenType.Identifier),
//                new Terminal("("),
//                new OptionMacro(
//                    new NonTerminal("parameter_list")
//                ),
//                new Terminal(")")
//            ),

//            //* parameter_list
//            new ProductionRule(
//                new NonTerminal("parameter_list"),
//                new NonTerminal("parameter"),
//                new RepetitionMacro(
//                    new Terminal(","),
//                    new NonTerminal("parameter")
//                )
//            ),

//            //* parameter
//            new ProductionRule(
//                new NonTerminal("parameter"),
//                new Terminal(TokenType.Identifier)
//            ),
//        };
//    }
//}
