using System.Collections;
using Aidan.TextAnalysis.Helpers;
using Aidan.TextAnalysis.Language.Components.Symbols;
using Aidan.TextAnalysis.Language.Extensions;

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
public interface IProductionRule : IEquatable<IProductionRule>
{
    /// <summary>
    /// Gets the head of the production rule.
    /// </summary>
    INonTerminal Head { get; }

    /// <summary>
    /// Gets the body of the production rule.
    /// </summary>
    ISentence Body { get; }

    /// <summary>
    /// Gets a value based hash for the production.
    /// </summary>
    /// <returns></returns>
    int GetHashCode();
}

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
public class ProductionRule : IProductionRule
{
    /// <summary>
    /// The head of the production rule. It is the non-terminal symbol that is being replaced. The left-hand side of the rule (LHS).
    /// </summary>
    public INonTerminal Head { get; }

    /// <summary>
    /// The body of the production rule. It is the sequence of symbols that replace the head. The right-hand side of the rule (RHS).
    /// </summary>
    public ISentence Body { get; }

    /// <summary>
    /// The number of terminal symbols in the body of the production rule.
    /// </summary>
    public int TerminalCount { get; }

    /// <summary>
    /// The number of non-terminal symbols in the body of the production rule.
    /// </summary>
    public int NonTerminalCount { get; }

    /// <summary>
    /// Determines whether the production rule is an epsilon production. 
    /// <br/>
    /// An epsilon production is a production rule where the body is a single epsilon symbol.
    /// </summary>
    public bool IsEpsilonProduction { get; }

    /// <summary>
    /// Gets the length of the production rule's body. The length is the number of symbols in the right-hand side of the rule.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Creates a new instance of <see cref="ProductionRule"/>.
    /// </summary>
    /// <param name="head"> The head of the production rule. </param>
    /// <param name="body"> The body of the production rule. </param>
    public ProductionRule(INonTerminal head, params ISymbol[] body)
    {
        Head = head;
        Body = new Sentence(body);
        TerminalCount = Body.Count(x => x.IsTerminal());
        NonTerminalCount = Body.Count(x => x.IsNonTerminal());
        IsEpsilonProduction = Body.Length == 1 && Body[0] is Epsilon;
        Length = Body.Length;
        Validate();
    }

    /// <summary>
    /// Creates a new instance of <see cref="ProductionRule"/>.
    /// </summary>
    /// <param name="head"> The head of the production rule. </param>
    /// <param name="body"> The body of the production rule. </param>
    public ProductionRule(INonTerminal head, IEnumerable<ISymbol> body)

    {
        Head = head;
        Body = new Sentence(body);
        TerminalCount = Body.Count(x => x.IsTerminal());
        NonTerminalCount = Body.Count(x => x.IsNonTerminal());
        IsEpsilonProduction = Body.Length == 1 && Body[0] is Epsilon;
        Length = Body.Length;
        Validate();
    }

    /// <summary>
    /// Returns a string that represents the current production rule.
    /// </summary>
    /// <returns>A string that represents the current production rule.</returns>
    public override string ToString()
    {
        var head = Head.ToString();
        var body = string.Join(" ", Body.Select(x => x.ToString()));

        return $"{head} -> {body}.";
    }

    /// <summary>
    /// Gets a value based hash for the production.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return HashHelper.ComputeHash(Head, Body);
    }

    /// <summary>
    /// Determines whether the specified production rule is equal to the current production rule.
    /// </summary>
    /// <param name="other">The production rule to compare with the current production rule.</param>
    /// <returns>true if the specified production rule is equal to the current production rule; otherwise, false.</returns>
    public bool Equals(IProductionRule? other)
    {
        return true
            && Head.Equals(other?.Head)
            && Body.Equals(other?.Body);
    }

    /*
     * private helpers.
     */

    /// <summary>
    /// Validates the production rule.
    /// </summary>
    /// <exception cref="Exception">Thrown when the head is null, the body is empty, or the body contains an invalid epsilon symbol.</exception>
    private void Validate()
    {
        if (Head is null)
        {
            throw new Exception("The head of a production rule cannot be null.");
        }
        if (Body.Length == 0)
        {
            throw new Exception("The body of a production rule cannot be empty.");
        }
        if (Body.Length > 1 && Body.Any(x => x is Epsilon))
        {
            throw new Exception("Invalid production rule. In a production rule's body, epsilon can only appear as the only symbol in the body. Example: A -> ε.");
        }
    }
}

/// <summary>
/// Represents a collection of production rules.
/// </summary>
public class ProductionCollection : IReadOnlyList<IProductionRule>
{
    /// <summary>
    /// Gets the production rules in the collection.
    /// </summary>
    private IProductionRule[] Productions { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductionCollection"/> class with the specified production rules.
    /// </summary>
    /// <param name="productions">The production rules to include in the collection.</param>
    public ProductionCollection(params IProductionRule[] productions)
    {
        Productions = productions;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductionCollection"/> class with the specified production rules.
    /// </summary>
    /// <param name="productions">The production rules to include in the collection.</param>
    public ProductionCollection(IEnumerable<IProductionRule> productions)
    {
        Productions = productions.ToArray();
    }

    /// <summary>
    /// Gets the production rule at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the production rule to get.</param>
    /// <returns>The production rule at the specified index.</returns>
    public IProductionRule this[int index] => Productions[index];

    /// <summary>
    /// Gets the number of production rules in the collection.
    /// </summary>
    public int Count => Productions.Length;

    /// <summary>
    /// Returns an enumerator that iterates through the production rules in the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the production rules in the collection.</returns>
    public IEnumerator<IProductionRule> GetEnumerator()
    {
        return Productions.AsEnumerable().GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the production rules in the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the production rules in the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return Productions.GetEnumerator();
    }
}
