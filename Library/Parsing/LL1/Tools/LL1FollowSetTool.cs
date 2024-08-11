using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;
using Aidan.TextAnalysis.Parsing.LL1.Components;

namespace Aidan.TextAnalysis.Parsing.LL1.Tools;

public class LL1FollowSetTool
{
    public static LL1FollowSet ComputeFollowSet(ProductionSet set, NonTerminal symbol)
    {
        var productions = set
            .Where(x => x.Body.Any(x => x == symbol))
            .ToArray();

        var follows = new List<Symbol>();

        foreach (var production in productions)
        {
            var sentence = new Sentence(production.Body);
            var indexes = sentence.GetIndexesOfNonTerminal(symbol);
            var head = production.Head;

            foreach (var index in indexes)
            {
                var isLast = index + 1 >= sentence.Length;

                if (isLast)
                {
                    if (head != symbol)
                    {
                        follows.AddRange(ComputeFollowSet(set, head).Follows);
                    }

                    continue;
                }

                if (sentence[index + 1] is Terminal terminal)
                {
                    follows.Add(terminal);
                    continue;
                }
                if (sentence[index + 1] is not NonTerminal nonTerminal)
                {
                    throw new InvalidOperationException("The symbol is not a nonterminal.");
                }

                var firstSet = LL1FirstSetTool.ComputeFirstSet(set, nonTerminal);

                follows.AddRange(firstSet.Firsts);

                if (!firstSet.Firsts.Contains(Epsilon.Instance))
                {
                    continue;
                }

                var currentNonTerminal = nonTerminal;
                var currentIndex = index + 1;

                /*
                    S -> aBC
                    B -> b | ε
                    C -> c | ε
                */

                while (true)
                {
                    firstSet = LL1FirstSetTool.ComputeFirstSet(set, currentNonTerminal);
                    follows.AddRange(firstSet.Firsts);

                    if (!firstSet.Firsts.Contains(Epsilon.Instance))
                    {
                        break;
                    }

                    if (currentIndex + 1 >= sentence.Length)
                    {
                        if (head != symbol)
                        {
                            follows.AddRange(ComputeFollowSet(set, head).Follows);
                        }

                        break;
                    }

                    if (sentence[currentIndex + 1] is Terminal nextTerminal)
                    {
                        follows.Add(nextTerminal);
                        break;
                    }
                    if (sentence[currentIndex + 1] is not NonTerminal nextNonTerminal)
                    {
                        throw new InvalidOperationException("The symbol is not a nonterminal.");
                    }

                    currentNonTerminal = nextNonTerminal;
                    currentIndex++;
                }
            }

        }

        if (symbol == set.Start)
        {
            follows.Add(Eoi.Instance);
        }

        follows.RemoveAll(x => x == Epsilon.Instance);

        return new LL1FollowSet(symbol, follows.Distinct().ToArray());
    }

    public static LL1FollowTable ComputeFollowTable(ProductionSet set)
    {
        var followSets = new List<LL1FollowSet>();

        foreach (var nonTerminal in set.GetNonTerminals())
        {
            followSets.Add(ComputeFollowSet(set, nonTerminal));
        }

        return new LL1FollowTable(followSets.ToArray());
    }

}

