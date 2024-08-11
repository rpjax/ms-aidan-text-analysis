using System.Text;
using Aidan.TextAnalysis.Language.Transformations;

namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// Definition of a context-free grammar. (Chomsky hierarchy type 2) <br/>
/// </summary>
/// <remarks>
/// The grammar is defined as a 4-tuple G = (N, T, P, S), where: <br/>
/// - N is a set of non-terminal symbols. <br/>
/// - T is a set of terminal symbols. <br/>
/// - P is a set of production rules. <br/>
/// - S is the start symbol. <br/>
/// </remarks>
public class Grammar
{
    private ProductionSet InternalOriginalSet { get; }
    private ProductionSet InternalWorkingSet { get; set; }

    public Grammar(NonTerminal start, IEnumerable<ProductionRule> productions)
    {
        InternalOriginalSet = new ProductionSet(start, productions);
        InternalWorkingSet = new ProductionSet(start, productions);
    }

    public Grammar(ProductionSet set)
    {
        InternalOriginalSet = set.Copy();
        InternalWorkingSet = set.Copy();
    }

    public NonTerminal Start => InternalWorkingSet.Start;
    public ProductionSet Productions => InternalWorkingSet;
    public SetTransformationCollection Transformations => InternalWorkingSet.Transformations;

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder.AppendLine($"Start Symbol: {Start}");
        builder.AppendLine("Productions:");

        foreach (var production in Productions)
        {
            builder.AppendLine(production.ToString());
        }

        return builder.ToString();
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

    public Grammar GetOriginalGrammar()
    {
        return new Grammar(InternalOriginalSet);
    }

    public ProductionSet GetOriginalProductionSet()
    {
        return InternalOriginalSet;
    }

}
