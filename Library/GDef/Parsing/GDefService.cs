using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Parsing.LR1;
using Aidan.TextAnalysis.RegularExpressions.Automata;
using Aidan.TextAnalysis.Tokenization;
using System.Text;

namespace Aidan.TextAnalysis.GDef;

public class GdefGrammar : IGrammar
{
    public Charset Alphabet { get; }
    public Charset IgnoredChars { get; }
    public IReadOnlyList<Lexeme> Lexemes { get; }
    public IReadOnlyList<string> IgnoredLexemes { get; }
    public IReadOnlyList<INonTerminal> NonTerminals { get; private set; }
    public IReadOnlyList<ITerminal> Terminals { get; private set; }
    public IReadOnlyList<IProductionRule> ProductionRules { get; private set; }
    public INonTerminal StartSymbol { get; private set; }

    public GdefGrammar(
        IEnumerable<char> alphabet,
        IEnumerable<char> ignoredChars,
        IEnumerable<Lexeme> lexemes,
        IReadOnlyList<string> ignoredLexemes,
        IEnumerable<IProductionRule> productions,
        INonTerminal start)
    {
        Alphabet = new Charset(alphabet);
        IgnoredChars = new Charset(ignoredChars);
        Lexemes = lexemes.ToArray();
        IgnoredLexemes = ignoredLexemes;

        NonTerminals = productions
            .Select(x => x.Head)
            .Distinct(new NonTerminalEqualityComparer())
            .ToArray();

        Terminals = productions
            .SelectMany(x => x.Body)
            .OfType<ITerminal>()
            .Distinct(new TerminalEqualityComparer())
            .ToArray();

        ProductionRules = productions.ToArray();
        StartSymbol = start;
    }

    /// <summary>
    /// Returns a string that represents the current grammar.
    /// </summary>
    /// <returns>A string that represents the current grammar.</returns>
    public override string ToString()
    {
        var builder = new StringBuilder();

        builder.AppendLine($"Start Symbol: {StartSymbol}");
        builder.AppendLine("Productions:");

        foreach (var production in ProductionRules)
        {
            builder.AppendLine(production.ToString());
        }

        return builder.ToString();
    }
}

public class GDefService
{
    public static Tokenizer CreateTokenizer(GdefGrammar grammar)
    {
        var calculator = new TokenizerCalculator(
            lexemes: grammar.Lexemes,
            ignoredChars: grammar.IgnoredChars,
            useDebug: false);

        return calculator.ComputeTokenizer();
    }

    public static LR1Parser CreateLR1Parser(GdefGrammar grammar)
    {
        var tokenizer = CreateTokenizer(grammar);
        var ignoredTokens = grammar.IgnoredLexemes.ToArray();

        return new LR1Parser(
            grammar: grammar, 
            tokenizer: tokenizer, 
            ignoredTokenTypes: ignoredTokens);
    }

}

