using System.Collections;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

public class LR1ItemCollection : 
    IEnumerable<LR1Item>,
    IEquatable<IEnumerable<LR1Item>>
{
    public LR1Item[] Items { get; }

    public LR1ItemCollection(LR1Item[] items)
    {
        Items = items;
    }

    public LR1Item this[int index] => Items[index];

    public int Length => Items.Length;

    public IEnumerator<LR1Item> GetEnumerator()
    {
        return ((IEnumerable<LR1Item>)Items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override int GetHashCode()
    {
        unchecked 
        {
            int hash = 17;

            foreach (var item in Items)
            {
                hash = hash * 23 + item.GetHashCode();
            }

            return hash;
        }
    }

    public bool Equals(IEnumerable<LR1Item>? other)
    {
        var otherItems = other?.ToArray();

        if (otherItems is null)
        {
            return false;
        }

        if(Items.Length != otherItems.Length)
        {
            return false;
        }

        for (int i = 0; i < Items.Length; i++)
        {
            if (!Items[i].Equals(otherItems[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as IEnumerable<LR1Item>);
    }

}
