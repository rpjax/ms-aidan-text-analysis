using Aidan.TextAnalysis.GDef;
using Aidan.TextAnalysis.GDef.Tokenization;
using Aidan.TextAnalysis.Parsing.Extensions;
using Aidan.TextAnalysis.RegularExpressions;
using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.RegularExpressions.Automata;
using System.Diagnostics;
using System.Globalization;

namespace Aidan.TextAnalysis.Tests;

public class Program
{
    public static void Main(string[] args)
    {
        /* debug test */
        var testLexemes = new Lexeme[]
        {
            new Lexeme("lexeme 'a'", RegexNode.FromString("a")),
            new Lexeme("lexeme 'ab'", RegexNode.FromString("ab")),
            //new Lexeme("lexeme 'ab'", new ConcatenationNode(
            //    new LiteralNode('a'),
            //    new StarNode(new LiteralNode('b'))
            //)),
            //new Lexeme("lexeme 'c'", RegexNode.FromString("c")),
        };
                                   
        /* GDef test */
        var lexemes = GDefLexemes.GetLexemes();
        var ignoredChars = new char[] { ' ', '\t', '\n', '\r', '\0' };

        /* tokenizer test */
        var calculator = new TokenizerCalculator(
            lexemes: lexemes,
            ignoredChars: ignoredChars,
            useDebug: true);

        var tokenizer = calculator.ComputeTokenizer();
        var input = "aab";

        var tokens = tokenizer.TokenizeToArray(input);

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }

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