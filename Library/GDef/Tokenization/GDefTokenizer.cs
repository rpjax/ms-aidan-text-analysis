using Aidan.TextAnalysis.Tokenization.StateMachine;

namespace Aidan.TextAnalysis.GDef.Tokenization;

public static class GDefTokenizers
{
    public static TokenizerMachine GrammarTokenizer { get; } 
    public static TokenizerMachine RegexTokenizer { get; } 

    static GDefTokenizers()
    {
        GrammarTokenizer = new GrammarTokenizerBuilder().Build();
        RegexTokenizer = new RegexTokenizerBuilder().Build();
    }
}