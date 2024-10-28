using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Grammars;

public class JsonGrammar : Grammar
{
    public JsonGrammar() : base(GetStart(), GetProductions())
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
            // start:
            new ProductionRule(
                new NonTerminal("start"),
                new NonTerminal("json")
            ),

            // json:
            new ProductionRule(
                new NonTerminal("json"),
                new NonTerminal("object"),
                new PipeMacro(),
                new NonTerminal("array")
            ),

            // object:
            new ProductionRule(
                new NonTerminal("object"),
                new Terminal("{"),
                new OptionMacro(
                    new NonTerminal("members")
                ),
                new Terminal("}")
            ),

            // members:
            new ProductionRule(
                new NonTerminal("members"),
                new NonTerminal("pair"),
                new RepetitionMacro(
                    new Terminal(","),
                    new NonTerminal("pair")
                )
            ),

            // pair:
            new ProductionRule(
                new NonTerminal("pair"),
                new Terminal("string"),
                new Terminal(":"),
                new NonTerminal("value")
            ),

            // array:
            new ProductionRule(
                new NonTerminal("array"),
                new Terminal("["),
                new OptionMacro(
                    new NonTerminal("elements")
                ),
                new Terminal("]")
            ),

            // elements:
            new ProductionRule(
                new NonTerminal("elements"),
                new NonTerminal("value"),
                new RepetitionMacro(
                    new Terminal(","),
                    new NonTerminal("value")
                )
            ),

            // value:
            new ProductionRule(
                new NonTerminal("value"),
                new Terminal("string"),
                new PipeMacro(),
                new NonTerminal("number"),
                new PipeMacro(),
                new NonTerminal("object"),
                new PipeMacro(),
                new NonTerminal("array"),
                new PipeMacro(),
                new Terminal("true"),
                new PipeMacro(),
                new Terminal("false"),
                new PipeMacro(),
                new Terminal("null")
            ),

            // number:
            new ProductionRule(
                new NonTerminal("number"),
                new Terminal("int"),
                new PipeMacro(),
                new Terminal("float"),
                new PipeMacro(),
                new Terminal("hex")
            )
        };
    }

}