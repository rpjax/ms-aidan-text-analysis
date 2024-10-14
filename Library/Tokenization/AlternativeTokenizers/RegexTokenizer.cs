using System.Text.RegularExpressions;

namespace Aidan.TextAnalysis.Tokenization.Experimental.RegexTokenization;

public class TokenProduction
{
    public string Type { get; }
    public Regex Regex { get; }

    public TokenProduction(
        string type,
        string pattern)
    {
        Type = type;
        Regex = new Regex(pattern);
    }
}

public class ProductionMatch
{
    public TokenProduction Production { get; }
    public Match Match { get; }

    public ProductionMatch(
        TokenProduction production,
        Match match)
    {
        Production = production;
        Match = match;
    }
}

public class TokenizationContext
{
    public string Input { get; }
    public int Index { get; private set; }
    public int Line { get; private set; }
    public int Column { get; private set; }

    public TokenizationContext(
        string input,
        int index,
        int line,
        int column)
    {
        Input = input;
        Index = index;
        Line = line;
        Column = column;
    }

    public void IncrementPosition()
    {
        Index++;
        Column++;
    }

    public void IncrementLine()
    {
        Line++;
        Column = 0;
    }

    public void ResetColumn()
    {
        Column = 0;
    }

}

public class RegexTokenizer : IStringTokenizer
{
    private TokenProduction[] Productions { get; }

    public RegexTokenizer(IEnumerable<TokenProduction> productions)
    {
        Productions = productions.ToArray();
    }

    public IEnumerable<IToken> Tokenize(
        string input,
        bool includeEoi = true)
    {
        var context = new TokenizationContext(input, index: 0, line: 0, column: 0);

        while (true)
        {
            var match = MatchProduction(context);

            //if (match == null)
            //{
            //    if (context.Index >= input.Length)
            //    {
            //        if (includeEoi)
            //        {
            //            var type = "EOI";
            //            var value = string.Empty;
            //            var metadata = GetTokenMetadata(context, match);

            //            yield return new TokenNew(type, value, metadata);
            //        }

            //        yield break;
            //    }

            //    throw new Exception(
            //        $"No production matched at index {index}.");
            //}
        }
    }

    private ProductionMatch? MatchProduction(TokenizationContext context)
    {
        var input = context.Input;
        var index = context.Index;
        var matches = new List<ProductionMatch>();

        foreach (var production in Productions)
        {
            var match = production.Regex.Match(input, index);

            if (match.Success && match.Index == index)
            {
                matches.Add(new ProductionMatch(production, match));
            }
        }

        return matches
            .OrderByDescending(m => m.Match.Length)
            .FirstOrDefault();
    }

    private string GetTokenType(ProductionMatch? match)
    {
        if (match == null)
        {
            return "EOI";
        }

        return match.Production.Type;
    }

    private TokenPosition GetTokenPosition(
        TokenizationContext context,
        ProductionMatch? match)
    {
        var index = context.Index;

        return new TokenPosition(
            startIndex: index,
            endIndex: index + match?.Match.Length ?? 0,
            line: context.Line,
            column: context.Column);
    }

    private TokenMetadata GetTokenMetadata(
        TokenizationContext context,
        ProductionMatch? match)
    {
        return new TokenMetadata(GetTokenPosition(context, match));
    }

}
