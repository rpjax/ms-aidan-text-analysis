using Aidan.TextAnalysis.Regexes.Ast;

namespace Aidan.TextAnalysis.Regexes.DfaComputation;

/* dfa stuff */

public class RegexLexeme
{
    public string Name { get; }
    public RegexNode Pattern { get; }

    public RegexLexeme(
        string name, 
        RegexNode pattern)
    {
        Name = name;
        Pattern = pattern;
    }

    public override string ToString()
    {
        return $"{Name}: /{Pattern}/";
    }

}
