using Aidan.Core;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Tools;
using Aidan.TextAnalysis.Parsing.LL1.Components;
using Aidan.TextAnalysis.Parsing.LL1.Tools;
using System.Text.Json;

namespace Aidan.TextAnalysis.Language.Extensions;

public static class ProductionSetAnalysisExtensions
{
    /*
        Analysis helpers.    
    */
    public static bool ContainsMacro(this ProductionSet set)
    {
        return set.Any(x => x.ContainsMacro());
    }

    public static bool ContainsUnitProduction(this ProductionSet set)
    {
        set.EnsureNoMacros();
        return set.Any(x => x.IsUnitProduction());
    }

    public static bool ContainsUnreachableProduction(this ProductionSet set)
    {
        set.EnsureNoMacros();
        return set.GetUnreachableProductions().Length != 0;
    }

    public static bool ContainsUnrealizableProduction(this ProductionSet set)
    {
        set.EnsureNoMacros();

        return new SymbolRealizabilityTool()
            .Execute(set).Length != 0;
    }

    public static IEnumerable<IGrouping<NonTerminal, ProductionRule>> GroupByHead(this ProductionSet set)
    {
        return set.GroupBy(x => x.Head);
    }

    public static Symbol[] GetUnreachableSymbols(this ProductionSet set)
    {
        set.EnsureNoMacros();

        return new SymbolReachabilityTool()
            .Execute(set);
    }

    public static ProductionRule[] GetUnreachableProductions(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var reachableSymbols = set.GetUnreachableSymbols();

        var unreachableProductions = set
            .Where(x => !reachableSymbols.Contains(x.Head))
            .ToArray();

        return unreachableProductions;
    }

    public static NonTerminal[] GetUnrealizableNonTerminals(this ProductionSet set)
    {
        set.EnsureNoMacros();

        return new SymbolRealizabilityTool()
            .Execute(set);
    }

    public static ProductionRule[] GetUnrealizableProductions(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var unrealizableNonTerminals = set.GetUnrealizableNonTerminals();
        var productions = new List<ProductionRule>();

        foreach (var nonTerminal in unrealizableNonTerminals)
        {
            productions.AddRange(set.Lookup(nonTerminal));
        }

        return productions.ToArray();
    }

    public static LL1LeftRecursionCicle[] GetLeftRecursionCicles(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var recursiveBranches = new LL1LeftRecursionTool()
            .Execute(set);

        var cicles = new List<LL1LeftRecursionCicle>();

        foreach (var branch in recursiveBranches)
        {
            var derivations = new List<Derivation>();

            foreach (var node in branch.Nodes)
            {
                if (node.Production is null)
                {
                    continue;
                }
                if (node.Symbol is not NonTerminal nonTerminal)
                {
                    throw new InvalidOperationException("The symbol is not a nonterminal.");
                }

                var originalSentence = new Sentence();

                if (node.Parent?.Production is not null)
                {
                    originalSentence = originalSentence.Add(node.Parent.Production.Value.Body);
                }

                var derivedSentence = new Sentence(node.Production.Value.Body);

                var derivation = new Derivation(
                    production: node.Production.Value,
                    nonTerminal: nonTerminal,
                    originalSentence: originalSentence,
                    derivedSentence: derivedSentence
                );

                derivations.Add(derivation);
            }

            cicles.Add(new LL1LeftRecursionCicle(derivations));
        }

        return cicles.ToArray();
    }

    public static ProductionSubset[] GetSubsetsGroupedByCommonTerminalLeftFactor(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var subsets = new List<ProductionSubset>();

        var nonTerminalProductions = set
            .GroupBy(x => x.Head)
            .ToArray();

        foreach (var group in nonTerminalProductions)
        {
            var commonPrefixProductions = group
                .Where(x => x.Body.Length > 0)
                .Where(x => x.Body[0].IsTerminal)
                .GroupBy(x => x.Body.First())
                .Where(x => x.Count() > 1)
                .SelectMany(x => x.ToArray())
                .ToArray();

            if (commonPrefixProductions.Length == 0)
            {
                continue;
            }

            subsets.Add(set.CreateSubset(commonPrefixProductions));
        }

        return subsets.ToArray();
    }

    public static ProductionSubset[] GetSubsetsGroupedByCommonNonTerminalLeftFactor(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var subsets = new List<ProductionSubset>();

        var nonTerminalProductions = set
            .GroupBy(x => x.Head)
            .ToArray();

        foreach (var group in nonTerminalProductions)
        {
            var commonPrefixProductions = group
                .Where(x => x.Body.Length > 0)
                .Where(x => x.Body[0].IsNonTerminal)
                .GroupBy(x => x.Body.First())
                .Where(x => x.Count() > 1)
                .SelectMany(x => x.ToArray())
                .ToArray();

            if (commonPrefixProductions.Length == 0)
            {
                continue;
            }

            subsets.Add(set.CreateSubset(commonPrefixProductions));
        }

        return subsets.ToArray();
    }

