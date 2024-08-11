using Aidan.Core;
using Aidan.Core.Extensions;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;
using Aidan.TextAnalysis.Parsing.LL1.Tools;

namespace Aidan.TextAnalysis.Language.Graph;

public enum ChildPosition
{
    Left,
    Middle,
    Right
}

public static class LL1GraphBuilder
{
    public static LL1GraphNode CreateGraphTree(ProductionSet set)
    {
        if (set.Start is null)
        {
            throw new InvalidOperationException("The production set has no start symbol.");
        }

        return FromSymbol(
            set: set,
            symbol: set.Start,
            stack: new Stack<LL1GraphNode>(),
            terminalProducers: new List<NonTerminal>()
        );
    }

    private static LL1GraphNode FromSymbol(
        ProductionSet set,
        Symbol symbol,
        Stack<LL1GraphNode> stack,
        List<NonTerminal> terminalProducers,
        ProductionRule? symbolProduction = null)
    {
        if (symbol.IsMacro)
        {
            throw new InvalidOperationException($"The symbol {symbol} is a macro. It cannot be added to the graph.");
        }

        if (symbol.IsTerminal)
        {
            if(symbolProduction is null)
            {
                throw new InvalidOperationException("The symbol production is null.");
            }

            terminalProducers.Add(symbolProduction.Value.Head);

            return new LL1GraphNode(
                symbol: symbol,
                production: symbolProduction
            );
        }

        var recursiveNode = stack
            .Where(x => x.Symbol == symbol)
            .FirstOrDefault();

        if (recursiveNode is not null)
        {
            var terminalFound = false;

            if (symbolProduction is null)
            {
                throw new InvalidOperationException("The symbol production is null.");
            }

            foreach (var item in symbolProduction.Value.Body)
            {
                if (ReferenceEquals(item, symbol))
                {
                    break;
                }
                if (item.IsTerminal)
                {
                    terminalFound = true;
                    break;
                }

                if (item is not NonTerminal _nonTerminal)
                {
                    throw new Exception("The item is not a non-terminal.");
                }

                var firstSet = LL1FirstSetTool.ComputeFirstSet(set, _nonTerminal);

                if (firstSet.Where(x => !x.IsEpsilon).IsEmpty())
                {
                    continue;
                }

                terminalFound = true;
            }

            foreach (var node in stack)
            {
                if (terminalFound)
                {
                    break;
                }
                if (node.Symbol == recursiveNode.Symbol)
                {
                    break;
                }

                var prefix = node.GetPrefix();

                foreach (var item in prefix)
                {
                    if (item.IsTerminal)
                    {
                        terminalFound = true;
                        break;
                    }

                    if(item is not NonTerminal _nonTerminal)
                    {
                        throw new Exception("The item is not a non-terminal.");
                    }

                    var firstSet = LL1FirstSetTool.ComputeFirstSet(set, _nonTerminal);

                    if (firstSet.Where(x => !x.IsEpsilon).IsEmpty())
                    {
                        continue;
                    }

                    terminalFound = true;
                }
            }

            var recursionType = terminalFound
                    ? RecursionType.Normal
                    : RecursionType.IndirectLeft;

            if (!terminalFound && stack.Peek() == recursiveNode)
            {
                recursionType = RecursionType.Left;
            }

            return new LL1GraphNode(
                symbol: symbol,
                production: symbolProduction,
                recursionType: recursionType,
                children: recursiveNode.Children
            );
        }
        
        // S -> a B c.
        // B -> S d.
        //var leftRecursiveNode = stack
        //    .Where(x => x.Symbol == symbol)
        //    .Where(x => x.Inde)

        //if (recursiveNode is not null)
        //{
        //    var terminalFound = false;

        //    if (symbolProduction is null)
        //    {
        //        throw new InvalidOperationException("The symbol production is null.");
        //    }

        //    foreach (var item in symbolProduction.Body)
        //    {
        //        if (ReferenceEquals(item, symbol))
        //        {
        //            break;
        //        }
        //        if (item.IsTerminal)
        //        {
        //            terminalFound = true;
        //            break;
        //        }
        //    }

        //    if (!terminalFound)
        //    {
        //        foreach (var node in stack)
        //        {
        //            var nodeSymbol = node.Symbol;

        //            if (node.Production is null)
        //            {
        //                continue;
        //            }

        //            foreach (var item in node.Production.Body)
        //            {
        //                if (item == nodeSymbol)
        //                {
        //                    break;
        //                }
        //                if (item.IsTerminal)
        //                {
        //                    terminalFound = true;
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    var recursionType = terminalFound
        //        ? RecursionType.Normal
        //        : RecursionType.IndirectLeft;

        //    if (!terminalFound && stack.PeekTerminal() == recursiveNode)
        //    {
        //        recursionType = RecursionType.Left;
        //    }

        //    return new LL1GraphNode(
        //        symbol: symbol,
        //        production: symbolProduction,
        //        recursionType: recursionType,
        //        children: recursiveNode.Children
        //    );
        //}

        /*
            S -> A b.
            A -> B c.
            A -> D d.
            B -> A f.
            D -> d.

            Tree:
                 S
                b A
             B c D d
            A f A f

            Derivation:
            S
            A b
            A c b | B d b

        */


        if (symbol is NonTerminal nonTerminal)
        {
            var root = new LL1GraphNode(nonTerminal, symbolProduction);
            var productions = set
                .Lookup(nonTerminal)
                .ToArray();

            stack.Push(root);

            foreach (var production in productions)
            {
                var count = production.Body.Length;

                foreach (var bodySymbol in production.Body)
                {
                    var child = FromSymbol(
                        set: set,
                        symbol: bodySymbol,
                        stack: stack,
                        terminalProducers: terminalProducers,
                        symbolProduction: production
                    );

                    child.SetParent(root);
                    root.AddChild(child);
                }
            }

            stack.Pop();

            return root;
        }

        throw new InvalidOperationException($"The symbol {symbol} is not a terminal or non-terminal symbol.");
    }

