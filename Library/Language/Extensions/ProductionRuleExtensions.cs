using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Parsing.LL1.Components;
using Aidan.TextAnalysis.Parsing.LL1.Tools;

namespace Aidan.TextAnalysis.Language.Extensions;

public static class ProductionRuleExtensions
{
    public static bool IsEpsilonProduction(this ProductionRule production)
    {
        return production.Body.Length == 1
            && production.Body[0] == Epsilon.Instance;
    }

    public static Symbol? GetLefmostSymbol(this ProductionRule production)
    {
        return production.Body
            .FirstOrDefault();
    }

    public static Symbol? GetRightmostSymbol(this ProductionRule production)
    {
        return production.Body
            .LastOrDefault();
    }

    public static NonTerminal? GetLeftmostNonTerminal(this ProductionRule production)
    {
        return production.Body
            .OfType<NonTerminal>()
            .FirstOrDefault();
    }

    public static NonTerminal? GetRightmostNonTerminal(this ProductionRule production)
    {
        return production.Body
            .OfType<NonTerminal>()
            .LastOrDefault();
    }

    public static Terminal? GetTerminalPrefix(this ProductionRule production)
    {
        return production.Body
            .OfType<Terminal>()
            .FirstOrDefault();
    }

    public static int IndexOfSymbol(this ProductionRule production, Symbol symbol)
    {
        var index = -1;

        foreach (var item in production.Body)
        {
            index++;

            if (ReferenceEquals(item, symbol))
            {
                break;
            }
        }

        return index;
    }

    public static bool IsLeftRecursive(this ProductionRule production)
    {
        return GetLefmostSymbol(production) is NonTerminal nonTerminal
            && nonTerminal == production.Head;
    }

    public static bool IsRightRecursive(this ProductionRule production)
    {
        return GetRightmostSymbol(production) is NonTerminal nonTerminal
            && nonTerminal == production.Head;
    }

    public static bool IsUnitProduction(this ProductionRule production)
    {
        return production.Body.Length == 1
            && production.Body[0].IsNonTerminal;
    }

    public static LL1FirstSet ComputeFirstSet(this ProductionRule production, ProductionSet set)
    {
        return LL1FirstSetTool.ComputeFirstSet(set, production);
    }

    public static LL1FollowSet ComputeFollowSet(this ProductionRule production, ProductionSet set)
    {
        return LL1FollowSetTool.ComputeFollowSet(set, production.Head);
    }

}
