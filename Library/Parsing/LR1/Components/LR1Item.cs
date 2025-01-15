using System.Runtime.CompilerServices;
using Aidan.TextAnalysis.Helpers;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Components.Symbols;
using Aidan.TextAnalysis.Language.Extensions;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents an LR(1) item.
/// </summary>
public class LR1Item : IEquatable<LR1Item>
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

    private int? HashCache { get; set; }
    private int? NoLookaheadHashCache { get; set; }

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
        Lookaheads = lookaheads
            .OrderBy(x => x)
            .ToArray();
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
    public string Signature => ComputeSignature(useLookaheads: true);

    /// <summary>
    /// Determines whether the specified <see cref="LR1Item"/> is equal to the current <see cref="LR1Item"/>.
    /// </summary>
    /// <param name="other">The <see cref="LR1Item"/> to compare with the current <see cref="LR1Item"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="LR1Item"/> is equal to the current <see cref="LR1Item"/>; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(LR1Item? other)
    {
        return other is not null
            && other.GetHashCode() == GetHashCode();
    }

    /// <summary>
    /// Returns the hash code for the current <see cref="LR1Item"/>.
    /// </summary>
    /// <returns>The hash code for the current <see cref="LR1Item"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
    {
        if (HashCache is not null)
        {
            return HashCache.Value;
        }

        object[] terms = new object[] { Production, Position }
            .Concat(Lookaheads)
            .ToArray();
        HashCache = HashHelper.ComputeHash(terms);
        return HashCache.Value;
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="LR1Item"/>.
    /// </summary>
    /// <returns>A string that represents the current <see cref="LR1Item"/>.</returns>
    public override string ToString()
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
                .ToArray();

        var lookaheadStrs = orderedLookaheads
            .Select(x => x.ToString())
            .ToArray();

        var lookaheads = string.Join(", ", lookaheadStrs);

        return $"[({@base}), {{{lookaheads}}}]";
    }

    /// <summary>
    /// Gets the signature of the LR(1) item.
    /// </summary>
    /// <param name="useLookaheads">A value indicating whether to use lookaheads in the signature.</param>
    /// <returns>The signature of the LR(1) item.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ComputeSignature(bool useLookaheads = true)
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
    /// Gets the signature of the LR(1) item.
    /// </summary>
    /// <param name="useLookaheads">A value indicating whether to use lookaheads in the signature.</param>
    /// <returns>The signature of the LR(1) item.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ComputeHash(bool useLookaheads = true)
    {
        if (useLookaheads)
        {
            return GetHashCode();
        }

        if (NoLookaheadHashCache is not null)
        {
            return NoLookaheadHashCache.Value;
        }

        NoLookaheadHashCache = HashHelper.ComputeHash(Production, Position);
        return NoLookaheadHashCache.Value;
    }

    /// <summary>
    /// Gets the alpha part of the production rule.
    /// </summary>
    /// <returns>The alpha part of the production rule.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ISentence GetAlpha()
    {
        if (Position == 0)
        {
            return new Sentence();
        }

        return Production.Body.GetRange(0, Position);
    }

    /// <summary>
    /// Gets the β part of the production rule.
    /// </summary>
    /// <returns> A new sentence containing the β part of the production rule. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LR1Item AdvancePosition()
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsItem(LR1Item item)
    {
        return Production == item.Production
            && Position == item.Position
            && ContainsLookaheads(item.Lookaheads);
    }
}
