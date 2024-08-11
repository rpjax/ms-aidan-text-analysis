using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Tokenization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents a single entry in the LR1 parsing table.
/// </summary>
public class LR1ParsingTableEntry
{
    /// <summary>
    /// The unique identifier of the entry.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// The state that this entry represents.
    /// </summary>
    public LR1State State { get; }

    /// <summary>
    /// The action table for this entry.
    /// </summary>
    public Dictionary<string, LR1Action> ActionTable { get; }

    /// <summary>
    /// Creates a new instance of <see cref="LR1ParsingTableEntry"/>.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="state"></param>
    /// <param name="actionsTable"></param>
    public LR1ParsingTableEntry(
        int id,
        LR1State state,
        Dictionary<Symbol, LR1Action> actionsTable)
    {
        Id = id;
        State = state;
        ActionTable = actionsTable
            .ToDictionary(x => CreateKey(x.Key), x => x.Value);
    }

    private static string CreateKey(Symbol obj)
    {
        return LR1ParsingTable.CreateActionKey(obj, useValue: true);
    }

    public override string ToString()
    {
        var actionStrBuilder = new StringBuilder();
        var mainStrBuilder = new StringBuilder();

        foreach (var action in ActionTable)
        {
            actionStrBuilder.AppendLine($"On ({action.Key}) {action.Value.ToString()};");
        }

        mainStrBuilder.AppendLine($"State: {Id}");
        mainStrBuilder.AppendLine(actionStrBuilder.ToString());

        return mainStrBuilder.ToString();
    }

    /// <summary>
    /// Looks up an action in the action table using the given lookahead token.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LR1Action? Lookup(Token token)
    {
        var key1 = LR1ParsingTable.CreateActionKey(token, useValue: true);

        if (ActionTable.TryGetValue(key1, out var action1))
        {
            return action1;
        }

        var key2 = LR1ParsingTable.CreateActionKey(token, useValue: false);

        if (ActionTable.TryGetValue(key2, out var action2))
        {
            return action2;
        }

        return null;
    }

    /// <summary>
    /// Looks up an action in the action table using the given non-terminal.
    /// </summary>
    /// <param name="nonTemrinal"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LR1Action? Lookup(NonTerminal nonTemrinal)
    {
        var key1 = LR1ParsingTable.CreateActionKey(nonTemrinal, useValue: true);

        if (ActionTable.TryGetValue(key1, out var action1))
        {
            return action1;
        }

        var key2 = LR1ParsingTable.CreateActionKey(nonTemrinal, useValue: false);

        if (ActionTable.TryGetValue(key2, out var action2))
        {
            return action2;
        }

        return null;
    }

}
