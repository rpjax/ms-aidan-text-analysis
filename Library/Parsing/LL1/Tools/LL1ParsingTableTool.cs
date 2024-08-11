using Aidan.TextAnalysis.Language;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;

namespace Aidan.TextAnalysis.Parsing.LL1.Tools;

public class LL1ParsingTableTool
{
    public static LL1ParsingTable ComputeLL1ParsingTable(ProductionSet set)
    {
        var entries = new List<LL1ParsingTableEntry>();

        foreach (var production in set)
        {
            var firstSet = production.ComputeFirstSet(set);
            var containsEpsilon = firstSet.Contains(Epsilon.Instance);

            foreach (var symbol in firstSet)
            {
                if (symbol is Epsilon)
                {
                    continue;
                }
                if (symbol is not Terminal terminal)
                {
                    throw new InvalidOperationException("Invalid symbol in first set");
                }

                entries.Add(new LL1ParsingTableEntry(production.Head, terminal, production));
            }

            if (containsEpsilon)
            {
                var followSet = production.ComputeFollowSet(set);

                foreach (var symbol in followSet)
                {
                    if (symbol is not Terminal terminal)
                    {
                        throw new InvalidOperationException("Invalid symbol in follow set");
                    }

                    if(entries.Where(x => x.State == production.Head && x.Lookahead == terminal).Any())
                    {
                        continue;
                    }

                    entries.Add(new LL1ParsingTableEntry(production.Head, terminal, production));
                }

                continue;
            }

        }

        return new LL1ParsingTable(entries);
    }
}

// public class LL1SemanticActionsTableTool
// {
//     public static LL1SemanticActionsTable ComputeSemanticActionsTable(LL1ParsingTable parsingTable)
//     {
//         var entries = new Dictionary<string, LL1SemanticAction>();
//         var table = new LL1SemanticActionsTable();

//         foreach (var entry in parsingTable.Entries)
//         {
//             var production = entry.Value;

//             if (production.Body.Length == 0)
//             {
//                 throw new InvalidOperationException();
//             }

//             var last = production.Body.Last();

//             var lastAction = new LL1SemanticAction(
//                 onMatch: (context, symbol) => OnMatchLast(context, symbol)
//             );

//             table.Subscribe(last, lastAction);

//             var head = production.Head;

//             if (!table.IsSubscribed(head))
//             {
//                 var headAction = new LL1SemanticAction(
//                     onExpand: (context, symbol) => OnHeadExpand(context, symbol)
//                 );

//                 table.Subscribe(head, headAction);
//             }
//         }

//         return table;
//     }

//     private static void OnMatchLast(LL1Context context, Symbol symbol)
//     {
//         context.SyntaxContext.FinilizeBranch();
//     }

//     private static void OnHeadExpand(LL1Context context, Symbol symbol)
//     {
//         context.SyntaxContext.CreateBranch(symbol);
//     }

// }

