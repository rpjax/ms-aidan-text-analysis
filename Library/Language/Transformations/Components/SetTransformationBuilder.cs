using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Language.Transformations;

public class SetTransformationBuilder
{
    private string Name { get; set; }
    private ProductionSet Set { get; }
    private List<SetOperation> Operations { get; }
    private bool IsBuilt { get; set; }

    public SetTransformationBuilder(string name, ProductionSet set)
    {
        Name = name;
        Set = set;
        Operations = new();
    }

    public SetTransformationBuilder AddProductions(params ProductionRule[] productions)
    {
        if (IsBuilt)
        {
            throw new InvalidOperationException("The transformation has already been built.");
        }

        foreach (var production in productions)
        {
            Operations.Add(new AddProductionOperation(production));
        }

        return this;
    }

    public SetTransformationBuilder RemoveProductions(params ProductionRule[] productions)
    {
        if (IsBuilt)
        {
            throw new InvalidOperationException("The transformation has already been built.");
        }

        foreach (var production in productions)
        {
            Operations.Add(new RemoveProductionOperation(production));
        }

        return this;
    }

    public SetTransformationBuilder ReplaceSymbol(Symbol oldSymbol, Symbol newSymbol)
    {
        if (IsBuilt)
        {
            throw new InvalidOperationException("The transformation has already been built.");
        }

        Operations.Add(new ReplaceSymbolOperation(oldSymbol, newSymbol));
        return this;
    }

    public SetTransformationBuilder SetStart(NonTerminal nonTerminal)
    {
        if (IsBuilt)
        {
            throw new InvalidOperationException("The transformation has already been built.");
        }

        Operations.Add(new SetStartOperation(
            original: Set.Start, 
            updated: nonTerminal
        ));
        return this;
    }

    public void Build()
    {
        if (IsBuilt)
        {
            throw new InvalidOperationException("The transformation has already been built.");
        }

        var transformation = new SetTransformation(Name, Operations.ToArray());

        Set.Transformations.Add(transformation);
        Set.TransformationsTracker.Add(transformation);
        Set.Apply(transformation);
        IsBuilt = true;
    }

}
