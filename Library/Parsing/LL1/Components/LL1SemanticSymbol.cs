using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Parsing.LL1.Components;

public enum LL1SemanticActionType
{
    FinilizeBranch
}

public class LL1SemanticSymbol : Symbol
{
    public static LL1SemanticSymbol FinilizeBranch { get; } = new(LL1SemanticActionType.FinilizeBranch);

    public override bool IsTerminal => false;

    public override bool IsNonTerminal => false;

    public override bool IsEpsilon => false;

    public override bool IsMacro => false;

    public override bool IsEoi => false;

    public LL1SemanticActionType Action { get; }

    public LL1SemanticSymbol(LL1SemanticActionType type)
    {
        Action = type;
    }

    public override string ToString()
    {
        return $"SemanticAction({Action})";
    }

    public override string ToNotation(NotationType notation)
    {
        return ToString(); ;
    }

    public override bool Equals(object? obj)
    {
        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public override bool Equals(Symbol? other)
    {
        throw new NotImplementedException();
    }

    public override bool Equals(ISymbol? other)
    {
        throw new NotImplementedException();
    }
}

