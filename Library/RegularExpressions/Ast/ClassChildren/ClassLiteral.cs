namespace Aidan.TextAnalysis.RegularExpressions.Ast;

public class ClassLiteral : ClassChild
{
    public char Character { get; }

    public ClassLiteral(char character) 
        : base(ClassChildType.Literal)
    {
        Character = character;
    }

    public override string ToString()
    {
        return Character.ToString();
    }

    public override bool Equals(ClassChild? other)
    {
        return other is ClassLiteral classCharacter
            && classCharacter.Character == Character;
    }

    public override IEnumerable<char> GetLiterals()
    {
        yield return Character;
    }
}
