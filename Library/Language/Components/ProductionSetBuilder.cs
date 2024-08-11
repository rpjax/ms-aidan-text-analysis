namespace Aidan.TextAnalysis.Language.Components;

public class ProductionSetBuilder
{
    private NonTerminal? Start { get; set; }
    private List<ProductionRule> Productions { get; }

    public ProductionSetBuilder()
    {
        Start = null;
        Productions = new();
    }

    public ProductionSetBuilder Add(ProductionRule production)
    {
        Productions.Add(production);
        return this;
    }

    public ProductionSetBuilder Add(NonTerminal head, params Symbol[] body)
    {
        Productions.Add(new ProductionRule(head, body));
        return this;
    }

    public ProductionSetBuilder SetStart(NonTerminal start)
    {
        Start = start;
        return this;
    }

    public ProductionSet Build()
    {
        if (Start is null)
        {
            throw new InvalidOperationException("The start symbol must be set.");
        }
        
        return new ProductionSet(Start, Productions);
    }
}