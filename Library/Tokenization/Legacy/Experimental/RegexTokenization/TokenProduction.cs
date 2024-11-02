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