    public static ProductionSubset[] GetSubsetsGroupedByFirstSetConflicts(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var subsets = new List<ProductionSubset>();

        var productionsGroup = set
            .GroupByHead()
            .ToArray();

        for (int i = 0; i < productionsGroup.Length; i++)
        {
            var overlappingProductions = new List<ProductionRule>();
            var group = productionsGroup[i];

            var firstSets = group
                .Select(x => x.ComputeFirstSet(set))
                .ToArray();

            var overlappingSets = firstSets
                .Where(firstSet1 => firstSets.Any(firstSet2 => !ReferenceEquals(firstSet1, firstSet2) && firstSet1.Overlaps(firstSet2)))
                .ToArray();

            if (overlappingSets.Length < 2)
            {
                continue;
            }

            var indexes = overlappingSets
                .Select(x => Array.IndexOf(firstSets, x))
                .ToArray();

            var _overlappingProductions = group
                .Where((_, index) => indexes.Contains(index))
                .ToArray();

            foreach (var production in _overlappingProductions)
            {
                if (!overlappingProductions.Contains(production))
                {
                    overlappingProductions.Add(production);
                }
            }

            subsets.Add(set.CreateSubset(_overlappingProductions));
        }

        return subsets.ToArray();
    }

    public static ProductionSubset[] GetSubsetsGroupedByFirstFollowConflicts(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var subsets = new List<ProductionSubset>();

        foreach (var production in set)
        {
            if (production.Body.Length < 1)
            {
                continue;
            }

            if (production.Body[0] is not NonTerminal nonTerminal)
            {
                continue;
            }

            if (set.Lookup(nonTerminal).All(x => !x.IsEpsilonProduction()))
            {
                continue;
            }

            var firstSet = LL1FirstSetTool.ComputeFirstSet(set, nonTerminal);
            var followSet = LL1FollowSetTool.ComputeFollowSet(set, nonTerminal);

            if (firstSet.Overlaps(followSet))
            {
                subsets.Add(set.CreateSubset(set.Lookup(nonTerminal)));
            }
        }

        return subsets.ToArray();
    }

    public static NonTerminal[] GetCommonPrefixProductionHeads(this ProductionSet set)
    {
        set.EnsureNoMacros();

        var nonTerminalProductions = set
            .GroupBy(x => x.Head)
            .ToArray();

        var heads = new List<NonTerminal>();

        foreach (var group in nonTerminalProductions)
        {
            var commonPrefixProductionsGroup = group
                .Where(x => x.Body.Length > 1)
                .GroupBy(x => x.Body.First())
                .Where(x => x.Count() > 1)
                .SelectMany(x => x.ToArray())
                .ToArray();

            if (commonPrefixProductionsGroup.Length == 0)
            {
                continue;
            }

            /*
                This part gets the full set of productionsGroup for the given non-terminal.
                The alternative productionsGroup must be preserved.
            */

            var head = commonPrefixProductionsGroup.First().Head;

            if (!heads.Contains(head))
            {
                heads.Add(head);
            }
        }

        return heads.ToArray();
    }

    public static LL1FirstTable ComputeFirstTable(this ProductionSet set)
    {
        set.EnsureNoMacros();
        return LL1FirstSetTool.ComputeFirstTable(set);
    }

    public static LL1FollowTable ComputeFollowTable(this ProductionSet set)
    {
        set.EnsureNoMacros();
        return LL1FollowSetTool.ComputeFollowTable(set);
    }

    public static LL1FirstSetConflict[] ComputeFirstSetConflicts(this ProductionSet set)
    {
        set.EnsureNoMacros();
        return LL1FirstSetConflictTool.ComputeFirstSetConflicts(set);
    }

    /*
     * LR1 Analysis helpers.
     */

    public static ProductionRule? TryGetAugmentedStartProduction(this ProductionSet set)
    {
        var startProductions = set.Lookup(set.Start).ToArray();

        if (startProductions.Length != 1)
        {
            return null;
        }

        var startProduction = startProductions[0];

        if (startProduction.Body.Length != 1)
        {
            return null;
        }
        if (startProduction.Body[0] is not NonTerminal nonTerminal)
        {
            return null;
        }

        return startProduction;
    }

    public static ProductionRule GetAugmentedStartProduction(this ProductionSet set)
    {
        var production = set.TryGetAugmentedStartProduction();

        if (production is null)
        {
            throw new InvalidOperationException("The production set does not have an augmented production.");
        }

        return production.Value;
    }

    public static bool IsAugmented(this ProductionSet set)
    {
        return TryGetAugmentedStartProduction(set) is not null;
    }

    public static void EnsureAugmented(this ProductionSet set)
    {
        if (!set.IsAugmented())
        {
            throw new InvalidOperationException("The production set is not augmented.");
        }
    }

