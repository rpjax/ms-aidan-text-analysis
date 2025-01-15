using System.Runtime.CompilerServices;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Parsing.LR1.TableComputation;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents the LR(1) parser table used for parsing.
/// </summary>
public class LR1ParserTable : ILR1ParserTable
{
    /// <summary>
    /// Gets the symbol hash table.
    /// </summary>
    private Dictionary<string, uint> SymbolHashTable { get; }

    /// <summary>
    /// Gets the parser table entries.
    /// </summary>
    private Dictionary<ulong, LR1Action> Entries { get; }

    /// <summary>
    /// Gets the production rules.
    /// </summary>
    private IProductionRule[] Productions { get; }

    /// <summary>
    /// Gets the computed states of the parser.
    /// </summary>
    private LR1State[]? States { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LR1ParserTable"/> class.
    /// </summary>
    /// <param name="stateTransitionsDictionary">The parser table entries.</param>
    /// <param name="productions">The production rules.</param>
    /// <param name="states">The states of the parser.</param>
    public LR1ParserTable(
        Dictionary<uint, LR1ParserTransition[]> stateTransitionsDictionary,
        IProductionRule[] productions,
        LR1State[]? states = null)
    {
        SymbolHashTable = ComputeSymbolHashTable(stateTransitionsDictionary);
        Entries = ComputeEntries(stateTransitionsDictionary);
        Productions = productions;
        States = states;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="LR1ParserTable"/> class from the specified grammar.
    /// </summary>
    /// <param name="grammar">The grammar to use for creating the parser table.</param>
    /// <returns>A new instance of the <see cref="LR1ParserTable"/> class.</returns>
    public static LR1ParserTable Create(IGrammar grammar)
    {
        return new LR1ParserTableFactory(grammar)
            .Create();
    }

    /// <summary>
    /// Computes the hash for the specified symbol.
    /// </summary>
    /// <param name="symbol">The symbol to compute the hash for.</param>
    /// <returns>The computed hash.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint ComputeSymbolHash(ISymbol symbol)
    {
        return (uint)symbol.Name.GetHashCode();
    }

    /// <summary>
    /// Computes the transition key for the specified state and symbol.
    /// </summary>
    /// <param name="state">The state to use.</param>
    /// <param name="symbol">The symbol to use.</param>
    /// <returns>The computed transition key.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong ComputeTransitionKey(uint state, ISymbol symbol)
    {
        return ((ulong)state) << 32 | ComputeSymbolHash(symbol);
    }

    /// <summary>
    /// Computes the symbol hash table from the specified entries.
    /// </summary>
    /// <param name="entries">The entries to use.</param>
    /// <returns>The computed symbol hash table.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Dictionary<string, uint> ComputeSymbolHashTable(
        Dictionary<uint, LR1ParserTransition[]> entries)
    {
        var symbols = entries
            .SelectMany(kv => kv.Value.Select(t => t.Symbol))
            .Distinct(new SymbolEqualityComparer())
            .ToArray();

        return symbols
            .ToDictionary(
                x => x.Name,
                x => ComputeSymbolHash(x)
            );
    }

    /// <summary>
    /// Computes the parser table entries from the specified state transitions dictionary.
    /// </summary>
    /// <param name="stateTransitionsDictionary">The state transitions dictionary to use.</param>
    /// <returns>The computed parser table entries.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Dictionary<ulong, LR1Action> ComputeEntries(
        Dictionary<uint, LR1ParserTransition[]> stateTransitionsDictionary)
    {
        return stateTransitionsDictionary
            .SelectMany(kv => kv.Value.Select(t => new { State = kv.Key, Transition = t }))
            .ToDictionary(
                x => ComputeTransitionKey(x.State, x.Transition.Symbol),
                x => x.Transition.Action
            );
    }

    /// <summary>
    /// Looks up the action for the specified state and symbol.
    /// </summary>
    /// <param name="state">The state to look up.</param>
    /// <param name="symbol">The symbol to look up.</param>
    /// <returns>The action for the specified state and symbol, or null if not found.</returns>
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

    /// <summary>
    /// Looks up the production rule for the specified index.
    /// </summary>
    /// <param name="index">The index of the production rule to look up.</param>
    /// <returns>The production rule for the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IProductionRule LookupProduction(uint index)
    {
        if (index < 0 || index >= Productions.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return Productions[index];
    }

    /// <summary>
    /// Computes the transition key for the specified state and symbol.
    /// </summary>
    /// <param name="state">The state to use.</param>
    /// <param name="symbol">The symbol to use.</param>
    /// <returns>The computed transition key.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ulong GetTransitionKey(uint state, ISymbol symbol)
    {
        return ((ulong)state) << 32 | GetSymbolHash(symbol);
    }

    /// <summary>
    /// Gets the hash for the specified symbol.
    /// </summary>
    /// <param name="symbol">The symbol to get the hash for.</param>
    /// <returns>The hash of the symbol.</returns>
    /// <exception cref="Exception">Thrown when the symbol is not found in the symbol hash table.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private uint GetSymbolHash(ISymbol symbol)
    {
        if (!SymbolHashTable.TryGetValue(symbol.Name, out var hash))
        {
            throw new Exception("Symbol not found in the symbol hash table.");
        }

        return hash;
    }
}
