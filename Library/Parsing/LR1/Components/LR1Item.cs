using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

public class LR1Item : 
    IEquatable<LR1Item>, 
    IEqualityComparer<LR1Item>
{
    public ProductionRule Production { get; }
    public int Position { get; }
    public Terminal[] Lookaheads { get; }

    public LR1Item(ProductionRule production, int position, params Terminal[] lookaheads)
    {
        Production = production;
        Position = position;
        Lookaheads = lookaheads;
    }

    public static bool operator ==(LR1Item left, LR1Item right)
    {
        return left.GetSignature(useLookaheads: true) == right.GetSignature(useLookaheads: true);
    }

    public static bool operator !=(LR1Item left, LR1Item right)
    {
        return left.GetSignature(useLookaheads: true) != right.GetSignature(useLookaheads: true);
    }

    public Symbol? Symbol => Position < Production.Body.Length 
        ? Production.Body[Position] 
        : null;

    public string Signature => GetSignature(useLookaheads: true);

    public bool Equals(LR1Item? other)
    {
        return other is not null 
            && other == this;
    }

    public bool Equals(LR1Item? left, LR1Item? right)
    {
        return (left is not null && right is not null)
            && left == right;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as LR1Item);
    }

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

    public override int GetHashCode()
    {
        return GetHashCode(this);
    }

    public override string ToString()
    {
        return GetSignature(useLookaheads: true);
    }

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

    public Sentence GetAlpha()
    {
        if(Position == 0)
        {
            return new Sentence();
        }

        return Production.Body.GetRange(0, Position);
    }

    public Sentence GetBeta()
    {
        var start = Position + 1;

        if ((Production.Body.Length - start) < 1)
        {
            return new Sentence();
        }

        return Production.Body.GetRange(start, Production.Body.Length - start);
    }

    public LR1Item CreateNextItem()
    {
        if(Position >= Production.Body.Length)
        {
            throw new InvalidOperationException("The position is already at the end of the production body.");
        }

        return new LR1Item(Production, Position + 1, Lookaheads);
    }

    public bool ContainsLookaheads(Terminal[] lookaheads)
    {
        foreach (var lookahead in lookaheads)
        {
            if (!Lookaheads.Any(x => x == lookahead))
            {
                return false;
            }
        }

        return true;
    }

    public bool ContainsItem(LR1Item item)
    {
        return Production == item.Production
            && Position == item.Position
            && ContainsLookaheads(item.Lookaheads);
    }

}
