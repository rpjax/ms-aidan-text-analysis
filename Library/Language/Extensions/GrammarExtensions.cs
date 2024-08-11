using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Parsing.LL1.Components;

namespace Aidan.TextAnalysis.Language.Extensions;

public static class GrammarExtensions
{
    public static bool IsLeftRecursive(this Grammar grammar)
    {
        return grammar.Productions
            .Any(x => x.IsLeftRecursive());
    }

    public static bool IsRightRecursive(this Grammar grammar)
    {
        return grammar.Productions
            .Any(x => x.IsRightRecursive());
    }

    public static bool IsNonDeterministic(this Grammar grammar)
    {
        return grammar.Productions
            .Where(x => x.GetTerminalPrefix() is not null)
            .GroupBy(x => x.Head.Name)
            .Any(x => x.GroupBy(y => y.GetTerminalPrefix()).Count() > 1);
    }

    public static Grammar ExpandMacros(this Grammar self)
    {
        self.Productions.ExpandMacros();
        return self;
    }

    public static Grammar AutoClean(this Grammar self)
    {
        self.Productions.RecursiveAutoClean();
        return self;
    }

    public static Grammar AutoFix(this Grammar self)
    {
        self.Productions.RecursiveAutoFix();
        return self;
    }

    public static Grammar AutoTransformLL1(this Grammar self)
    {
        self.Productions.AutoTransformLL1();
        return self;
    }

    public static Grammar AutoTransformLR1(this Grammar self)
    {
        self.Productions.AutoTransformLR1();
        return self;
    }

    public static LL1Grammar ToLL1(this Grammar self)
    {
        return new LL1Grammar(self.Start, self.Productions);
    }
}