    public static DerivationNode CreateDerivationTree(ProductionSet set)
    {
        return CreateDerivationTree(
            set: set, 
            sentence: new Sentence(set.Start),
            recursionStack: new(),
            production: null,
            derivedNonTerminal: null
        );
    }

    public static DerivationNode? CreateDerivationTree(
        ProductionSet set,
        Sentence sentence,
        Stack<NonTerminal> recursionStack,
        ProductionRule? production = null,
        NonTerminal? derivedNonTerminal = null)
    {
        var root = new DerivationNode(sentence, production, derivedNonTerminal);

        var leftmostNonTerminal = sentence.GetLeftmostNonTerminal();

        if (leftmostNonTerminal is null)
        {
            return null;
        }

        if (recursionStack.Contains(leftmostNonTerminal))
        {
            var epsilonProductions = set.Lookup(leftmostNonTerminal)
                .Where(x => x.Body == new Sentence(Epsilon.Instance))
                .ToArray()
                ;

            if (epsilonProductions.Length == 0)
            {
                throw new InvalidOperationException("The non-terminal does not have branch to end the recursion.");
            }

            var epsilonProduction = epsilonProductions.First();

            var derivation = sentence.DeriveLeftmostNonTerminal(epsilonProduction);

            return new DerivationNode(
                sentence: derivation.DerivedSentence,
                production: epsilonProduction,
                derivedNonTerminal: leftmostNonTerminal
            );
        }

        var nonTemrinalProductions = set
            .Lookup(leftmostNonTerminal)
            .ToArray();

        if (nonTemrinalProductions.Length == 0)
        {
            throw new InvalidOperationException($"The non-terminal {leftmostNonTerminal} has no nonTemrinalProductions.");
        }

        /*
            S -> A b.
            A -> B c.
            A -> D d.
            B -> A f.
            D -> d.

            Tree:
                 S
                A b
             B c D d
            A f A f

            Derivation:
            S
            A b
            A c b | B d b

        */
       
        foreach (var nonTerminalProduction in nonTemrinalProductions)
        {
            var derivation = sentence.DeriveLeftmostNonTerminal(nonTerminalProduction);
            var str = derivation.ToString();

            recursionStack.Push(leftmostNonTerminal);

            var child = CreateDerivationTree(
                set: set,
                sentence: derivation.DerivedSentence,
                recursionStack: recursionStack,
                production: nonTerminalProduction,
                derivedNonTerminal: leftmostNonTerminal
            );

            recursionStack.Pop();

            if (child is not null)
            {
                root.AddChild(child);
            }   
        }

        return root;
    }

}

public class DerivationNode
{
    public Sentence Sentence { get; }
    public ProductionRule? Production { get; }
    public NonTerminal? DerivedNonTerminal { get; }

    public DerivationNode? Parent { get; private set; }

    private List<DerivationNode> InternalChildren { get; }

    public DerivationNode(
        Sentence sentence,
        ProductionRule? production = null,
        NonTerminal? derivedNonTerminal = null)
    {
        Sentence = sentence;
        Production = production;
        DerivedNonTerminal = derivedNonTerminal;
        InternalChildren = new List<DerivationNode>();
    }

    public IReadOnlyList<DerivationNode> Children => InternalChildren;

    public int Position => DerivedNonTerminal is not null
        ? Sentence.IndexOfSymbol(DerivedNonTerminal)
        : -1;

    public override string ToString()
    {
        if(Production is not null && DerivedNonTerminal is not null)
        {
            return $"derived ({DerivedNonTerminal}) at {Position} in ({Sentence}) using ({Production})";
        }

        return Sentence.ToString();
    }

    public void SetParent(DerivationNode parent)
    {
        if (Parent is not null)
        {
            throw new InvalidOperationException("The parent is already set.");
        }

        Parent = parent;
    }

    public void AddChild(DerivationNode child)
    {
        if (child.Parent is not null)
        {
            throw new InvalidOperationException("The child already has a parent.");
        }

        child.SetParent(this);
        InternalChildren.Add(child);
    }

}
