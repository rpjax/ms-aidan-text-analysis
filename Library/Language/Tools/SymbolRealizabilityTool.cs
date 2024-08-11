using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;

namespace Aidan.TextAnalysis.Language.Tools;

public class SymbolRealizabilityTool
{
    public NonTerminal[] Execute(ProductionSet set)
    {
        var realizableSymbols = new List<Symbol>();
        var progress = false;
        var workingSet = set.ToList();

        while (true)
        {
            foreach (var production in workingSet.ToArray())
            {
                if (realizableSymbols.Contains(production.Head))
                {
                    workingSet.Remove(production);
                    continue;
                }

                var isRealizable = production.Body
                    .All(x => x.IsTerminal || realizableSymbols.Contains(x));

                if (isRealizable)
                {
                    realizableSymbols.Add(production.Head);
                    progress = true;
                    workingSet.Remove(production);
                }
            }

            if (!progress)
            {
                break;
            }

            progress = false;
        }

        var unrealizableSymbols = set
            .Select(x => x.Head)
            .Distinct()
            .Except(realizableSymbols)
            .Select(x => x.AsNonTerminal())
            .ToArray();

        return unrealizableSymbols;
    }

}
