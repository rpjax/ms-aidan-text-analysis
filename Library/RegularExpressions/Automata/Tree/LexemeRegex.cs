using Aidan.TextAnalysis.RegularExpressions.Ast;

namespace Aidan.TextAnalysis.RegularExpressions.Automata;

public class LexemeRegex
{
    public string Name { get; }
    public RegexNode Regex { get; }

    public LexemeRegex(string name, RegexNode regex)
    {
        Name = name;
        Regex = regex;
    }

    public override string ToString()
    {
        return $"{Name} = /{Regex}/";
    }

    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;

            hash = hash * 23 + Name.GetHashCode();
            hash = hash * 23 + Regex.GetHashCode();

            return hash;
        }
    }

}
