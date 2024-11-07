using Aidan.TextAnalysis.Regexes.Ast;

namespace Aidan.TextAnalysis.Regexes.DfaComputation;

/* dfa stuff */

public class RegexLexeme
{
    public string Name { get; }
    public IRegexNode Pattern { get; }

    public RegexLexeme(
        string name, 
        IRegexNode pattern)
    {
        Name = name;
        Pattern = pattern;
    }

    public override string ToString()
    {
        return $"{Name}: /{Pattern}/";
    }

}
