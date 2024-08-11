using System.Collections;

namespace Aidan.TextAnalysis.Language.Components;

public struct ProductionGroup : IEnumerable<ProductionSet>
{
    private ProductionSet[] Sets { get; }

    public ProductionGroup(params ProductionSet[] productions)
    {
        Sets = productions;
    }

    public int Length => Sets.Length;

    public ProductionSet this[int index]
    {
        get => Sets[index];
    }

    public static implicit operator ProductionSet[](ProductionGroup group)
    {
        return group.Sets;
    }

    public static implicit operator ProductionGroup(ProductionSet[] productions)
    {
        return new ProductionGroup(productions);
    }

    public static implicit operator ProductionGroup(List<ProductionSet> productions)
    {
        return new ProductionGroup(productions.ToArray());
    }

    public IEnumerator<ProductionSet> GetEnumerator()
    {
        return ((IEnumerable<ProductionSet>)Sets).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Sets.GetEnumerator();
    }

    public ProductionGroup Copy()
    {
        /*
            Sets are not immutable, so a deep copy is required.
        */
        return new ProductionGroup(Sets.Select(x => x.Copy()).ToArray());
    }

}
