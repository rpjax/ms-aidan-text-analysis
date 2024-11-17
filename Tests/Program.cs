using Aidan.Core.Patterns;
using Aidan.TextAnalysis.Parsing.Extensions;
using Aidan.TextAnalysis.RegularExpressions;
using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.Tokenization;
using Aidan.TextAnalysis.Tokenization.StateMachine;
using Aidan.TextAnalysis.Tokenization.StateMachine.Builders;
using System.Globalization;

namespace Aidan.TextAnalysis.Tests;

public static class Program
{
    public static void Main(string[] args)
    {
        var regex = RegExpr.FromPattern("a|b");
    }

}
