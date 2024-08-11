using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;

namespace Aidan.TextAnalysis.Language.Transformations;

public class DuplicateProductionsRemoval : ISetTransformer
{
    public SetTransformationCollection ExecuteTransformations(ProductionSet set)
    {
        var ignoreSet = new List<ProductionRule>();

        set.EnsureNoMacros();
        set.ResetTransformationsTracker();

        var duplicates = set
            .GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .SelectMany(x => x.Skip(1))
            .ToArray();

        foreach (var duplicate in duplicates)
        {
            set.GetTransformationBuilder("Duplicate Production Removal")
                .RemoveProductions(duplicate)
                .AddProductions(duplicate)
                .Build();
        }

        return set.GetTrackedTransformations();
    }
}
