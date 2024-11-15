using Aidan.TextAnalysis.RegularExpressions.Ast.Extensions;

namespace Aidan.TextAnalysis.RegularExpressions.Automata.Extensions;

/// <summary>
/// Provides extension methods for <see cref="AutomatonNode"/> and <see cref="LexemeRegex"/>.
/// </summary>
public static class AutomatonNodeExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="AutomatonNode"/> is an epsilon state.
    /// </summary>
    /// <param name="node">The <see cref="AutomatonNode"/> to check.</param>
    /// <returns><c>true</c> if the node is an epsilon state; otherwise, <c>false</c>.</returns>
    public static bool IsEpsilonState(this AutomatonNode node)
    {
        return node.Regexes
            .Any(x => x.Regex.ContainsEpsilon);
    }

    /// <summary>
    /// Gets the lexemes associated with the epsilon transitions of the current node.
    /// </summary>
    /// <param name="node">The <see cref="AutomatonNode"/> to check.</param>
    /// <returns>The lexemes associated with the epsilon transitions of the current node.</returns>
    public static LexemeRegex[] GetEpsilonLexemes(this AutomatonNode node)
    {
        return node.Regexes
            .Where(x => x.Regex.ContainsEpsilon)
            .ToArray();
    }
}
