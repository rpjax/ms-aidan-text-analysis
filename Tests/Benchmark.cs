using System.Diagnostics;
using System.Globalization;

namespace Tests;

public class Benchmark
{
    private Action Action { get; }
    private int WarmupIterations { get; } = 10;
    private int Iterations { get; } = 1000;

    public Benchmark(Action action)
    {
        Action = action;
    }

    private static string FormatTime(double seconds)
    {
        return seconds.ToString("F10", CultureInfo.InvariantCulture);
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
