using Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;

namespace Aidan.TextAnalysis.RegularExpressions.Automata.Extensions;

/// <summary>
/// Provides extension methods for <see cref="AutomatonNode"/> and <see cref="LexemeRegex"/>.
/// </summary>
public static class AutomatonNodeExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="AutomatonNode"/> can produce epsilon (the empty string).
    /// </summary>
    /// <remarks> 
    /// This method checks if the state can produce epsilon (the empty string), not if the state itself is epsilon.
    /// <br/> Example: <c>/a|ε/</c> or <c>/a*/</c> can produce epsilon, but are not epsilon themselves.
    /// <br/> If <see cref="ContainsEpsilonRegex"/> returns <c>true</c>, this method will also return <c>true</c>, 
    /// but the reverse is not guaranteed.
    /// <br/> Example: <c>/ε/</c> is an epsilon regex, while <c>/a|ε/</c> is not an epsilon regex, but both will 
    /// return <c>true</c> for this method.
    /// </remarks>
    /// <param name="node">The automaton node to check.</param>
    /// <returns><c>true</c> if the node can produce epsilon; otherwise, <c>false</c>.</returns>
    public static bool CanProduceEpsilon(this AutomatonNode node)
    {
        return node.Regexes
            .Any(x => x.Regex.ContainsEpsilon);
    }

    /// <summary>
    /// Determines whether the specified <see cref="AutomatonNode"/> contains an epsilon regex.
    /// </summary>
    /// <remarks> 
    /// This method checks if the node contains a regex that is epsilon, meaning the entire regex is the empty string.
    /// <br/> Example: <c>/ε/</c> is an epsilon regex, but <c>/a|ε/</c> is not.
    /// </remarks>
    /// <param name="node">The automaton node to check.</param>
    /// <returns><c>true</c> if the node contains an epsilon regex; otherwise, <c>false</c>.</returns>
    public static bool ContainsEpsilonRegex(this AutomatonNode node)
    {
        return node.Regexes
            .Any(x => x.Regex.IsEpsilon());
    }

    /// <summary>
    /// Determines whether the specified <see cref="AutomatonNode"/> is an epsilon state.
    /// </summary>
    /// <remarks> 
    /// A node is considered an epsilon state if it contains exactly one regex, and that regex is epsilon.
    /// This means that the state can only be epsilon itself, not just produce epsilon.
    /// <br/> Example: <c>/ε/</c> qualifies as an epsilon state, but <c>/a|ε/</c> does not.
    /// </remarks>
    /// <param name="node">The automaton node to check.</param>
    /// <returns><c>true</c> if the node is an epsilon state; otherwise, <c>false</c>.</returns>
    public static bool IsEpsilonState(this AutomatonNode node)
    {
        return node.Regexes.Length == 1 && node.Regexes[0].Regex.IsEpsilon();
    }
}
