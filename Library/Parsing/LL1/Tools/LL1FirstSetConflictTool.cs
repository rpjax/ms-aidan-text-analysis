using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;
using Aidan.TextAnalysis.Parsing.LL1.Components;

namespace Aidan.TextAnalysis.Parsing.LL1.Tools;

public class LL1FirstSetConflictTool
{
    public static LL1FirstSetConflict[] ComputeFirstSetConflicts(ProductionSet set)
    {
        var firstTable = set.ComputeFirstTable();

        var clashes = new List<LL1FirstSetConflict>();

        foreach (var production in set)
        {
            var firstSet = production.ComputeFirstSet(set);
            var firsts = firstSet.Firsts;

            foreach (var symbol in firsts)
            {
                if (symbol is Terminal terminal)
                {
                    var clashingSymbols = firsts
                        .GroupBy(x => x)
                        .Where(x => x.Count() > 1)
                        .Select(x => x.Key)
                        .ToArray();

                    if (clashingSymbols.Length != 0)
                    {
                        clashes.Add(new LL1FirstSetConflict(production.Head, clashingSymbols));
                    }
                }
            }
        }

        return clashes.ToArray();
    }
}
