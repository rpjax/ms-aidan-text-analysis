using Aidan.TextAnalysis.GDef.Components;
using Aidan.TextAnalysis.Parsing.LR1;
using Aidan.TextAnalysis.Parsing.Tree;
using Aidan.TextAnalysis.RegularExpressions.Automata;
using Aidan.TextAnalysis.Tokenization;

namespace Aidan.TextAnalysis.GDef;

public class GDefService
{
    public static Tokenizer CreateTokenizer(IEnumerable<GDefLexeme> lexemes, IEnumerable<char> ignoredChars)
    {
        var _lexemes = lexemes
            .Select(x => new Lexeme(x.Name, x.Pattern))
            .ToArray();

        var calculator = new TokenizerCalculator(
            lexemes: _lexemes,
            ignoredChars: ignoredChars,
            useDebug: false);

        return calculator.ComputeTokenizer();
    }

    public static LR1Parser CreateLR1Parser(string rawGrammar, IEnumerable<char> ignoredChars)
    {
        var grammar = GDefParser.ParseToGrammar(rawGrammar);
        var tokenizer = CreateTokenizer(grammar.Lexemes, ignoredChars);
        var ignoredLexemes = grammar.Lexemes
            .Where(x => x.IsIgnored)
            .Select(x => x.Name)
            .ToArray();

        return new LR1Parser(grammar, tokenizer, ignoredLexemes);
    }

}

public class GDefTranslatorRefactoring
{
    private CstRootNode GrammarCst { get; }
    private List<GDefLexeme> Lexemes { get; }

    public GDefTranslatorRefactoring(CstRootNode grammar)
    {
        GrammarCst = grammar;
    }


}

