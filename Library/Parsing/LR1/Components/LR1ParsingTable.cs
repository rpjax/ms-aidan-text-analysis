using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;
using Aidan.TextAnalysis.Parsing.LR1.Tools;
using Aidan.TextAnalysis.Tokenization;
using Aidan.TextAnalysis.Tokenization.Extensions;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents a LR(1) parsing table. Contains the entries and the production rules.
/// </summary>
public class LR1ParsingTable : IEnumerable<LR1ParsingTableEntry>
{
    private LR1ParsingTableEntry[] Entries { get; }
    private ProductionRule[] Productions { get; }

    /// <summary>
    /// Creates a new instance of <see cref="LR1ParsingTable"/>.
    /// </summary>
    /// <param name="entries"></param>
    /// <param name="productions"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public LR1ParsingTable(
        LR1ParsingTableEntry[] entries,
        ProductionRule[] productions)
    {
        Entries = entries;
        Productions = productions;

        for (int i = 0; i < Entries.Length; i++)
        {
            var entry = Entries[i];

            if (entry.Id != i)
            {
                throw new InvalidOperationException("The entry id is not sequential.");
            }

            foreach (var action in entry.ActionTable.Values)
            {
                if (action is LR1ShiftAction shiftAction)
                {
                    if (shiftAction.NextState < 0 || shiftAction.NextState >= Entries.Length)
                    {
                        throw new InvalidOperationException("The shift action references an invalid state.");
                    }
                }
                if (action is LR1ReduceAction reduceAction)
                {
                    if (reduceAction.ProductionIndex < 0 || reduceAction.ProductionIndex >= Productions.Length)
                    {
                        throw new InvalidOperationException("The reduce action references an invalid production.");
                    }
                }
            }
        }

    }

    public LR1ParsingTableEntry this[int index] => Entries[index];

    /*
     * Key creation methods. These keys are used to index the action table.
     */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string CreateActionKey(Token token, bool useValue = false)
    {
        return token.ComputeFnv1aHash(useValue).ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string CreateActionKey(Terminal terminal, bool useValue = false)
    {
        return terminal.ComputeFnv1aHash(useValue).ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string CreateActionKey(NonTerminal nonterminal)
    {
        return nonterminal.ComputeFnv1aHash().ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string CreateActionKey(Symbol symbol, bool useValue = false)
    {
        if(symbol is Terminal terminal)
        {
            return CreateActionKey(terminal, useValue);
        }
        if(symbol is NonTerminal nonterminal)
        {
            return CreateActionKey(nonterminal);
        }

        throw new Exception("Invalid symbol type.");
    }

    /*
     * Static factory method
     */

    public static LR1ParsingTable Create(Grammar grammar)
    {
        return new LR1ParsingTableFactory()
            .Create(grammar);
    }

    /*
     * Lookup methods
     */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LR1Action? Lookup(int state, Token token)
    {
        if(state < 0 || state >= Entries.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(state));
        }

        return Entries[state].Lookup(token);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LR1GotoAction? LookupGoto(int state, NonTerminal nonTerminal)
    {
        if (state < 0 || state >= Entries.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(state));
        }

        var action = Entries[state].Lookup(nonTerminal);

        if (action is LR1GotoAction gotoAction)
        {
            return gotoAction;
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref ProductionRule GetProduction(int index)
    {
        if(index < 0 || index >= Productions.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return ref Productions[index];
    }

    /*
     * Enumeration of entries
     */

    public IEnumerator<LR1ParsingTableEntry> GetEnumerator()
    {
        return ((IEnumerable<LR1ParsingTableEntry>)Entries).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
