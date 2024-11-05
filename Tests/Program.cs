using Aidan.TextAnalysis.GDef;
using Aidan.TextAnalysis.GDef.Tokenization;
using Aidan.TextAnalysis.Parsing.Extensions;
using Aidan.TextAnalysis.Regexes;
using System.Diagnostics;
using System.Globalization;

namespace Aidan.TextAnalysis.Tests;

public class Program
{
    public static void Main(string[] args)
    {
        /* regex tests */
        var regex = new ConcatenationNode(
            new LiteralNode('a'),
            new ConcatenationNode(
                new LiteralNode('b'),
                new StarNode(new LiteralNode('a'))
            )
        );

        // Represents the regex: ((a|b*)c(d|e)f)* | (g(h|i*|j)*k)
        var complexRegex = new UnionNode(
            new StarNode(
                new ConcatenationNode(
                    new ConcatenationNode(
                        new ConcatenationNode(
                            new UnionNode(
                                new LiteralNode('a'),
                                new StarNode(new LiteralNode('b'))
                            ),
                            new LiteralNode('c')
                        ),
                        new UnionNode(
                            new LiteralNode('d'),
                            new LiteralNode('e')
                        )
                    ),
                    new LiteralNode('f')
                )
            ),
            new ConcatenationNode(
                new LiteralNode('g'),
                new ConcatenationNode(
                    new StarNode(
                        new UnionNode(
                            new LiteralNode('h'),
                            new UnionNode(
                                new StarNode(new LiteralNode('i')),
                                new LiteralNode('j')
                            )
                        )
                    ),
                    new LiteralNode('k')
                )
            )
        );

        var dfaCalculator = new RegexDfaCalculator(complexRegex);
        var dfa = dfaCalculator.ComputeDfa();

        Console.WriteLine();

        /* parser tests */

        var testGrammar = @"
/*
	lexer stuff
*/

lexeme if = 'if' ;

start
	: json 
	;

json
	: object
	| array
	;

object
	: '{' { members } '}' 
	;

members
	: pair  { ',' members } 
	;

pair
	: $string ':' value 
	;

array
	: '[' [ elements ] ']'
	;

elements
	: value { ',' value }
	;

value 
	: number 
	| object 
	| array
	| $string
	| bool
	| null 
	;

bool
	: 'true' 
	| 'false' 
	;

null
	: 'null' 
	;

number
	: $int 
	| $float 
	| $hex 
	;

";

        var tokens = GDefTokenizers.GrammarTokenizer
            .Tokenize(testGrammar)
            .ToArray();

        var grammarCst = GDefParser.Parse(testGrammar);
        var html = grammarCst.ToHtmlTreeView();
        var grammar = GDefParser.ParseGrammar(testGrammar);
        Console.WriteLine(grammar);
    }

    private static string FormatTime(double seconds)
    {
        return seconds.ToString("F10", CultureInfo.InvariantCulture);
    }
}

public class Benchmark
{
    private Action Action { get; }
    private int WarmupIterations { get; } = 10;
    private int Iterations { get; } = 1000;

    public Benchmark(Action action)
    {
        Action = action;
    }

    public double Run(int iterations)
    {
        var sw = new Stopwatch();
        sw.Start();

        for (var i = 0; i < iterations; i++)
        {
            Action();
        }

        sw.Stop();
        return sw.Elapsed.TotalSeconds;
    }

}