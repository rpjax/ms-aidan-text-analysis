using Aidan.TextAnalysis.Helpers;
using Aidan.TextAnalysis.Language.Components;
using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

public class LR1ParserTable : ILR1ParserTable
{
    private Dictionary<string, uint> SymbolHashTable { get; }
    private Dictionary<ulong, LR1Action> Entries { get; }
    private IProductionRule[] Productions { get; }

    public LR1ParserTable(
        Dictionary<uint, LR1ParserTransition[]> entries,
        ISymbol[] symbols,
        IProductionRule[] productions)
    {
        SymbolHashTable = symbols
            .ToDictionary(
                x => x.Name,
                x => ComputeSymbolHash(x)
            );

        Entries = entries
            .SelectMany(kv => kv.Value.Select(t => new { State = kv.Key, Transition = t }))
            .ToDictionary(
                x => ComputeTransitionKey(x.State, x.Transition.Symbol),
                x => x.Transition.Action
            );

        Productions = productions;

    }

    public static LR1ParserTable Create(Grammar grammar)
    {
        return new LR1ParserTableFactory(grammar)
            .Create();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LR1Action? Lookup(uint state, ISymbol symbol)
    {
        var key = ComputeTransitionKey(state, symbol);

        if (Entries.TryGetValue(key, out var action))
        {
            return action;
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IProductionRule LookupProduction(uint index)
    {
        if (index < 0 || index >= Productions.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return Productions[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ulong ComputeTransitionKey(uint state, ISymbol symbol)
    {
        // bitwise OR the state and symbol hash
        //ulong key = state;
        //key = key << 32;
        //key = key | symbol.ComputeHash(); 
        return ((ulong)state) << 32 | GetSymbolHash(symbol);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private uint GetSymbolHash(ISymbol symbol)
    {
        if (!SymbolHashTable.TryGetValue(symbol.Name, out var hash))
        {
            throw new Exception("Symbol not found in the symbol hash table.");
        }

        return hash;
    }

    private uint ComputeSymbolHash(ISymbol symbol)
    {
        return HashHelper.ComputeFnvHash(symbol.Name);
    }
}
