using System.Diagnostics;

namespace Aidan.TextAnalysis.Tests;

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
