using Aidan.TextAnalysis.RegularExpressions.Ast;
using Aidan.TextAnalysis.Tokenization;

namespace Aidan.TextAnalysis.RegularExpressions.Parsing;

public static class RegExprParser
{
    private static Tokenizer? Tokenizer { get; set; }

    public static RegExpr Parse(string pattern)
    {
        var tokens = GetTokenizer()
            .TokenizeToArray(pattern);


        return null;
    }

    private static Tokenizer GetTokenizer()
    {
        if (Tokenizer is null)
        {
            Tokenizer = new RegexTokenizerBuilder()
                .Build();
        }

        return Tokenizer;
    }

}
