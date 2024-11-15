using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.RegularExpressions.Automata;

namespace Aidan.TextAnalysis.GDef.Tokenization;

public static class GDefLexemes
{
    public static string Comment { get; } = "comment";
    public static string Identifier { get; } = "identifier";
    public static string String { get; } = "string";
    public static char[] StringDelimiters { get; } = new char[] { '"', '\'' };

    /* keywords */
    public static string Lexeme { get; } = "lexeme";
    public static string Use { get; } = "use";
    public static string Charset { get; } = "charset";

    /* special characters */
    public static char[] SpecialChars { get; } = new char[]
    {
        ':', ';', '$', '{', '}', '[', ']', '(', ')', '|', '='
    };

    public static Lexeme[] GetLexemes()
    {
        /* 
         * The order of lexemes matters. 
         * If a lexeme is a subset of another lexeme, the subset lexeme should be defined first 
         */
        var lexemes = new List<Lexeme>
        {
            GetLexemeLexeme(),
            GetUseLexeme(),
            GetCharsetLexeme(),
            GetIdentifierLexeme(),
        };

        lexemes.AddRange(GetSpecialCharsLexemes());

        return lexemes.ToArray();
    }

    private static Lexeme GetIdentifierLexeme()
    {
        var letterRegex = new UnionNode(
            RegexNode.FromCharacterRange('a', 'z'),
            RegexNode.FromCharacterRange('A', 'Z')
        );

        var digitRegex = RegexNode.FromCharacterRange('0', '9');
        var underscoreRegex = RegexNode.FromCharacter('_');

        // Start of identifier: a letter or underscore
        var startRegex = new UnionNode(letterRegex, underscoreRegex);

        // Rest of identifier: letters, digits, or underscores, repeated zero or more times
        var restRegex = new StarNode(
            new UnionNode(
                new UnionNode(letterRegex, digitRegex),
                underscoreRegex
            )
        );

        // Full identifier regex: start with a letter or underscore, followed by any combination of letters, digits, or underscores
        var identifierRegex = new ConcatenationNode(startRegex, restRegex);

        return new Lexeme(name: Identifier, pattern: identifierRegex);
    }

    private static Lexeme GetLexemeLexeme()
    {
        return new Lexeme(name: Lexeme, pattern: RegexNode.FromString("lexeme"));
    }

    private static Lexeme GetUseLexeme()
    {
        return new Lexeme(name: Use, pattern: RegexNode.FromString("use"));
    }

    private static Lexeme GetCharsetLexeme()
    {
        return new Lexeme(name: Charset, pattern: RegexNode.FromString("charset"));
    }

    private static IEnumerable<Lexeme> GetSpecialCharsLexemes()
    {
        foreach (var c in SpecialChars)
        {
            yield return new Lexeme(name: c.ToString(), pattern: RegexNode.FromCharacter(c));
        }
    }
}
