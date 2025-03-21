﻿using System.Collections;
using Aidan.TextAnalysis.Helpers;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents an immutable collection of LR(1) items.
/// </summary>
public class LR1ItemCollection :
    IEnumerable<LR1Item>,
    IEquatable<LR1ItemCollection>
{
    /// <summary>
    /// Gets the LR(1) items in the collection.
    /// </summary>
    public LR1Item[] Items { get; }

    /// <summary>
    /// Gets the number of items in the collection.
    /// </summary>
    public int Length => Items.Length;

    /// <summary>
    /// Initializes a new instance of the <see cref="LR1ItemCollection"/> class with the specified items.
    /// </summary>
    /// <param name="items">The LR(1) items to include in the collection.</param>
    public LR1ItemCollection(LR1Item[] items)
    {
        Items = items;
    }

    /// <summary>
    /// Gets the LR(1) item at the specified index.
    /// </summary>
    /// <param name="index">The index of the item to get.</param>
    /// <returns>The LR(1) item at the specified index.</returns>
    public LR1Item this[int index] => Items[index];

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator for the collection.</returns>
    public IEnumerator<LR1Item> GetEnumerator()
    {
        return ((IEnumerable<LR1Item>)Items).GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator for the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Determines whether the specified collection of LR(1) items is equal to the current collection.
    /// </summary>
    /// <param name="other">The collection to compare with the current collection.</param>
    /// <returns>true if the specified collection is equal to the current collection; otherwise, false.</returns>
    public bool Equals(LR1ItemCollection? other)
    {
        return other is not null
            && other.GetHashCode() == GetHashCode();
    }

    /// <summary>
    /// Returns a hash code for the collection.
    /// </summary>
    /// <returns>A hash code for the collection.</returns>
    public override int GetHashCode()
    {
        return HashHelper.ComputeHash(Items);
    }

}
