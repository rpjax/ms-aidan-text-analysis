using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;

namespace Aidan.TextAnalysis.Language.Transformations;

public class UnitProductionExpansion : ISetTransformer
{
    public SetTransformationCollection ExecuteTransformations(ProductionSet set)
    {
        var ignoreSet = new List<ProductionRule>();

        set.EnsureNoMacros();
        set.ResetTransformationsTracker();

        foreach (var production in set.Copy())
        {
            if (!production.IsUnitProduction())
            {
                continue;
            }
            if (ignoreSet.Contains(production))
            {
                continue;
            }
            if (production.Body[0] is not NonTerminal nonTerminal)
            {
                throw new InvalidOperationException("The body of the production is not a nonterminal.");
            }

            var unitProduction = production;
            var unitHead = unitProduction.Head;
            var unitBody = nonTerminal;
            var replacementProductions = set.Lookup(unitBody).ToArray();
            var newProductions = new List<ProductionRule>();

            if(replacementProductions.Length == 0)
            {
                throw new InvalidOperationException("The nonterminal does not have any productions.");
            }

            if (unitHead == unitBody)
            {
                ignoreSet.Add(production);
                continue;
            }

            var skip = false;

            foreach (var replacementProduction in replacementProductions)
            {
                var newProduction = new ProductionRule(unitHead, replacementProduction.Body);

                if (newProduction == unitProduction)
                {
                    skip = true;
                    ignoreSet.Add(production);
                    break;
                }

                newProductions.Add(newProduction);
            }

            if (skip)
            {
                continue;
            }

            set.GetTransformationBuilder($"Unit Production Expansion")
                .RemoveProductions(unitProduction)
                .AddProductions(newProductions.ToArray())
                .Build()
                ;
        }

        return set.GetTrackedTransformations();
    }
}
