namespace Aidan.TextAnalysis.Parsing.LL1.Components;

// public class LL1SemanticAction
// {
//     private Action<LL1Context, Symbol>? InternalOnMatch { get; }
//     private Action<LL1Context, Symbol>? InternalOnExpand { get; }

//     public LL1SemanticAction(Action<LL1Context, Symbol>? onMatch = null, Action<LL1Context, Symbol>? onExpand = null)
//     {
//         InternalOnMatch = onMatch;
//         InternalOnExpand = onExpand;
//     }

//     public virtual void OnMatch(LL1Context context, Symbol symbol)
//     {
//         InternalOnMatch?.Invoke(context, symbol);
//     }

//     public virtual void OnExpand(LL1Context context, Symbol symbol)
//     {
//         InternalOnExpand?.Invoke(context, symbol);
//     }
// }

// public class LL1SemanticActionsTable
// {
//     private Dictionary<string, LL1SemanticAction> Entries { get; }

//     public LL1SemanticActionsTable(Dictionary<string, LL1SemanticAction>? entries = null)
//     {
//         Entries = entries ?? new();
//     }

//     public static string CreateActionKey(Symbol state, IEnumerable<Symbol> sentence)
//     {
//         var sentenceStr = string.Join(" ", sentence.Select(x => x.ToString()));
//         var key = $"{state}::{sentenceStr}";
//         return key;
//     }

//     public void Subscribe(Symbol symbol, LL1SemanticAction action)
//     {
//         if (!Entries.TryAdd(CreateActionKey(symbol), action))
//         {
//             throw new InvalidOperationException("The action is already subscribed.");
//         }
//     }

//     public void Unsubscribe(Symbol symbol)
//     {
//         if (!Entries.Remove(CreateActionKey(symbol)))
//         {
//             throw new InvalidOperationException("The action is not subscribed.");
//         }
//     }

//     public bool IsSubscribed(Symbol symbol)
//     {
//         return Entries.ContainsKey(CreateActionKey(symbol));
//     }

//     public void InvokeMatch(LL1Context context, Symbol symbol)
//     {
//         if (!Entries.TryGetValue(CreateActionKey(symbol), out var action))
//         {
//             return;
//         }

//         action.OnMatch(context, symbol);
//     }

//     public void InvokeExpand(LL1Context context, Symbol symbol)
//     {
//         if (!Entries.TryGetValue(CreateActionKey(symbol), out var action))
//         {
//             return;
//         }

//         action.OnExpand(context, symbol);
//     }

// }

