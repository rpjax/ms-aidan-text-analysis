namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents an action type in a LR(1) parsing table.
/// </summary>
public enum LR1ActionType
{
    Shift,
    Reduce,
    Goto,
    Accept,
}

/// <summary>
/// Defines an action in a LR(1) parsing table. Concrete implementations are:
/// <list type="bullet">
///    <item><description><see cref="LR1ShiftAction"/></description></item>
///    <item><description><see cref="LR1ReduceAction"/></description></item>
///    <item><description><see cref="LR1GotoAction"/></description></item>
///    <item><description><see cref="LR1AcceptAction"/></description></item>
/// </list>
/// </summary>
public abstract class LR1Action : IEquatable<LR1Action>
{
    /// <summary>
    /// The type of the action.
    /// </summary>
    public LR1ActionType Type { get; init; }

    /// <summary>
    /// Returns a string representation of the action.
    /// </summary>
    /// <returns></returns>
    public abstract override string ToString();

    /// <summary>
    /// Determines weather this action is equal to another action.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public abstract bool Equals(LR1Action? other);

    /// <summary>
    /// Returns a hash code for this action.
    /// </summary>
    /// <remarks>
    /// The hash is value based, so two equal actions will have the same hash code.
    /// </remarks>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    /// <summary>
    /// Determines weather this action is equal to another object.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as LR1Action);
    }

    /// <summary>
    /// Casts this action to a <see cref="LR1ShiftAction"/>.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidCastException"></exception>
    public LR1ShiftAction AsShift()
    {
        return (LR1ShiftAction)this ?? throw new InvalidCastException();
    }

    /// <summary>
    /// Casts this action to a <see cref="LR1ReduceAction"/>.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidCastException"></exception>
    public LR1ReduceAction AsReduce()
    {
        return (LR1ReduceAction)this ?? throw new InvalidCastException();
    }

    /// <summary>
    /// Casts this action to a <see cref="LR1GotoAction"/>.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidCastException"></exception>
    public LR1GotoAction AsGoto()
    {
        return (LR1GotoAction)this ?? throw new InvalidCastException();
    }

    /// <summary>
    /// Casts this action to a <see cref="LR1AcceptAction"/>.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidCastException"></exception>
    public LR1AcceptAction AsAccept()
    {
        return (LR1AcceptAction)this ?? throw new InvalidCastException();
    }

}

/// <summary>
/// Represents a SHIFT action in a LR(1) parsing table.
/// </summary>
public class LR1ShiftAction : LR1Action
{
    public int NextState { get; }

    public LR1ShiftAction(int nextState)
    {
        Type = LR1ActionType.Shift;
        NextState = nextState;
    }

    public override bool Equals(LR1Action? other)
    {
        return other is LR1ShiftAction action 
            && NextState == action.NextState;
    }

    public override string ToString()
    {
        return $"Shift and Goto {NextState}";
    }
}

/// <summary>
/// Represents a REDUCE action in a LR(1) parsing table.
/// </summary>
public class LR1ReduceAction : LR1Action
{
    public int ProductionIndex { get; }

    public LR1ReduceAction(int productionIndex)
    {
        Type = LR1ActionType.Reduce;
        ProductionIndex = productionIndex;
    }

    public override bool Equals(LR1Action? other)
    {
        return other is LR1ReduceAction action 
            && ProductionIndex == action.ProductionIndex;
    }

    public override string ToString()
    {
        return $"Reduce using {ProductionIndex}";
    }
}

/// <summary>
/// Represents a GOTO action in a LR(1) parsing table.
/// </summary>
public class LR1GotoAction : LR1Action
{
    public int NextState { get; }

    public LR1GotoAction(int nextState)
    {
        Type = LR1ActionType.Goto;
        NextState = nextState;
    }

    public override bool Equals(LR1Action? other)
    {
        return other is LR1GotoAction action && NextState == action.NextState;
    }

    public override string ToString()
    {
        return $"Goto {NextState}";
    }
}

/// <summary>
/// Represents an ACCEPT action in a LR(1) parsing table.
/// </summary>
public class LR1AcceptAction : LR1Action
{
    public LR1AcceptAction()
    {
        Type = LR1ActionType.Accept;
    }

    public override bool Equals(LR1Action? other)
    {
        return other is LR1AcceptAction;
    }

    public override string ToString()
    {
        return "Accept";
    }
}
