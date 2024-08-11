using System.Diagnostics.CodeAnalysis;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;

namespace Aidan.TextAnalysis.Language.Transformations;

public enum OperationType
{
    AddProduction,
    RemoveProduction,
    ReplaceSymbol,
    SetStart
}

public abstract class SetOperation : 
    IEquatable<SetOperation>, 
    IEqualityComparer<SetOperation>
{
    public abstract OperationType Type { get; }
    public string? Explanation { get; set; }

    public static bool operator ==(SetOperation left, SetOperation right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SetOperation left, SetOperation right)
    {
        return !left.Equals(right);
    }

    public abstract bool Equals(SetOperation? other);
    public abstract bool Equals(SetOperation? x, SetOperation? y);
    public abstract override bool Equals(object? obj);
    public abstract int GetHashCode([DisallowNull] SetOperation obj);
    public abstract override int GetHashCode();
    public abstract override string ToString();
    public abstract void Apply(ProductionSet set);
    public abstract void Reverse(ProductionSet set);

    public AddProductionOperation AsAddProduction()
    {
        if (this is not AddProductionOperation addProduction)
        {
            throw new InvalidOperationException("The operation is not an AddProductionOperation");
        }

        return addProduction;
    }

    public RemoveProductionOperation AsRemoveProduction()
    {
        if (this is not RemoveProductionOperation removeProduction)
        {
            throw new InvalidOperationException("The operation is not a RemoveProductionOperation");
        }

        return removeProduction;
    }

    public ReplaceSymbolOperation AsReplaceSymbol()
    {
        if (this is not ReplaceSymbolOperation replaceSymbol)
        {
            throw new InvalidOperationException("The operation is not a ReplaceSymbolOperation");
        }

        return replaceSymbol;
    }

}

public class AddProductionOperation : SetOperation
{
    public override OperationType Type => OperationType.AddProduction;
    public ProductionRule Production { get; }

    public AddProductionOperation(ProductionRule production)
    {
        Production = production;
    }

    public override bool Equals(SetOperation? other)
    {
        return other is AddProductionOperation addProduction
            && addProduction.Production == Production;
    }

    public override bool Equals(SetOperation? x, SetOperation? y)
    {
        if (x is not AddProductionOperation addProductionX || y is not AddProductionOperation addProductionY)
        {
            return false;
        }

        return addProductionX.Production == addProductionY.Production;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as SetOperation);
    }

    public override int GetHashCode([DisallowNull] SetOperation obj)
    {
        return obj.GetHashCode();
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ Production.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        if (!string.IsNullOrWhiteSpace(Explanation))
        {
            return Explanation;
        }

        return $"+ {Production}";
    }

    public override void Apply(ProductionSet set)
    {
        set.Add(Production);
    }

    public override void Reverse(ProductionSet set)
    {
        set.Remove(Production);
    }
}

public class RemoveProductionOperation : SetOperation
{
    public override OperationType Type => OperationType.RemoveProduction;
    public ProductionRule Production { get; }

    public RemoveProductionOperation(ProductionRule production)
    {
        Production = production;
    }

    public override bool Equals(SetOperation? other)
    {
        return other is RemoveProductionOperation removeProduction
            && removeProduction.Production == Production;
    }

    public override bool Equals(SetOperation? x, SetOperation? y)
    {
        if (x is not RemoveProductionOperation removeProductionX || y is not RemoveProductionOperation removeProductionY)
        {
            return false;
        }

        return removeProductionX.Production == removeProductionY.Production;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as SetOperation);
    }

    public override int GetHashCode([DisallowNull] SetOperation obj)
    {
        return obj.GetHashCode();
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ Production.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        if (!string.IsNullOrWhiteSpace(Explanation))
        {
            return Explanation;
        }

        return $"- {Production}";
    }

    public override void Apply(ProductionSet set)
    {
        set.Remove(Production);
    }

    public override void Reverse(ProductionSet set)
    {
        set.Add(Production);
    }

}

public class ReplaceSymbolOperation : SetOperation
{
    public override OperationType Type => OperationType.ReplaceSymbol;
    public Symbol OldSymbol { get; }
    public Symbol NewSymbol { get; }

    public ReplaceSymbolOperation(Symbol oldSymbol, Symbol newSymbol)
    {
        OldSymbol = oldSymbol;
        NewSymbol = newSymbol;
    }

    public override bool Equals(SetOperation? other)
    {
        return other is ReplaceSymbolOperation replaceSymbol
            && replaceSymbol.OldSymbol == OldSymbol
            && replaceSymbol.NewSymbol == NewSymbol;
    }

    public override bool Equals(SetOperation? x, SetOperation? y)
    {
        if (x is not ReplaceSymbolOperation replaceSymbolX || y is not ReplaceSymbolOperation replaceSymbolY)
        {
            return false;
        }

        return replaceSymbolX.OldSymbol == replaceSymbolY.OldSymbol
            && replaceSymbolX.NewSymbol == replaceSymbolY.NewSymbol;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as SetOperation);
    }

    public override int GetHashCode([DisallowNull] SetOperation obj)
    {
        return obj.GetHashCode();
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ OldSymbol.GetHashCode();
            hash = (hash * 16777619) ^ NewSymbol.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        if (!string.IsNullOrWhiteSpace(Explanation))
        {
            return Explanation;
        }

        return $"Replace {OldSymbol} with {NewSymbol}";
    }

    public override void Apply(ProductionSet set)
    {
        set.Replace(OldSymbol, NewSymbol);
    }

    public override void Reverse(ProductionSet set)
    {
        set.Replace(NewSymbol, OldSymbol);
    }

}

public class SetStartOperation : SetOperation
{
    public override OperationType Type => OperationType.SetStart;

    private NonTerminal OriginalStart { get; }
    private NonTerminal UpdatedStart { get; }

    public SetStartOperation(NonTerminal original, NonTerminal updated)
    {
        OriginalStart = original;
        UpdatedStart = updated;
    }

    public override void Apply(ProductionSet set)
    {
        set.Start = UpdatedStart;
    }

    public override void Reverse(ProductionSet set)
    {
        set.Start = OriginalStart;
    }

    public override bool Equals(SetOperation? other)
    {
        return other is SetStartOperation setStart
            && setStart.OriginalStart == OriginalStart
            && setStart.UpdatedStart == UpdatedStart;
    }

    public override bool Equals(SetOperation? x, SetOperation? y)
    {
        return x is SetStartOperation setStartX
            && y is SetStartOperation setStartY
            && setStartX.OriginalStart == setStartY.OriginalStart
            && setStartX.UpdatedStart == setStartY.UpdatedStart;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as SetOperation);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ OriginalStart.GetHashCode();
            hash = (hash * 16777619) ^ UpdatedStart.GetHashCode();
            return hash;
        }
    }

    public override int GetHashCode([DisallowNull] SetOperation obj)
    {
        return obj.GetHashCode();
    }

    public override string ToString()
    {
        return $"Set start to {UpdatedStart}";
    }
}

