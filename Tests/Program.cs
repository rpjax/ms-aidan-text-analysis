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
        var tokenizer = new GrammarTokenizerBuilder()
            .Build();

        var input = @"foobar lexeme user use charset baz";

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