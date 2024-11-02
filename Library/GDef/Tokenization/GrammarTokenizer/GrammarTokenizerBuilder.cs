using Aidan.Core.Patterns;
using Aidan.TextAnalysis.Tokenization.StateMachine;

namespace Aidan.TextAnalysis.GDef.Tokenization;

public class GrammarTokenizerBuilder : IBuilder<TokenizerMachine>
{
    public TokenizerMachine Build()
    {
        var builder = new TokenizerDfaBuilder(initialStateName: States.InitialState);

        SkipWhitespace(builder);
        EscapeSequence(builder);
        OperatorChars(builder);
        Utf8Chars(builder);

        return builder.Build();
    }

    private void SkipWhitespace(TokenizerDfaBuilder builder)
    {
        builder.FromInitialState()
            .OnWhitespace()
            .Recurse()
            ;
    }
}
