using Aidan.TextAnalysis.Tokenization.StateMachine.Components;

namespace Aidan.TextAnalysis.Tokenization.StateMachine.Builders;

public class ManualTableTransitionBuilder : TableTransitionBuilderBase<ManualTableTransitionBuilder>
{
    private ManualTokenizerBuilder Builder { get; }

    public ManualTableTransitionBuilder(
        ManualTokenizerBuilder builder,
        TokenizerState currentState)
        : base(
            currentState: currentState, 
            charset: builder.GetCharset())
    {
        Builder = builder;
    }
   
    /* 
     * build finilizer methods 
     */

    public ManualTokenizerBuilder GoTo(string name)
    {
        if (Characters.Count == 0)
        {
            throw new InvalidOperationException("Characters not set.");
        }

        var nextState = Builder.FindState(name);

        if (nextState == null)
        {
            nextState = Builder.CreateState(name, isAccepting: false);
        }

        foreach (var character in Characters)
        {
            Builder.AddTransition(CurrentState.Name, character, nextState.Name);
        }

        return Builder;
    }

    public ManualTokenizerBuilder Accept()
    {
        if (!CurrentState.IsAccepting)
        {
            CurrentState.IsAccepting = true;
        }

        return Builder;
    }

    public ManualTokenizerBuilder RecurseOnNoTransition()
    {
        if (!CurrentState.IsRecursiveOnNoTransition)
        {
            CurrentState.IsRecursiveOnNoTransition = true;
        }

        return Builder;
    }

    public ManualTokenizerBuilder Recurse()
    {
        if (Characters.Count == 0)
        {
            throw new InvalidOperationException("Characters not set.");
        }

        foreach (var character in Characters)
        {
            Builder.AddTransition(CurrentState.Name, character, CurrentState.Name);
        }

        return Builder;
    }

    public ManualTokenizerBuilder GoToInitialState()
    {
        if (Characters.Count == 0)
        {
            throw new InvalidOperationException("Characters not set.");
        }

        var initialState = Builder.GetInitialState();

        foreach (var character in Characters)
        {
            Builder.AddTransition(CurrentState.Name, character, initialState.Name);
        }

        return Builder;
    }

}
