using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.RegularExpressions.Ast.ClassChildren;

public class ClassSet : ClassChild
{
    public string Name { get; }
    public Charset Charset { get; }

    public ClassSet(string name, Charset charset)
        : base(ClassChildType.CharacterSet)
    {
        Name = name;
        Charset = charset;
    }

    public override string ToString()
    {
        return $"{Name}";
    }

    public override bool Equals(ClassChild? other)
    {
        return other is ClassSet set
            && set.Name == Name
            && set.Charset.Equals(Charset);
    }

    public override IEnumerable<char> GetLiterals()
    {
        return Charset;
    }

}