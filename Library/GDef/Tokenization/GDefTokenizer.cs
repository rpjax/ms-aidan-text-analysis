using Aidan.TextAnalysis.Tokenization;

namespace Aidan.TextAnalysis.GDef.Tokenization;

public static class GDefTokenizers
{
    public static Tokenizer GrammarTokenizer { get; } 
    public static Tokenizer RegexTokenizer { get; } 

    static GDefTokenizers()
    {
        //GrammarTokenizer = new GrammarTokenizerBuilder().Build();
        RegexTokenizer = new RegexTokenizerBuilder().Build();
    }
}