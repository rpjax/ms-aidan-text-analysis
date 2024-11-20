using Aidan.Core.Patterns;
using Aidan.TextAnalysis.GDef;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Components.TreeRefactor;
using Aidan.TextAnalysis.Parsing.Extensions;
using Aidan.TextAnalysis.Parsing.LR1.Components;
using Aidan.TextAnalysis.Parsing.LR1.Debug.Grammars;
using Aidan.TextAnalysis.RegularExpressions;
using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;
using Aidan.TextAnalysis.RegularExpressions.Automata;
using Aidan.TextAnalysis.RegularExpressions.Parsing;
using Aidan.TextAnalysis.Tokenization;
using Aidan.TextAnalysis.Tokenization.StateMachine;
using Aidan.TextAnalysis.Tokenization.StateMachine.Builders;
using System.Globalization;

namespace Aidan.TextAnalysis.Tests;

public static class Program
{
    public static void Main(string[] args)
    {
       // var regex = RegExpr.FromPattern("'[^']*'");
        var regex = RegExpr.FromPattern("[a-zA-Z_][a-zA-Z0-9_]*");
        GDefService gDefService = new GDefService();

        var nt_json = new NonTerminalNode("json");
        var nt_object = new NonTerminalNode("object");
        var nt_array = new NonTerminalNode("array");
        var nt_member = new NonTerminalNode("member");
        var nt_element = new NonTerminalNode("element");
        var nt_value = new NonTerminalNode("value");

        nt_json.AddChildren(new Language.Components.TreeRefactor.UnionNode(nt_object, nt_array));

        nt_object.AddChildren(
            new TerminalNode("{"),
            new NullableNode(nt_member),
            new ZeroOrMoreNode(
                new GroupingNode(
                    new TerminalNode(","),
                    nt_member
                )
            ),
            new TerminalNode("}")
        );

        nt_array.AddChildren(
            new TerminalNode("["),
            new NullableNode(nt_value),
            new ZeroOrMoreNode(
                new GroupingNode(
                    new TerminalNode(","),
                    nt_value
                )
            ),
            new TerminalNode("]")
        );

        nt_member.AddChildren(
            new TerminalNode("string"),
            new TerminalNode(":"),
            nt_value
        );

        nt_value.AddChildren(
            new Language.Components.TreeRefactor.UnionNode(
                new TerminalNode("number"),
                new TerminalNode("string"),
                new TerminalNode("true"),
                new TerminalNode("false"),
                new TerminalNode("null")
            )
        );

        var item = new LRItem(nt_array, 0, new TerminalNode[] { new TerminalNode("["), new TerminalNode("{") });

        Console.WriteLine(nt_json);
        Console.WriteLine();
        /*
         * 
         */
    }

}
