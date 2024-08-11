using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Parsing.LL1.Components;

namespace Aidan.TextAnalysis.Parsing.LL1.Tools;

public class LL1FirstSetTool
{
    public static LL1FirstSet ComputeFirstSet(ProductionSet set, NonTerminal symbol)
    {
        var derivations = set.Lookup(symbol).ToArray();
        var firsts = new List<Symbol>();

        foreach (var derivation in derivations)
        {
            var sentence = new Sentence(derivation.Body);

            if (sentence.Length == 0)
            {
                throw new InvalidOperationException("Empty sentence in production rule.");
            }

            if (sentence[0] is Terminal terminal)
            {
                firsts.Add(terminal);
                continue;
            }

            if (sentence[0] is Epsilon epsilon)
            {
                firsts.Add(epsilon);
                continue;
            }

            if (sentence[0] is not NonTerminal nonTerminal)
            {
                throw new InvalidOperationException("The symbol is not a nonterminal.");
            }

            var target = null as NonTerminal;
            var index = 0;

            while (true)
            {
                target = sentence[index] as NonTerminal;

                if (target is null)
                {
                    throw new InvalidOperationException("The symbol is not a nonterminal.");
                }

                var firstSet = ComputeFirstSet(set, target);

                firsts.AddRange(firstSet.Firsts);

                if (!firstSet.Firsts.Contains(Epsilon.Instance))
                {
                    break;
                }

                firsts.Remove(Epsilon.Instance);

                if (index + 1 >= sentence.Length)
                {
                    break;
                }

                index++;
            }
        }

        return new LL1FirstSet(symbol, firsts.Distinct().ToArray());
    }

    public static LL1FirstSet ComputeFirstSet(ProductionSet set, ProductionRule production)
    {
        var firsts = new List<Symbol>();
        var sentence = new Sentence(production.Body);

        if (sentence.Length == 0)
        {
            throw new InvalidOperationException("Empty sentence in production rule.");
        }

        if (sentence[0].IsTerminal)
        {
            firsts.Add(sentence[0]);
            return new LL1FirstSet(production.Head, firsts.ToArray());
        }

        if (sentence[0] is not NonTerminal)
        {
            throw new InvalidOperationException("The symbol is not a nonterminal.");
        }

        var target = null as NonTerminal;
        var index = 0;

        while (true)
        {
            target = sentence[index] as NonTerminal;

            if (target is null)
            {
                throw new InvalidOperationException("The symbol is not a nonterminal.");
            }

            var firstSet = ComputeFirstSet(set, target);

            firsts.AddRange(firstSet.Firsts);

            if (!firstSet.Firsts.Contains(Epsilon.Instance))
            {
                break;
            }

            firsts.Remove(Epsilon.Instance);

            if (index + 1 >= sentence.Length)
            {
                break;
            }

            index++;
        }

        return new LL1FirstSet(production.Head, firsts.ToArray());
    }

    public static LL1FirstTable ComputeFirstTable(ProductionSet set)
    {
        var firstSets = new List<LL1FirstSet>();

        foreach (var nonTerminal in set.GetNonTerminals())
        {
            firstSets.Add(ComputeFirstSet(set, nonTerminal));
        }

        return new LL1FirstTable(firstSets.ToArray());
    }

}

