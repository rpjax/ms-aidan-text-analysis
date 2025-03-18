using System.Diagnostics;
using Aidan.TextAnalysis.GDef;
using Aidan.TextAnalysis.GDef.Parsing;

namespace Tests;

public static class Program
{
    static string rawJsonGrammar = @"
/* lexer */

lexeme int = '[0-9]+';
lexeme float = '[0-9]+\.[0-9]+';
lexeme hex = '0x[0-9a-fA-F]+';
lexeme id = '[a-zA-Z_][a-zA-Z0-9_]*';

[charset: 'utf8', ignore: true]
lexeme string = '""([^""\n]*)""';

/* parser */

start
	: json 'bar'
	;
";

    public static void Main(string[] args)
    {
        var parser = GDefService.CreateLR1Parser(
            rawGrammar: @"
lexeme id = '[A-z]+';

root: assignment;
assignment: 'var' $id '=' $id;
",
            ignoredChars: new char[] { '\0', '\n', ' ' });

        var ignoredChars = new char[] { ' ', '\t', '\n', '\r', '\0' };
        var jsonParser = GDefService.CreateLR1Parser(rawJsonGrammar, ignoredChars);

        return;

        // Warm-up runs
        const int warmUpRuns = 5000;
        for (int i = 0; i < warmUpRuns; i++)
        {
            _ = GDefParser.Parse(rawJsonGrammar);
        }

        Console.WriteLine("Warpup Done");

        // Actual benchmark runs
        const int iterations = 10000;
        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            _ = GDefParser.Parse(rawJsonGrammar);
        }

        stopwatch.Stop();

        var elapsedTicks = stopwatch.ElapsedTicks;
        var microseconds = elapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
        var averageMicroseconds = microseconds / iterations;

        // Format and print
        Console.WriteLine($"Total Time: {microseconds} µs");
        Console.WriteLine($"Average Time Per Iteration: {averageMicroseconds} µs");

    }

}