    public static int GetProductionIndex(this ProductionSet set, ProductionRule production)
    {
        var index = -1;

        for (int i = 0; i < set.Length; i++)
        {
            if (set[i] == production)
            {
                if (index != -1)
                {
                    throw new InvalidOperationException("The production set contains duplicate productions.");
                }

                index = i;
                break;
            }
        }

        return index;
    }

    /*
        Assertion helpers.
    */
    public static void EnsureNoMacros(this ProductionSet set)
    {
        if (set.ContainsMacro())
        {
            throw new InvalidOperationException("The production set contains macros.");
        }
    }

    public static Error[] GetLL1Errors(this ProductionSet set)
    {
        var errors = new List<Error>();

        var unrealizableProductions = set.GetUnrealizableProductions();

        if (unrealizableProductions.Length != 0)
        {
            var unrealizableProductionsStr = string.Join(Environment.NewLine, unrealizableProductions.Select(x => x.ToString()));

            var error = new ErrorBuilder()
                .SetTitle("The grammar contains unrealizable productionsGroup.")
                .AddDetail("The unrealizable productionsGroup", unrealizableProductionsStr)
                .AddDetail("The production set", set.ToString())
                .Build();
                ;

            errors.Add(error);
        }

        var unreachableProductions = set.GetUnreachableProductions();

        if (unreachableProductions.Length != 0)
        {
            var unreachableProductionsStr = string.Join(Environment.NewLine, unreachableProductions.Select(x => x.ToString()));

            var error = new ErrorBuilder()
                .SetTitle("The grammar contains unreachable symbols.")
                .AddDetail("The unreachable productionsGroup", unreachableProductionsStr)
                .AddDetail("The production set", set.ToString())
                .Build();
                ;

            errors.Add(error);
        }

        var leftRecursiveCicles = set.GetLeftRecursionCicles();

        if (leftRecursiveCicles.Length != 0)
        {
            var ciclesStr = string.Join(Environment.NewLine, leftRecursiveCicles.Select(x => x.ToString()));
            var leftRecursiveCiclesStr = string.Join(Environment.NewLine, leftRecursiveCicles.Select(x => x.ToString()));

            var error = new ErrorBuilder()
                .SetTitle("The grammar contains left recursion cicles.")
                .AddDetail("The left recursion cicles", ciclesStr)
                .AddDetail("The production set", set.ToString())
                .AddDetail("The left recursion cicles", leftRecursiveCiclesStr)
                .Build()
                ;

            errors.Add(error);
        }

        var firstSetConflictingProductionSubsets = set.GetSubsetsGroupedByFirstSetConflicts();

        if (firstSetConflictingProductionSubsets.Length != 0)
        {
            var subsetsStr = string.Join(Environment.NewLine, firstSetConflictingProductionSubsets.Select(x => x.ToString()));

            var error = new ErrorBuilder()
                .SetTitle("The grammar contains first set conflicts.")
                .AddDetail("The first set conflicting production subsets", subsetsStr)
                .AddDetail("The production set", set.ToString())
                .Build()
                ;

            errors.Add(error);
        }

        var firstFollowConflictingProductionSubsets = set.GetSubsetsGroupedByFirstFollowConflicts();

        if (firstFollowConflictingProductionSubsets.Length != 0)
        {
            var subsetsStr = string.Join(Environment.NewLine, firstFollowConflictingProductionSubsets.Select(x => x.ToString()));

            var error = new ErrorBuilder()
                .SetTitle("The grammar contains first-follow set conflicts.")
                .AddDetail("The first-follow set conflicting production subsets", subsetsStr)
                .AddDetail("The production set", set.ToString())
                .Build()
                ;

            errors.Add(error);
        }

        return errors.ToArray();
    }

    public static Error[] GetLR1Errors(this ProductionSet set)
    {
        var errors = new List<Error>();

        var unrealizableProductions = set.GetUnrealizableProductions();

        if (unrealizableProductions.Length != 0)
        {
            var str = string.Join(Environment.NewLine, unrealizableProductions.Select(x => x.ToString()));

            var error = new ErrorBuilder()
                .SetTitle("The grammar contains unrealizable productions.")
                .AddDetail("The unrealizable productions", str)
                .AddDetail("The production set", set.ToString())
                .Build();
                ;

            errors.Add(error);
        }

        var unreachableProductions = set.GetUnreachableProductions();

        if (unreachableProductions.Length != 0)
        {
            var str = string.Join(Environment.NewLine, unreachableProductions.Select(x => x.ToString()));

            var error = new ErrorBuilder()
                .SetTitle("The grammar contains unreachable symbols.")
                .AddDetail("The unreachable productions", str)
                .AddDetail("The production set", set.ToString())
                .Build();
                ;

            errors.Add(error);
        }

        return errors.ToArray();
    }
}


