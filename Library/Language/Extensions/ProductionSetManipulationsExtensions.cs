using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Language.Extensions;

/*
    Internal API for manipulating production sets. 
    In order to save memory, the sets are not immutable, but they have to be carefully manipulated.
    Outside of the internal scope of the library, sets can be considered immutable.
*/
public static partial class ProductionSetManipulationsExtensions
{
    internal static void Add(this ProductionSet self, params ProductionRule[] productions)
    {
        if (productions.Length == 0)
        {
            throw new InvalidOperationException("The list of productions is empty.");
        }

        self.Productions.AddRange(productions);
    }

    internal static void Remove(this ProductionSet self, params ProductionRule[] productions)
    {
        self.Productions.RemoveAll(x => productions.Any(y => x == y));
    }

    internal static void Replace(this ProductionSet self, Symbol symbol, Sentence replacement)
    {
        foreach (var production in self)
        {
            if (!production.Body.Contains(symbol))
            {
                continue;
            }

            var head = production.Head;
            var newBody = production.Body.Replace(symbol, replacement);
            var newProduction = new ProductionRule(head, newBody);

            self.Productions.Remove(production);
            self.Productions.Add(newProduction);
        }
    }

    internal static void Replace(this ProductionSet self, Symbol symbol, params Symbol[] replacement)
    {
        self.Replace(symbol, new Sentence(replacement));
    }

}

