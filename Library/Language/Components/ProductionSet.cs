using System.Collections;
using Aidan.TextAnalysis.Language.Transformations;

namespace Aidan.TextAnalysis.Language.Components;

public class ProductionSet :
    IEnumerable<ProductionRule>
{
    public NonTerminal Start { get; internal set; }
    internal List<ProductionRule> Productions { get; }
    internal List<SetTransformation> Transformations { get; }
    internal List<SetTransformation> TransformationsTracker { get; }

    public ProductionSet(
        NonTerminal start,
        IEnumerable<ProductionRule> productions,
        IEnumerable<SetTransformation>? transformations = null)
    {
        Start = start;
        Productions = productions.ToList();
        Transformations = transformations?.ToList() ?? new();
        TransformationsTracker = new();

        if (Start is null)
        {
            throw new ArgumentNullException(nameof(start));
        }
        if (Productions is null)
        {
            throw new ArgumentNullException(nameof(productions));
        }
        if (Transformations is null)
        {
            throw new ArgumentNullException(nameof(transformations));
        }
        if(!Productions.Any(x => x.Head != Start))
        {
            throw new ArgumentException("The start symbol must be the head of at least one production rule.");
        }
    }

    public ProductionRule this[int index]
    {
        get => Productions[index];
        set => Productions[index] = value;
    }

    public int Length => Productions.Count;
    public SetTransformationCollection TransformationCollection => Transformations;

    public static bool operator ==(ProductionSet left, ProductionSet right)
    {
        return left.SequenceEqual(right);
    }

    public static bool operator !=(ProductionSet left, ProductionSet right)
    {
        return !left.SequenceEqual(right);
    }

    public static implicit operator ProductionRule[](ProductionSet set)
    {
        return set.Productions.ToArray();
    }

    public static implicit operator List<ProductionRule>(ProductionSet set)
    {
        return set.Productions.ToList();
    }

    public IEnumerator<ProductionRule> GetEnumerator()
    {
        return Productions.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Productions.GetEnumerator();
    }

    public override bool Equals(object? obj)
    {
        return obj is ProductionSet set
            && set.SequenceEqual(this);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            foreach (var production in Productions)
            {
                hash = (hash * 16777619) ^ production.GetHashCode();
            }

            return hash;
        }
    }

    public override string ToString()
    {
        return ToNotation(NotationType.Sentential);
    }

    public ProductionSet Copy()
    {
        /*
            Production rules are immutable, so we can just return a new production set with the same production rules.
        */
        return new ProductionSet(Start, Productions, Transformations);
    }

    public IEnumerable<NonTerminal> GetNonTerminals()
    {
        return Productions
            .Select(x => x.Head)
            .Distinct();
    }

    public IEnumerable<Terminal> GetTerminals()
    {
        return Productions
            .SelectMany(x => x.Body)
            .OfType<Terminal>()
            .Distinct();
    }

    public IEnumerable<ProductionRule> Lookup(NonTerminal nonTerminal)
    {
        return Productions.Where(x => x.Head == nonTerminal);
    }

    public SetTransformationBuilder GetTransformationBuilder(string name)
    {
        return new SetTransformationBuilder(name, this);
    }

    public void ResetTransformationsTracker()
    {
        TransformationsTracker.Clear();
    }

    public SetTransformationCollection GetTrackedTransformations()
    {
        return TransformationsTracker;
    }

    public void Apply(SetTransformation transformation)
    {
        foreach (var operation in transformation)
        {
            operation.Apply(this);
        }
    }

    public void Reverse(SetTransformation transformation)
    {
        foreach (var operation in transformation.Operations.Reverse())
        {
            operation.Reverse(this);
        }
    }

    public string ToNotation(NotationType notation)
    {
        switch (notation)
        {
            case NotationType.Sentential:
                return ToSententialNotation();

            case NotationType.Bnf:
                return ToBnfNotation();

            case NotationType.Ebnf:
                return ToEbnfNotation();

            case NotationType.EbnfKleene:
                return ToEbnfKleeneNotation();
        }

        throw new InvalidOperationException("Invalid notation type.");
    }

    /*
     * notation conversion helper methods section.
    */

    private string ToSententialNotation()
    {
        return string.Join($"{Environment.NewLine}", Productions.Select(x => x.ToNotation(NotationType.Sentential)));
    }

    private string ToBnfNotation()
    {
        return string.Join($"{Environment.NewLine}", Productions.Select(x => x.ToNotation(NotationType.Bnf)));
    }

    private string ToEbnfNotation()
    {
        return string.Join($"{Environment.NewLine}", Productions.Select(x => x.ToNotation(NotationType.Ebnf)));
    }

    private string ToEbnfKleeneNotation()
    {
        return string.Join(Environment.NewLine, Productions.Select(x => x.ToNotation(NotationType.EbnfKleene)));
    }

}
