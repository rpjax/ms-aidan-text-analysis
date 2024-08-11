namespace Aidan.TextAnalysis.Language.Components;

/// <summary>
/// A production rule is a rule that defines how a non-terminal symbol can be replaced by a sequence of other symbols. <br/>
/// The production rules are components of a context-free grammar that describe the syntax of a language.
/// </summary>
/// <remarks>
/// The arrow symbol represents the replacement operation. So, (X -> Y), reads as "X can be replaced by Y". <br/>
/// Examples: (sentential notation):
/// <br/>
/// <list type="bullet">
///    <item> <code>integer -> [ sign ] digit { digit }.</code> </item>
///    <item> <code>sign -> '+' | '-'.</code> </item>
///    <item> <code>digit -> '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9'. </code> </item>
/// </list>
/// </remarks>
public struct ProductionRule : IEquatable<ProductionRule>
{
    /// <summary>
    /// The head of the production rule. It is the non-terminal symbol that is being replaced. The left-hand side of the rule (LHS).
    /// </summary>
    public NonTerminal Head { get; }

    /// <summary>
    /// The body of the production rule. It is the sequence of symbols that replace the head. The right-hand side of the rule (RHS).
    /// </summary>
    public Sentence Body { get; }

    /// <summary>
    /// The number of terminal symbols in the body of the production rule.
    /// </summary>
    public int TerminalCount { get; }

    /// <summary>
    /// The number of non-terminal symbols in the body of the production rule.
    /// </summary>
    public int NonTerminalCount { get; }

    /// <summary>
    /// Creates a new instance of <see cref="ProductionRule"/>.
    /// </summary>
    /// <param name="head"> The head of the production rule. </param>
    /// <param name="body"> The body of the production rule. </param>
    public ProductionRule(NonTerminal head, params Symbol[] body)
    {
        Head = head;
        Body = body;
        TerminalCount = Body.Count(x => x.IsTerminal);
        NonTerminalCount = Body.Count(x => x.IsNonTerminal);
        Validate();
    }

    /// <summary>
    /// Determines whether the production rule is an epsilon production. 
    /// <br/>
    /// An epsilon production is a production rule where the body is a single epsilon symbol.
    /// </summary>
    public bool IsEpsilonProduction => Body.Length == 1 && Body[0] is Epsilon;

    /// <summary>
    /// Gets the length of the production rule's body. The length is the number of symbols in the right-hand side of the rule.
    /// </summary>
    public int Length => Body.Length;

    public static bool operator ==(ProductionRule left, ProductionRule right)
    {
        return left.Head == right.Head 
            && left.Body == right.Body;
    }

    public static bool operator !=(ProductionRule left, ProductionRule right)
    {
        return !(left == right);
    }

    public bool Equals(ProductionRule other)
    {
        return other == this;
    }

    public override bool Equals(object? obj)
    {
        return obj is ProductionRule rule && rule == this;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = hash * 23 + Head.GetHashCode();
            hash = hash * 23 + Body.GetHashCode();

            return hash;
        }
    }

    public override string ToString()
    {
        return ToNotation(NotationType.Sentential);
    }

    public string ToNotation(NotationType notation)
    {
        switch (notation)
        {
            case NotationType.Sentential:
                return ToSententialNotation();

            case NotationType.Bnf:
                return ToBnfNotation();

            case NotationType.Ebnf:
                return ToEbnfNotation();

            case NotationType.EbnfKleene:
                return ToEbnfKleeneNotation();
        }

        throw new InvalidOperationException("Invalid notation type.");
    }

    /*
     * private helpers.
     */

    private void Validate()
    {
        if(Head is null)
        {
            throw new Exception("The head of a production rule cannot be null.");
        }
        if(Body.Length == 0)
        {
            throw new Exception("The body of a production rule cannot be empty.");
        }
        if (Body.Length > 1 && Body.Any(x => x is Epsilon))
        {
            throw new Exception("Invalid production rule. In a production rule's body, epsilon can only appear as the only symbol in the body. Example: A -> ε.");
        }
    }

    private string ToSententialNotation()
    {
        var head = Head.ToNotation(NotationType.Sentential);
        var body = string.Join(" ", Body.Select(x => x.ToNotation(NotationType.Sentential)));

        return $"{head} -> {body}.";
    }

    private string ToBnfNotation()
    {
        var head = Head.ToNotation(NotationType.Bnf);
        var body = string.Join(" ", Body.Select(x => x.ToNotation(NotationType.Bnf)));

        return $"{head} ::= {body} ;";
    }

    private string ToEbnfNotation()
    {
        var head = Head.ToNotation(NotationType.Ebnf);
        var body = string.Join(" ", Body.Select(x => x.ToNotation(NotationType.Ebnf)));

        return $"{head} = {body} ;";
    }

    private string ToEbnfKleeneNotation()
    {
        var head = Head.ToNotation(NotationType.EbnfKleene);
        var body = string.Join(" ", Body.Select(x => x.ToNotation(NotationType.EbnfKleene)));

        return $"{head} = {body} ;";
    }
}
