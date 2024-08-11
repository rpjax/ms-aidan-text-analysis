using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;

namespace Aidan.TextAnalysis.Language.Transformations;

public class RemoveUnreachableProductions : ISetTransformer
{
    public SetTransformationCollection ExecuteTransformations(ProductionSet set)
    {
        var reachable = new HashSet<NonTerminal>();
        var visited = new HashSet<NonTerminal>();
        var start = set.Start;

        set.EnsureNoMacros();
        set.ResetTransformationsTracker();

        if (start is null)
        {
            throw new InvalidOperationException("The start symbol is not set.");
        }

        reachable.Add(start);

        void Visit(NonTerminal nonTerminal)
        {
            if (visited.Contains(nonTerminal))
            {
                return;
            }

            visited.Add(nonTerminal);

            foreach (var production in set.Lookup(nonTerminal))
            {
                foreach (var symbol in production.Body)
                {
                    if (symbol is NonTerminal nt)
                    {
                        reachable.Add(nt);
                        Visit(nt);
                    }
                }
            }
        }

        Visit(start);

        var unreachableProductions = set.Copy()
            .Where(x => !reachable.Contains(x.Head));

        foreach (var production in unreachableProductions)
        {
            set.GetTransformationBuilder($"Unreachable Production Removal")
                .RemoveProductions(production)
                .Build();
        }

        return set.GetTrackedTransformations();
    }
}
