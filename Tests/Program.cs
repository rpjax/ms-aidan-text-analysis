using Aidan.TextAnalysis.GDef.Tokenization;
using Aidan.TextAnalysis.Grammars;
using Aidan.TextAnalysis.Parsing.LR1;
using Aidan.TextAnalysis.Tokenization;
using Aidan.TextAnalysis.Tokenization.GenericLexer;
using Aidan.TextAnalysis.Tokenization.StateMachine;
using System.Globalization;

namespace Aidan.TextAnalysis.Tests;

public class Program
{
    public static void Main(string[] args)
    {
        var regexLexer = new RegexTokenizerBuilder()
            .Build()
            ;

        var grammarLexer = new GrammarTokenizerBuilder()
            .Build()
            ;

        var testRegex = "^a.b*c[d-e](f|g){2,5}h\\^i\\$j\\*k\\+l\\?m\\{1,3\\}n[o-p]q\\|rs\\.t$\r\n";
        var testGrammar = @"
/*
	lexer stuff
*/

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

        foreach (var token in grammarLexer.Tokenize(testGrammar))
        {
            Console.WriteLine(token);
        }
       
        var tokenizer = CreateDebugDfa();

        var grammar = new JsonGrammar();
        var parser = new LR1Parser(grammar, tokenizer);

        var cst = parser.Parse(" { \"foo\": 123, \"bar\": 456 }");

        var inputSizes = new[] { 100, 500, 1000, 5000, 10000, 50000 }; // Varying input sizes
        const int warmUpIterations = 1000;
        const int benchmarkIterations = 10000;

        var smallFragment = @"/* foobar // 1 * 1 == 1 */ mathews 22 -34 955 john_doe 1.23 -456.78 _alpha beta 1024 gamma -0.001 delta epsilon 42.0 zeta theta /* foobar // 1 * 1 == 1 */";

        // Warm-up iterations
        for (int i = 0; i < warmUpIterations; i++)
        {
            var tokens = tokenizer.Tokenize(smallFragment).ToArray();
        }

        foreach (var inputSize in inputSizes)
        {
            // Generate input of the desired size by repeating a pattern
            var input = string.Join(" ", Enumerable.Repeat(smallFragment, inputSize / smallFragment.Length));

            // Benchmark iterations
            double bestTime = double.MaxValue;
            double worstTime = double.MinValue;
            double totalElapsedTime = 0;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < benchmarkIterations; i++)
            {
                stopwatch.Restart();
                tokenizer.Tokenize(input).ToArray();
                stopwatch.Stop();

                double elapsedTime = stopwatch.Elapsed.TotalSeconds;
                totalElapsedTime += elapsedTime;

                if (elapsedTime < bestTime)
                {
                    bestTime = elapsedTime;
                }

                if (elapsedTime > worstTime)
                {
                    worstTime = elapsedTime;
                }
            }

            double averageTime = totalElapsedTime / benchmarkIterations;
            double charsPerSecond = input.Length / averageTime;

            Console.WriteLine($"Input Size: {input.Length} characters");
            Console.WriteLine($"Best Time: {FormatTime(bestTime)} seconds");
            Console.WriteLine($"Worst Time: {FormatTime(worstTime)} seconds");
            Console.WriteLine($"Average Time: {FormatTime(averageTime)} seconds");
            Console.WriteLine($"Characters per Second: {charsPerSecond:F2}");
            Console.WriteLine();
        }

    }

    private static TokenizerMachine CreateDebugDfa()
    {
        var builder = new TokenizerDfaBuilder();

        var dfa = builder
            .SetCharset(CharsetType.Ascii)

            .FromInitialState()
                .OnWhitespace()
                .Recurse()

            // Handle Positive Integers
            .FromInitialState()
                .OnCharacterRange('0', '9')
                .GoTo("int_digit_accepted")

            .FromState("int_digit_accepted")
                .OnCharacterRange('0', '9')
                .Recurse()

            .FromState("int_digit_accepted")
                .OnCharacter('.')
                .GoTo("float_start")

            .FromState("int_digit_accepted")
                .OnWhitespace()
                .OnEoi()
                .Accept("int")

            // Handle Negative Numbers (int and float)
            .FromInitialState()
                .OnCharacter('-')
                .GoTo("negative_start")

            .FromState("negative_start")
                .OnCharacterRange('0', '9')
                .GoTo("negative_int")

            .FromState("negative_start")
                .OnCharacter('.')
                .GoTo("negative_float_start")

            .FromState("negative_int")
                .OnCharacterRange('0', '9')
                .Recurse()

            .FromState("negative_int")
                .OnCharacter('.')
                .GoTo("negative_float_start")

            .FromState("negative_int")
                .OnWhitespace()
                .OnEoi()
                .Accept("int")

            // Handle Floats (positive and negative)
            .FromState("float_start")
                .OnCharacterRange('0', '9')
                .GoTo("float_digit_accepted")

            .FromState("negative_float_start")
                .OnCharacterRange('0', '9')
                .GoTo("negative_float_digit_accepted")

            .FromState("float_digit_accepted")
                .OnCharacterRange('0', '9')
                .Recurse()

            .FromState("float_digit_accepted")
                .OnWhitespace()
                .OnEoi()
                .Accept("float")

            .FromState("negative_float_digit_accepted")
                .OnCharacterRange('0', '9')
                .Recurse()

            .FromState("negative_float_digit_accepted")
                .OnWhitespace()
                .OnEoi()
                .Accept("float")

            // Handle Identifiers
            .FromInitialState()
                .OnCharacter('_')
                .OnCharacterRange('a', 'z')
                .OnCharacterRange('A', 'Z')
                .GoTo("identifier_start")

            .FromState("identifier_start")
            .OnCharacter('_')
                .OnCharacterRange('a', 'z')
                .OnCharacterRange('A', 'Z')
                .OnCharacterRange('0', '9')
                .Recurse()

            .FromState("identifier_start")
                .OnWhitespace()
                .OnEoi()
                .Accept("identifier")

            // keyword 'if' test
            //.FromInitialState()
            //    .OnCharacter('i')
            //    .GoTo("if_i_seen")

            //.FromState("if_i_seen")
            //    .OnCharacter('f')
            //    .GoTo("if_f_seen")

            //.FromState("if_f_seen")
            //    .OnWhitespace()
            //    .OnEoi()
            //    .Accept("if")

            // Handles C style comments
            /* */
            .FromInitialState()
                .OnCharacter('/')
                .GoTo("c_comment_start")

            .FromState("c_comment_start")
                .OnCharacter('*')
                .GoTo("c_line_comment")

            .FromState("c_line_comment")
                .OnAnyCharacterExcept('*')
                .Recurse()

            .FromState("c_line_comment")
                .OnCharacter('*')
                .GoTo("c_line_comment_end_star")

            .FromState("c_line_comment_end_star")
                .OnAnyCharacterExcept('/')
                .GoTo("c_line_comment")

            .FromState("c_line_comment_end_star")
                .OnCharacter('/')
                .GoTo("c_comment_end")

            .FromState("c_comment_end")
                .OnAnyCharacter()
                .Accept("comment")

            .Build();

        return dfa;
    }

    private static string FormatTime(double seconds)
    {
        return seconds.ToString("F10", CultureInfo.InvariantCulture);
    }
}