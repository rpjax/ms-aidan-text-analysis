using System.Collections;

namespace Aidan.TextAnalysis.Language.Components;

public class ProductionSubset : 
    IEnumerable<ProductionRule>
{
    public ProductionSet MainSet { get; }
    public ProductionRule[] Productions { get; }

    public ProductionSubset(ProductionSet mainSet, ProductionRule[] productions)
    {
        MainSet = mainSet;
        Productions = productions;
    }

    public int Length => Productions.Length;

    public IEnumerator<ProductionRule> GetEnumerator()
    {
        return ((IEnumerable<ProductionRule>)Productions).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<ProductionRule>)Productions).GetEnumerator();
    }

    public override string ToString()
    {
        return string.Join("\n", Productions.Select(x => x.ToString()));
    }
}
