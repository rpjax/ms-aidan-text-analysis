using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;

namespace Aidan.TextAnalysis.Language.Transformations;

/// <summary>
/// Represents a transformation that factors out common prefixes in a production subset.
/// </summary>
/// <remarks>
/// All productions in the subset must have a common head and a common prefix (alpha).
/// </remarks>
public class TerminalLeftFactorization : ISetTransformer
{
    /*  
        A -> a A B.  
        A -> a B c.  
        A -> a A c.   
        A -> a.

        after factorization:
        A - a A′. 
        A′ -> A B.
        A′ -> B c.
        A′ -> A c.
        A′ -> ε.
    */
    public SetTransformationCollection ExecuteTransformations(ProductionSet set)
    {
        set.EnsureNoMacros();
        set.ResetTransformationsTracker();

        var subsets = set.GetSubsetsGroupedByCommonTerminalLeftFactor();

        foreach (var subset in subsets)
        {
            LeftFactorSubset(subset);
        }

        return set.GetTrackedTransformations();
    }

    private void LeftFactorSubset(ProductionSubset subset)
    {
        var set = subset.MainSet;

        if (subset.Length == 0)
        {
            throw new InvalidOperationException("The production set is empty.");
        }

        var head = subset.First().Head;
        var alpha = subset.First().Body.First();

        foreach (var production in subset)
        {
            if (production.Head != head)
            {
                throw new InvalidOperationException($"The productions do not have a common head. Conflicting head in subset: {production.Head}");
            }
            if (production.Body.First() != alpha)
            {
                throw new InvalidOperationException("The productions do not have a common prefix.");
            }
            if (!production.Body.First().IsTerminal)
            {
                throw new InvalidOperationException("The common prefix must be a terminal symbol.");
            }
        }

        var betas = subset
            .Select(x => x.Body.Skip(1).ToArray())
            .Select(x => new Sentence(x))
            .Distinct()
            .ToArray()
            ;

        var producesEpsilon = betas.Any(x => x.Length == 0);

        var builder = set.GetTransformationBuilder("Left Factorization");

        var newNonTerminal = set.CreateNonTerminalPrime(head);
        var newHeadProduction = new ProductionRule(
            head: head,
            body: new Sentence(alpha, newNonTerminal)
        );

        builder
            .RemoveProductions(subset.ToArray())
            .AddProductions(newHeadProduction)
            ;

        if (producesEpsilon)
        {
            var epsilonProduction = new ProductionRule(
                head: newNonTerminal,
                body: new Epsilon()
            );

            builder.AddProductions(epsilonProduction);
        }

        for (int i = 0; i < betas.Length; i++)
        {
            var beta = betas[i];

            var newProduction = new ProductionRule(
                head: newNonTerminal,
                body: beta
            );

            builder.AddProductions(newProduction);
        }

        builder.Build();
    }

}

public class NonTerminalLeftFactorization : ISetTransformer
{
    public SetTransformationCollection ExecuteTransformations(ProductionSet set)
    {
        set.EnsureNoMacros();
        set.ResetTransformationsTracker();

        var subsets = set.GetSubsetsGroupedByCommonNonTerminalLeftFactor();

        foreach (var subset in subsets)
        {
            LeftFactorSubset(subset);
        }

        return set.GetTrackedTransformations();
    }

    private void LeftFactorSubset(ProductionSubset subset)
    {
        var set = subset.MainSet;

        if (subset.Length == 0)
        {
            throw new InvalidOperationException("The production set is empty.");
        }

        var head = subset.First().Head;
        var alpha = subset.First().Body.First();

        foreach (var production in subset)
        {
            if (production.Head != head)
            {
                throw new InvalidOperationException($"The productions do not have a common head. Conflicting head in subset: {production.Head}");
            }
            if (production.Body.First() != alpha)
            {
                throw new InvalidOperationException("The productions do not have a common prefix.");
            }
            if (!production.Body.First().IsNonTerminal)
            {
                throw new InvalidOperationException("The common prefix must be a non-terminal symbol.");
            }
        }

        var betas = subset
            .Select(x => x.Body.Skip(1).ToArray())
            .Select(x => new Sentence(x))
            .Distinct()
            .ToArray()
            ;

        var producesEpsilon = betas.Any(x => x.Length == 0);

        var builder = set.GetTransformationBuilder("Non-Symbol Left Factorization");

        var newNonTerminal = set.CreateNonTerminalPrime(head);
        var newHeadProduction = new ProductionRule(
            head: head,
            body: new Sentence(alpha, newNonTerminal)
        );

        builder
            .RemoveProductions(subset.ToArray())
            .AddProductions(newHeadProduction)
            ;

        if (producesEpsilon)
        {
            var epsilonProduction = new ProductionRule(
                head: newNonTerminal,
                body: new Epsilon()
            );

            builder.AddProductions(epsilonProduction);
        }

        for (int i = 0; i < betas.Length; i++)
        {
            var beta = betas[i];

            var newProduction = new ProductionRule(
                head: newNonTerminal,
                body: beta
            );

            builder.AddProductions(newProduction);
        }

        builder.Build();
    }

}
