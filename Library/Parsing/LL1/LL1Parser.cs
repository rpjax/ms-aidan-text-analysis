using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Tokenization;
using Aidan.TextAnalysis.Parsing.LL1.Components;
using Aidan.TextAnalysis.Parsing.Components;
using Aidan.TextAnalysis.Language.Extensions;
using Aidan.TextAnalysis.Tokenization.Extensions;

namespace Aidan.TextAnalysis.Parsing;

[Obsolete("This parser is under maintenance. CST builder integration is pending.")]
public class LL1Parser
{
    private LL1Grammar Grammar { get; }
    private Tokenizer Tokenizer { get; }

    public LL1Parser(LL1Grammar grammar)
    {
        Grammar = grammar;
        Tokenizer = new Tokenizer();
    }

    public CstNode Parse(string input)
    {
        using var inputStream = new InputStream(input, Tokenizer);
        var stack = new LL1Stack();
        var context = new LL1Context(Grammar, inputStream, stack);

        context.Init();

        while (true)
        {
            if (stack.Top is Eoi)
            {
                return MatchEoi(context);
            }

            if (stack.Top is Epsilon)
            {
                MatchEpsilon(context);
                continue;
            }

            if (stack.Top is Terminal)
            {
                MatchTerminal(context);
                continue;
            }

            if (stack.Top is LL1SemanticSymbol semanticSymbol)
            {
                MatchSemanticSymbol(context, semanticSymbol);
                continue;
            }

            Expand(context);
        }
    }

    private CstNode MatchEoi(LL1Context context)
    {
        if (context.InputStream.LookaheadToken is null)
        {
            throw new Exception("Unexpected end of tokens.");
        }
        if (context.InputStream.LookaheadToken.Type != TokenType.Eoi)
        {
            throw new Exception($"Unexpected token ({context.InputStream.LookaheadToken}). Expected EOI.");
        }

        context.Stack.Pop();
        context.InputStream.Consume();
        return context.SyntaxContext.BuildConcreteSyntaxTree();
    }

    private void MatchEpsilon(LL1Context context)
    {
        context.Stack.Pop();
    }

    private void MatchTerminal(LL1Context context)
    {
        var input = context.InputStream;
        var stack = context.Stack;
        var stackTop = (Terminal)context.Stack.Top!;
        var lookahead = context.InputStream.LookaheadToken;

        if (lookahead is null)
        {
            throw new Exception("Unexpected end of tokens.");
        }
        if (stackTop.Type != lookahead.Type)
        {
            throw new Exception($"Unexpected token ({input.LookaheadToken}). Expected {stackTop}.");
        }
        if (stackTop.Value is not null && stackTop.ComputeFnv1aHash() != lookahead.ComputeFnv1aHash())
        {
            throw new Exception($"Unexpected token ({input.LookaheadToken}). Expected {stackTop}.");
        }

        input.Consume();
        stack.Pop();
        context.SyntaxContext.AddAttribute(input.LookaheadToken!);
    }

    private void MatchSemanticSymbol(LL1Context context, LL1SemanticSymbol symbol)
    {
        context.Stack.Pop();

        switch (symbol.Action)
        {
            case LL1SemanticActionType.FinilizeBranch:
                context.SyntaxContext.FinilizeBranch();
                break;

            default:
                throw new InvalidOperationException("Invalid semantic action type.");
        }
    }

    private void Expand(LL1Context context)
    {
        throw new NotImplementedException();
        //var input = context.InputStream;
        //var stack = context.Stack;
        //var parsingTable = context.ParsingTable;

        //if (stack.Top is not NonTerminal nonTerminal)
        //{
        //    throw new InvalidOperationException("Invalid token at the top of the stack.");
        //}
        //if (input.LookaheadTerminal is null)
        //{
        //    throw new Exception("Unexpected end of tokens.");
        //}

        //if (!parsingTable.Lookup(nonTerminal, input.LookaheadTerminal, out var production))
        //{
        //    throw new Exception("Syntax error");
        //}

        //stack.Pop();
        //stack.Push(LL1SemanticSymbol.FinilizeBranch);
        //stack.Push(production);
        //context.SyntaxContext.CreateBranch(nonTerminal);
    }

}
