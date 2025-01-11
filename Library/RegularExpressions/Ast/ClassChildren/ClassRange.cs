namespace Aidan.TextAnalysis.RegularExpressions.Tree;

public class ClassRange : ClassChild
{
    public char Start { get; }
    public char End { get; }

    public ClassRange(char start, char end) 
        : base(ClassChildType.CharacterRange)
    {
        Start = start;
        End = end;

        if (Start >= End)
        {
            throw new InvalidOperationException();
        }
    }

    public override string ToString()
    {
        return $"{Start}-{End}";
    }

    public override bool Equals(ClassChild? other)
    {
        return other is ClassRange range
            && range.Start == Start 
            && range.End == End;
    }

    public override IEnumerable<char> GetLiterals()
    {
        var start = (int) Start;
        var end = (int) End;

        for (int i = start; i <= end; i++)
        {
            yield return (char) i;
        }
    }
}
