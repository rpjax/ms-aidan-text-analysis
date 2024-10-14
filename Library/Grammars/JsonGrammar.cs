using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Tokenization;

namespace Aidan.TextAnalysis.Grammars;

public class JsonGrammar : Grammar
{
    public JsonGrammar() : base(GetStart(), GetProductions())
    {
    }

    private static ProductionRule[] GetProductions()
    {
        return new ProductionRule[]
        {
            // json:
            new ProductionRule(
                "json",
                new NonTerminal("object"),
                new AlternativeMacro(),
                new NonTerminal("array")
            ),

            // object:
            new ProductionRule(
                "object",
                new Terminal(TokenType.Punctuation, "{"),
                new OptionMacro(
                    new NonTerminal("members")
                ),
                new Terminal(TokenType.Punctuation, "}")
            ),

            // members:
            new ProductionRule(
                "members",
                new NonTerminal("pair"),
                new RepetitionMacro(
                    new Terminal(TokenType.Punctuation, ","),
                    new NonTerminal("pair")
                )
            ),

            // pair:
            new ProductionRule(
                "pair",
                new Terminal(TokenType.String),
                new Terminal(TokenType.Punctuation, ":"),
                new NonTerminal("value")
            ),

            // array:
            new ProductionRule(
                "array",
                new Terminal(TokenType.Punctuation, "["),
                new OptionMacro(
                    new NonTerminal("elements")
                ),
                new Terminal(TokenType.Punctuation, "]")
            ),

            // elements:
            new ProductionRule(
                "elements",
                new NonTerminal("value"),
                new RepetitionMacro(
                    new Terminal(TokenType.Punctuation, ","),
                    new NonTerminal("value")
                )
            ),

            // value:
            new ProductionRule(
                "value",
                new Terminal(TokenType.String),
                new AlternativeMacro(),
                new NonTerminal("number"),
                new AlternativeMacro(),
                new NonTerminal("object"),
                new AlternativeMacro(),
                new NonTerminal("array"),
                new AlternativeMacro(),
                new Terminal(TokenType.Identifier, "true"),
                new AlternativeMacro(),
                new Terminal(TokenType.Identifier, "false"),
                new AlternativeMacro(),
                new Terminal(TokenType.Identifier, "null")
            ),

            // number:
            new ProductionRule(
                "number",
                new Terminal(TokenType.Integer),
                new AlternativeMacro(),
                new Terminal(TokenType.Float),
                new AlternativeMacro(),
                new Terminal(TokenType.Hexadecimal)
            )
        };
    }

    private static NonTerminal GetStart()
    {
        return new NonTerminal("json");
    }
}