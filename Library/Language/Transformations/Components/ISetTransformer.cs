using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Language.Transformations;

public interface ISetTransformer
{
    SetTransformationCollection ExecuteTransformations(ProductionSet set);
}
