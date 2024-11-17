using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;
using Aidan.TextAnalysis.Parsing.Components;
using Aidan.TextAnalysis.Parsing.Core;
using Aidan.TextAnalysis.Parsing.LR1.Components;
using Aidan.TextAnalysis.Tokenization;
using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.Parsing.LR1;

/// <summary>
/// Represents a LR(1) parser. It is capable of parsing text based on a given grammar.
/// </summary>
public class LR1Parser : IStringParser
{
    private static readonly string[] DefaultIgnoreSet = new string[]
    {
        "comment"
    };

    private IGrammar Grammar { get; }
    private IStringTokenizer Tokenizer { get; }
    private ILR1ParserTable Table { get; }
    private string[] IgnoredTokenTypes { get; }

    /// <summary>
    /// Creates a new instance of <see cref="LR1Parser"/>. It automatically transforms the grammar to LR(1) and creates a parsing table.
    /// </summary>
    /// <param name="grammar"> The grammar to parse. </param>
    /// <param name="tokenizer"> The lexer to tokenize the input text. </param>
    /// <param name="ignoredTokenTypes"> The set of token types to ignore. </param>
    public LR1Parser(
        IGrammar grammar, 
        IStringTokenizer tokenizer, 
        string[]? ignoredTokenTypes = null)
    {
        Grammar = grammar;
        Tokenizer = tokenizer;
        Table = LR1ParserTable.Create(Grammar);
        IgnoredTokenTypes = ignoredTokenTypes ?? DefaultIgnoreSet;
    }

    /// <summary>
    /// Parses the given text and returns the concrete syntax tree (CST) based on the grammar specified in the constructor.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public CstRootNode Parse(string text)
    {
        var tokens = Tokenizer.Tokenize(text);
            
        using var inputStream = new LR1InputStream(
            tokens: tokens,
            ignoreSet: IgnoredTokenTypes);
            
        var stack = new LR1ParserStack();

        var cstBuilder = new CstBuilder(
            includeEpsilons: false
        );

        var context = new LR1ParserContext(
            inputStream: inputStream,
            stack: stack,
            cstBuilder: cstBuilder
        );

        //* pushes the initial state onto the stack
        stack.PushState(0);

        while (true)
        {
            var action = GetNextAction(context);

            ExecuteAction(context, action);

            if (action.Type == LR1ActionType.Accept)
            {
                break;
            }
        }

        return context.CstBuilder.Build();
    }

    /// <summary>
    /// Gets the next action to execute by performing a lookup in the parsing parsing table using the current state and the lookahead token.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private LR1Action GetNextAction(LR1ParserContext context)
    {
        var currentState = context.Stack.GetCurrentState();
        var lookahead = context.InputStream.GetLookaheadSymbol();

        var action = Table.Lookup(currentState, lookahead);

        if (action is null)
        {
            throw new Exception();
        }

        return action;
    }

    /// <summary>
    /// Dynamically executes the action based on its type.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="action"></param>
    /// <exception cref="InvalidOperationException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ExecuteAction(LR1ParserContext context, LR1Action action)
    {
        switch (action.Type)
        {
            case LR1ActionType.Shift:
                Shift(context, action.AsShift());
                return;

            case LR1ActionType.Reduce:
                Reduce(context, action.AsReduce());
                return;

            case LR1ActionType.Goto:
                Goto(context, action.AsGoto());
                break;

            case LR1ActionType.Accept:
                Accept(context, action.AsAccept());
                break;

            default:
                throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Shifts the lookahead token onto the stack and consumes it from the input stream.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="action"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Shift(LR1ParserContext context, LR1ShiftAction action)
    {
        var lookahead = context.InputStream.GetLookaheadSymbol();
        var token = context.InputStream.GetLookaheadToken();

        /* handles the shift action */
        context.Stack.PushSymbol(lookahead);
        context.Stack.PushState(action.NextState);
        context.InputStream.Advance();
        /* add leaf to CST */
        context.CstBuilder.CreateLeaf(token);
    }

    /// <summary>
    /// Reduces the stack based on the production rule specified in the action.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="reduceAction"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Reduce(LR1ParserContext context, LR1ReduceAction reduceAction)
    {
        var production = Table.LookupProduction(reduceAction.ProductionIndex);

        if (production.IsEpsilonProduction())
        {
            EpsilonReduce(context, production);
        }
        else
        {
            NormalReduce(context, production);
        }
    }

    /// <summary>
    /// Reduces the stack based on a normal production rule.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="production"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void NormalReduce(LR1ParserContext context, IProductionRule production)
    {
        for (int i = 0; i < production.Body.Length; i++)
        {
            context.Stack.PopState();
            context.Stack.PopSymbol();
        }

        var nonTerminal = production.Head;
        var currentState = context.Stack.GetCurrentState();

        context.Stack.PushSymbol(nonTerminal);

        var action = Table.Lookup(currentState, nonTerminal);

        if (action is null)
        {
            throw new InvalidOperationException();
        }

        if (action.Type != LR1ActionType.Goto)
        {
            throw new InvalidOperationException();
        }

        var gotoAction = action.AsGoto();
        var nextState = gotoAction.NextState;

        // The state 1 is always the accept state due to the way the LR(1) parser is constructed.
        // The only reduction that occurs in state 1 is the start symbol reduction, wich is the accept condition.
        // So after reducing the start symbol, the parser should accept the input, and the CST build process should be finished.
        var isAcceptState = nextState == 1;

        /* executes the goto action */
        context.Stack.PushState(nextState);

        if (isAcceptState)
        {
            context.CstBuilder.CreateRoot(production);
        }
        else
        {
            context.CstBuilder.CreateInternal(production);
        }
    }

    /// <summary>
    /// Reduces the stack based on an epsilon production rule.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="production"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EpsilonReduce(LR1ParserContext context, IProductionRule production)
    {
        var nonTerminal = production.Head;
        var currentState = context.Stack.GetCurrentState();

        context.Stack.PushSymbol(nonTerminal);

        var action = Table.Lookup(currentState, nonTerminal);

        if (action is null)
        {
            throw new InvalidOperationException();
        }

        if (action.Type != LR1ActionType.Goto)
        {
            throw new InvalidOperationException();
        }

        var gotoAction = action.AsGoto();
        var nextState = gotoAction.NextState;

        context.Stack.PushState(gotoAction.NextState);
        context.CstBuilder.CreateEpsilonInternal(production);
    }

    /// <summary>
    /// Goes to the next state based on the goto action.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="gotoAction"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Goto(LR1ParserContext context, LR1GotoAction gotoAction)
    {
        context.Stack.PushState(gotoAction.NextState);
    }

    /// <summary>
    /// Accepts the input.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="action"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Accept(LR1ParserContext context, LR1AcceptAction action)
    {
        context.Stack.PopState();
    }

}