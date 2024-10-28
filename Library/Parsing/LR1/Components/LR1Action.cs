namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents an action type in a LR(1) parsing table.
/// </summary>
public enum LR1ActionType
{
    /// <summary>
    /// Represents a shift action.
    /// </summary>
    Shift,

    /// <summary>
    /// Represents a reduce action.
    /// </summary>
    Reduce,

    /// <summary>
    /// Represents a goto action.
    /// </summary>
    Goto,

    /// <summary>
    /// Represents an accept action.
    /// </summary>
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
    /// <returns>A string representation of the action.</returns>
    public abstract override string ToString();

    /// <summary>
    /// Determines whether this action is equal to another action.
    /// </summary>
    /// <param name="other">The other action to compare with.</param>
    /// <returns>True if the actions are equal, otherwise false.</returns>
    public abstract bool Equals(LR1Action? other);

    /// <summary>
    /// Returns a hash code for this action.
    /// </summary>
    /// <remarks>
    /// The hash is value based, so two equal actions will have the same hash code.
    /// </remarks>
    /// <returns>A hash code for this action.</returns>
    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    /// <summary>
    /// Determines whether this action is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>True if the object is an LR1Action and is equal to this action, otherwise false.</returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as LR1Action);
    }

    /// <summary>
    /// Casts this action to a <see cref="LR1ShiftAction"/>.
    /// </summary>
    /// <returns>The casted <see cref="LR1ShiftAction"/>.</returns>
    /// <exception cref="InvalidCastException">Thrown if the action cannot be casted to <see cref="LR1ShiftAction"/>.</exception>
    public LR1ShiftAction AsShift()
    {
        return (LR1ShiftAction)this ?? throw new InvalidCastException();
    }

    /// <summary>
    /// Casts this action to a <see cref="LR1ReduceAction"/>.
    /// </summary>
    /// <returns>The casted <see cref="LR1ReduceAction"/>.</returns>
    /// <exception cref="InvalidCastException">Thrown if the action cannot be casted to <see cref="LR1ReduceAction"/>.</exception>
    public LR1ReduceAction AsReduce()
    {
        return (LR1ReduceAction)this ?? throw new InvalidCastException();
    }

    /// <summary>
    /// Casts this action to a <see cref="LR1GotoAction"/>.
    /// </summary>
    /// <returns>The casted <see cref="LR1GotoAction"/>.</returns>
    /// <exception cref="InvalidCastException">Thrown if the action cannot be casted to <see cref="LR1GotoAction"/>.</exception>
    public LR1GotoAction AsGoto()
    {
        return (LR1GotoAction)this ?? throw new InvalidCastException();
    }

    /// <summary>
    /// Casts this action to a <see cref="LR1AcceptAction"/>.
    /// </summary>
    /// <returns>The casted <see cref="LR1AcceptAction"/>.</returns>
    /// <exception cref="InvalidCastException">Thrown if the action cannot be casted to <see cref="LR1AcceptAction"/>.</exception>
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
    /// <summary>
    /// The next state to shift to.
    /// </summary>
    public uint NextState { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LR1ShiftAction"/> class.
    /// </summary>
    /// <param name="nextState">The next state to shift to.</param>
    public LR1ShiftAction(uint nextState)
    {
        Type = LR1ActionType.Shift;
        NextState = nextState;
    }

    /// <summary>
    /// Determines whether this action is equal to another action.
    /// </summary>
    /// <param name="other">The other action to compare with.</param>
    /// <returns>True if the actions are equal, otherwise false.</returns>
    public override bool Equals(LR1Action? other)
    {
        return other is LR1ShiftAction action
            && NextState == action.NextState;
    }

    /// <summary>
    /// Returns a string representation of the action.
    /// </summary>
    /// <returns>A string representation of the action.</returns>
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
    /// <summary>
    /// The index of the production to reduce by.
    /// </summary>
    public uint ProductionIndex { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LR1ReduceAction"/> class.
    /// </summary>
    /// <param name="productionIndex">The index of the production to reduce by.</param>
    public LR1ReduceAction(uint productionIndex)
    {
        Type = LR1ActionType.Reduce;
        ProductionIndex = productionIndex;
    }

    /// <summary>
    /// Determines whether this action is equal to another action.
    /// </summary>
    /// <param name="other">The other action to compare with.</param>
    /// <returns>True if the actions are equal, otherwise false.</returns>
    public override bool Equals(LR1Action? other)
    {
        return other is LR1ReduceAction action
            && ProductionIndex == action.ProductionIndex;
    }

    /// <summary>
    /// Returns a string representation of the action.
    /// </summary>
    /// <returns>A string representation of the action.</returns>
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
    /// <summary>
    /// The next state to go to.
    /// </summary>
    public uint NextState { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LR1GotoAction"/> class.
    /// </summary>
    /// <param name="nextState">The next state to go to.</param>
    public LR1GotoAction(uint nextState)
    {
        Type = LR1ActionType.Goto;
        NextState = nextState;
    }

    /// <summary>
    /// Determines whether this action is equal to another action.
    /// </summary>
    /// <param name="other">The other action to compare with.</param>
    /// <returns>True if the actions are equal, otherwise false.</returns>
    public override bool Equals(LR1Action? other)
    {
        return other is LR1GotoAction action && NextState == action.NextState;
    }

    /// <summary>
    /// Returns a string representation of the action.
    /// </summary>
    /// <returns>A string representation of the action.</returns>
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
    /// <summary>
    /// Initializes a new instance of the <see cref="LR1AcceptAction"/> class.
    /// </summary>
    public LR1AcceptAction()
    {
        Type = LR1ActionType.Accept;
    }

    /// <summary>
    /// Determines whether this action is equal to another action.
    /// </summary>
    /// <param name="other">The other action to compare with.</param>
    /// <returns>True if the actions are equal, otherwise false.</returns>
    public override bool Equals(LR1Action? other)
    {
        return other is LR1AcceptAction;
    }

    /// <summary>
    /// Returns a string representation of the action.
    /// </summary>
    /// <returns>A string representation of the action.</returns>
    public override string ToString()
    {
        return "Accept";
    }
}
