using Aidan.TextAnalysis.Tokenization.StateMachine.Components;

namespace Aidan.TextAnalysis.Tokenization.StateMachine.Builders;

public class TableTransitionBuilder : TableTransitionBuilderBase<TableTransitionBuilder>
{
    private TokenizerBuilder Builder { get; }

    public TableTransitionBuilder(
        TokenizerBuilder builder,
        TokenizerState currentState)
        : base(
            currentState: currentState,
            charset: builder.GetCharset())
    {
        Builder = builder;
    }

    public TokenizerBuilder GoTo(uint state)
    {
        foreach (var c in Characters)
        {
            Builder.AddTransition(
                sourceStateId: CurrentState.Id,
                character: c,
                nextStateId: state);
        }

        return Builder;
    }

    public TokenizerBuilder Recurse()
    {
        foreach (var c in Characters)
        {
            Builder.AddTransition(
                sourceStateId: CurrentState.Id,
                character: c,
                nextStateId: CurrentState.Id);
        }

        return Builder;
    }

    public TokenizerBuilder Accept()
    {
        if (!CurrentState.IsAccepting)
        {
            CurrentState.IsAccepting = true;
        }

        return Builder;
    }

}
