using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents an LR(1) item.
/// </summary>
public class LR1Item :
    IEquatable<LR1Item>
{
    /// <summary>
    /// Gets the production rule of the LR(1) item.
    /// </summary>
    public IProductionRule Production { get; }

    /// <summary>
    /// Gets the position of the LR(1) item.
    /// </summary>
    public int Position { get; }

    /// <summary>
    /// Gets the lookaheads of the LR(1) item.
    /// </summary>
    public ITerminal[] Lookaheads { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LR1Item"/> class.
    /// </summary>
    /// <param name="production">The production rule.</param>
    /// <param name="position">The position in the production rule.</param>
    /// <param name="lookaheads">The lookaheads for the item.</param>
    public LR1Item(IProductionRule production, int position, params ITerminal[] lookaheads)
    {
        Production = production;
        Position = position;
        Lookaheads = lookaheads;
    }

    /// <summary>
    /// Determines whether two <see cref="LR1Item"/> instances are equal.
    /// </summary>
    /// <param name="left">The left instance.</param>
    /// <param name="right">The right instance.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(LR1Item left, LR1Item right)
    {
        return left.GetSignature(useLookaheads: true) == right.GetSignature(useLookaheads: true);
    }

    /// <summary>
    /// Determines whether two <see cref="LR1Item"/> instances are not equal.
    /// </summary>
    /// <param name="left">The left instance.</param>
    /// <param name="right">The right instance.</param>
    /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(LR1Item left, LR1Item right)
    {
        return left.GetSignature(useLookaheads: true) != right.GetSignature(useLookaheads: true);
    }

    /// <summary>
    /// Gets the symbol at the current position in the production rule.
    /// </summary>
    public ISymbol? Symbol => Position < Production.Body.Length
        ? Production.Body[Position]
        : null;

    /// <summary>
    /// Gets the signature of the LR(1) item.
    /// </summary>
    public string Signature => GetSignature(useLookaheads: true);

    /// <summary>
    /// Determines whether the specified <see cref="LR1Item"/> is equal to the current <see cref="LR1Item"/>.
    /// </summary>
    /// <param name="other">The <see cref="LR1Item"/> to compare with the current <see cref="LR1Item"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="LR1Item"/> is equal to the current <see cref="LR1Item"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(LR1Item? other)
    {
        return other is not null
            && other == this;
    }

    /// <summary>
    /// Determines whether the specified <see cref="LR1Item"/> instances are equal.
    /// </summary>
    /// <param name="left">The left instance.</param>
    /// <param name="right">The right instance.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(LR1Item? left, LR1Item? right)
    {
        return (left is not null && right is not null)
            && left == right;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="LR1Item"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current <see cref="LR1Item"/>.</param>
    /// <returns><c>true</c> if the specified object is equal to the current <see cref="LR1Item"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as LR1Item);
    }

    /// <summary>
    /// Returns the hash code for the specified <see cref="LR1Item"/>.
    /// </summary>
    /// <param name="obj">The <see cref="LR1Item"/>.</param>
    /// <returns>The hash code for the specified <see cref="LR1Item"/>.</returns>
    public int GetHashCode([DisallowNull] LR1Item obj)
    {
        unchecked
        {
            var hash = 17;
            hash = hash * 23 + Production.GetHashCode();
            hash = hash * 23 + Position.GetHashCode();

            foreach (var lookahead in Lookaheads)
            {
                hash = hash * 23 + lookahead.GetHashCode();
            }

            return hash;
        }
    }

    /// <summary>
    /// Returns the hash code for the current <see cref="LR1Item"/>.
    /// </summary>
    /// <returns>The hash code for the current <see cref="LR1Item"/>.</returns>
    public override int GetHashCode()
    {
        return GetHashCode(this);
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="LR1Item"/>.
    /// </summary>
    /// <returns>A string that represents the current <see cref="LR1Item"/>.</returns>
    public override string ToString()
    {
        return GetSignature(useLookaheads: true);
    }

    /// <summary>
    /// Gets the signature of the LR(1) item.
    /// </summary>
    /// <param name="useLookaheads">A value indicating whether to use lookaheads in the signature.</param>
    /// <returns>The signature of the LR(1) item.</returns>
    public string GetSignature(bool useLookaheads = true)
    {
        var sentenceStrBuilder = new List<string>();

        for (int i = 0; i < Production.Body.Length; i++)
        {
            if (i == Position)
            {
                sentenceStrBuilder.Add("•");
            }

            sentenceStrBuilder.Add(Production.Body[i].ToString());
        }

        if (Position == Production.Body.Length)
        {
            sentenceStrBuilder.Add("•");
        }

        var sentenceStr = string.Join(" ", sentenceStrBuilder);
        var @base = $"{Production.Head} -> {sentenceStr}";

        var orderedLookaheads = Lookaheads.Length < 2
            ? Lookaheads
            : Lookaheads
                .OrderBy(x => x)
                .ToArray();

        var lookaheadStrs = orderedLookaheads
            .Select(x => x.ToString())
            .ToArray();

        var lookaheads = string.Join(", ", lookaheadStrs);

        if (useLookaheads)
        {
            return $"[({@base}), {{{lookaheads}}}]";
        }

        return $"[({@base})]";
    }

    /// <summary>
    /// Gets the alpha part of the production rule.
    /// </summary>
    /// <returns>The alpha part of the production rule.</returns>
    public ISentence GetAlpha()
    {
        if (Position == 0)
        {
            return new Sentence();
        }

        return Production.Body.GetRange(0, Position);
    }

    /// <summary>
    /// Gets the beta part of the production rule.
    /// </summary>
    /// <returns>The beta part of the production rule.</returns>
    public ISentence GetBeta()
    {
        var start = Position + 1;

        if ((Production.Body.Length - start) < 1)
        {
            return new Sentence();
        }

        return Production.Body.GetRange(start, Production.Body.Length - start);
    }

    /// <summary>
    /// Creates the next LR(1) item by advancing the position.
    /// </summary>
    /// <returns>The next LR(1) item.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the position is already at the end of the production body.</exception>
    public LR1Item CreateNextItem()
    {
        if (Position >= Production.Body.Length)
        {
            throw new InvalidOperationException("The position is already at the end of the production body.");
        }

        return new LR1Item(Production, Position + 1, Lookaheads);
    }

    /// <summary>
    /// Determines whether the LR(1) item contains the specified lookaheads.
    /// </summary>
    /// <param name="lookaheads">The lookaheads to check.</param>
    /// <returns><c>true</c> if the item contains the specified lookaheads; otherwise, <c>false</c>.</returns>
    public bool ContainsLookaheads(ITerminal[] lookaheads)
    {
        foreach (var lookahead in lookaheads)
        {
            if (!Lookaheads.Any(x => x.Equals(lookahead)))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Determines whether the LR(1) item contains the specified item.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns><c>true</c> if the item contains the specified item; otherwise, <c>false</c>.</returns>
    public bool ContainsItem(LR1Item item)
    {
        return Production == item.Production
            && Position == item.Position
            && ContainsLookaheads(item.Lookaheads);
    }
}
