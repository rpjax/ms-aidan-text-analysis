using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Tokenization;

namespace Aidan.TextAnalysis.Grammars;

public class GdefGrammar : Grammar
{
    public GdefGrammar() : base(GetStart(), GetProductions())
    {
    }

    private static NonTerminal GetStart()
    {
        return new NonTerminal("start");
    }

    private static ProductionRule[] GetProductions()
    {
        return new ProductionRule[]
        {
            new ProductionRule(
                "start",
                new NonTerminal("grammar")
            ),

            new ProductionRule(
                "grammar",
                new OptionMacro(
                    new NonTerminal("lexer_settings")
                ),
                new NonTerminal("production_list")
            ),

            new ProductionRule(
                "lexer_settings",
                new Terminal(TokenType.Punctuation, "<"),
                new Terminal(TokenType.Identifier, "lexer"),
                new Terminal(TokenType.Punctuation, ">"),
                    
                new RepetitionMacro(
                    new NonTerminal("lexer_statement")
                ),

                new Terminal(TokenType.Punctuation, "<"),
                new Terminal(TokenType.Punctuation, "/"),
                new Terminal(TokenType.Identifier, "lexer"),
                new Terminal(TokenType.Punctuation, ">")
            ),

            new ProductionRule(
                "lexer_statement",
                new Terminal(TokenType.Identifier, "use"),
                new Terminal(TokenType.Identifier),
                new Terminal(TokenType.Punctuation, ";"),
                new AlternativeMacro(),
                new Terminal(TokenType.Identifier, "lexeme"),
                new Terminal(TokenType.Identifier),
                new NonTerminal("regex"),
                new Terminal(TokenType.Punctuation, ";")
            ),

            new ProductionRule(
                "regex",
                new Terminal(TokenType.String)
            ),

            new ProductionRule(
                "production_list",
                new NonTerminal("production"),
                new RepetitionMacro(
                    new NonTerminal("production")
                )
            ),

            new ProductionRule(
                "production",
                new Terminal(TokenType.Identifier),
                new Terminal(TokenType.Punctuation, ":"),
                new NonTerminal("production_body"),
                new Terminal(TokenType.Punctuation, ";")
            ),

            new ProductionRule(
                "production_body",
                new NonTerminal("symbol"),
                new RepetitionMacro(
                    new NonTerminal("symbol")
                ),
                new OptionMacro(
                    new NonTerminal("semantic_action")
                )
            ),

            new ProductionRule(
                "symbol",
                new NonTerminal("terminal"),
                new AlternativeMacro(),
                new NonTerminal("non_terminal"),
                new AlternativeMacro(),
                new NonTerminal("macro")
            ),

            new ProductionRule(
                "terminal",
                new Terminal(TokenType.String),
                new AlternativeMacro(),
                new NonTerminal("lexeme"),
                new AlternativeMacro(),
                new NonTerminal("epsilon")
            ),

            new ProductionRule(
                "non_terminal",
                new Terminal(TokenType.Identifier)
            ),

            new ProductionRule(
                "epsilon",
                new Terminal(TokenType.Identifier, "ε")
            ),

            new ProductionRule(
                "macro",
                new NonTerminal("grouping"),
                new AlternativeMacro(),
                new NonTerminal("option"),
                new AlternativeMacro(),
                new NonTerminal("repetition"),
                new AlternativeMacro(),
                new NonTerminal("alternative")
            ),

            new ProductionRule(
                "grouping",
                new Terminal(TokenType.Punctuation, "("),
                new NonTerminal("symbol"),
                new RepetitionMacro(
                    new NonTerminal("symbol")
                ),
                new Terminal(TokenType.Punctuation, ")")
            ),

            new ProductionRule(
                "option",
                new Terminal(TokenType.Punctuation, "["),
                new NonTerminal("symbol"),
                new RepetitionMacro(
                    new NonTerminal("symbol")
                ),
                new Terminal(TokenType.Punctuation, "]")
            ),

            new ProductionRule(
                "repetition",
                new Terminal(TokenType.Punctuation, "{"),
                new NonTerminal("symbol"),
                new RepetitionMacro(
                    new NonTerminal("symbol")
                ),
                new Terminal(TokenType.Punctuation, "}")
            ),

            new ProductionRule(
                "alternative",
                new Terminal(TokenType.Punctuation, "|")
            ),

            new ProductionRule(
                "lexeme",
                new Terminal(TokenType.Punctuation, "$"),
                new Terminal(TokenType.Identifier)
            ),

            /*
             * Semantic Actions
             */

            //* semantic_action
            new ProductionRule(
                "semantic_action",
                new Terminal(TokenType.Punctuation, "="),
                new Terminal(TokenType.Punctuation, ">"),
                new NonTerminal("action_block")
            ),

            //* action_block
            new ProductionRule(
                "action_block",
                new Terminal(TokenType.Punctuation, "{"),
                new NonTerminal("semantic_statement"),
                new RepetitionMacro(
                    new Terminal(TokenType.Punctuation, ","),
                    new NonTerminal("semantic_statement")
                ),
                new Terminal(TokenType.Punctuation, "}")
            ),

            //* semantic_statement
            new ProductionRule(
                "semantic_statement",
                new NonTerminal("reduction")
            ),
            new ProductionRule(
                "semantic_statement",
                new NonTerminal("assignment")
            ),

            //* reduction
            new ProductionRule(
                "reduction",
                new Terminal(TokenType.Punctuation, "$"),
                new Terminal(TokenType.Punctuation, ":"),
                new NonTerminal("expression")
            ),

            //* assignment
            new ProductionRule(
                "assignment",
                new Terminal(TokenType.Identifier),
                new Terminal(TokenType.Punctuation, ":"),
                new NonTerminal("expression")
            ),

            //* expression
            new ProductionRule(
                "expression",
                new NonTerminal("literal")
            ),
            new ProductionRule(
                "expression",
                new NonTerminal("reference")
            ),
            new ProductionRule(
                "expression",
                new NonTerminal("index_expression")
            ),
            new ProductionRule(
                "expression",
                new NonTerminal("function_call")
            ),
            new ProductionRule(
                "expression",
                new NonTerminal("expression"),
                new Terminal(TokenType.Punctuation, "."),
                new NonTerminal("function_call")
            ),

            //* literal
            new ProductionRule(
                "literal",
                new Terminal(TokenType.String)
            ),
            new ProductionRule(
                "literal",
                new Terminal(TokenType.Integer)
            ),
            new ProductionRule(
                "literal",
                new Terminal(TokenType.Float)
            ),

            //* reference
            new ProductionRule(
                "reference",
                new Terminal(TokenType.Identifier)
            ),

            //* index_expression
            new ProductionRule(
                "index_expression",
                new Terminal(TokenType.Punctuation, "["),
                new Terminal(TokenType.Integer),
                new Terminal(TokenType.Punctuation, "]")
            ),

            //* function_call
            new ProductionRule(
                "function_call",
                new Terminal(TokenType.Identifier),
                new Terminal(TokenType.Punctuation, "("),
                new OptionMacro(
                    new NonTerminal("parameter_list")
                ),
                new Terminal(TokenType.Punctuation, ")")
            ),

            //* parameter_list
            new ProductionRule(
                "parameter_list",
                new NonTerminal("parameter"),
                new RepetitionMacro(
                    new Terminal(TokenType.Punctuation, ","),
                    new NonTerminal("parameter")
                )
            ),

            //* parameter
            new ProductionRule(
                "parameter",
                new Terminal(TokenType.Identifier)
            ),
        };
    }
}
