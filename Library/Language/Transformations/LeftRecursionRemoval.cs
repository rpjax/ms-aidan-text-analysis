using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;
using Aidan.TextAnalysis.Parsing.LL1.Tools;

namespace Aidan.TextAnalysis.Language.Transformations;

public class LeftRecursionRemoval : ISetTransformer
{
    public SetTransformationCollection ExecuteTransformations(ProductionSet set)
    {
        var recursiveBranches = new LL1LeftRecursionTool()
            .Execute(set);

        set.EnsureNoMacros();
        set.ResetTransformationsTracker();

        while (recursiveBranches.Length > 0)
        {
            foreach (var branch in recursiveBranches)
            {
                if (branch.Root.Symbol is not NonTerminal rootSymbol)
                {
                    throw new InvalidOperationException("The root symbol is not a nonterminal.");
                }

                //var recursiveProduction = branch.Nodes
                //    .First(node => node.Production is not null && node.Production.Body.First() == branch.Kernel_.Symbol).Production;

                //var recursiveProduction = branch.Nodes
                //    .Skip(1)
                //    .Where(x => x.Production is not null)
                //    .Select(x => x.Production!)
                //    .ToArray();

                //var recursiveProduction = branch.Nodes
                //    .Skip(1)
                //    .Select(x => x.Production!)
                //    .First();

                var recursiveProduction = branch.Nodes.Last().Production;

                if (recursiveProduction is null)
                {
                    throw new InvalidOperationException("The recursive production is null.");
                }

                var recursiveSymbol = recursiveProduction.Value.Head;

                /*
                 * get all the productions of the root symbol that don't start with the recursive symbol. 
                 * remove the original recursive production.
                 * replace the recursive production with a new set productions, replacing the root symbol with the body of the productions
                 * 
                 * Ex:
                 * S -> A b 
                 * A -> B c.
                 * D -> S e.
                 * 
                 * Turns to:
                 * S -> A b.
                 * A -> B c.
                 * D -> A b e. (D -> S e. replaced with D -> A b e.)
                 */

                var productions = set.Lookup(rootSymbol)
                    .Where(production => production.Body.First() != recursiveSymbol)
                    .ToArray();

                if (productions.Length == 0)
                {
                    throw new InvalidOperationException("The grammar is not in the correct form.");
                }

                var suffix = recursiveProduction.Value.Body.Skip(1).ToArray();
                var builder = set.GetTransformationBuilder("Left Recursion Removal");

                foreach (var item in productions)
                {
                    var newProduction = new ProductionRule(
                        head: recursiveSymbol,
                        body: item.Body.Concat(suffix).ToArray()
                    );

                    builder.AddProductions(newProduction);
                }

                builder.RemoveProductions(recursiveProduction.Value);
                builder.Build();
            }

            recursiveBranches = new LL1LeftRecursionTool()
                .Execute(set);
        }

        return set.GetTrackedTransformations();
    }

}
