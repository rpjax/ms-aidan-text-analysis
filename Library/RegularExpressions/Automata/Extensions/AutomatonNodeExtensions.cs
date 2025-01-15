using Aidan.TextAnalysis.RegularExpressions.Automata.Tree;

namespace Aidan.TextAnalysis.RegularExpressions.Automata.Extensions;

/// <summary>
/// Provides extension methods for <see cref="AutomatonState"/> and <see cref="LexemeRegex"/>.
/// </summary>
public static class AutomatonNodeExtensions
{
    /// <summary>
    /// Determines whether the current node is a recursive state with respect to the specified node.
    /// </summary>
    /// <param name="self">The current node.</param>
    /// <param name="node">The node to check for recursion.</param>
    /// <returns><c>true</c> if the current node is a recursive state; otherwise, <c>false</c>.</returns>
    public static bool IsRecursiveState(this AutomatonState self, AutomatonState node)
    {
        if (self.Equals(node))
        {
            return true;
        }

        if (self.Parent is null)
        {
            return false;
        }

        return self.Parent.IsRecursiveState(node);
    }

    /// <summary>
    /// Finds the recursive state with respect to the specified node.
    /// </summary>
    /// <param name="self">The current node.</param>
    /// <param name="node">The node to find.</param>
    /// <returns>The recursive state if found; otherwise, <c>null</c>.</returns>
    public static AutomatonState? FindRecursiveState(this AutomatonState self, AutomatonState node)
    {
        if (self.Equals(node))
        {
            return self;
        }

        if (self.Parent is null)
        {
            return null;
        }

        return self.Parent.FindRecursiveState(node);
    }

    /// <summary>
    /// Determines whether the specified <see cref="AutomatonState"/> is an epsilon state.
    /// </summary>
    /// <param name="node">The <see cref="AutomatonState"/> to check.</param>
    /// <returns><c>true</c> if the node is an epsilon state; otherwise, <c>false</c>.</returns>
    public static bool IsEpsilonState(this AutomatonState node)
    {
        return node.Regexes
            .Any(x => x.Regex.ContainsEpsilon);
    }

    /// <summary>
    /// Gets the lexemes associated with the epsilon transitions of the current node.
    /// </summary>
    /// <param name="node">The <see cref="AutomatonState"/> to check.</param>
    /// <returns>The lexemes associated with the epsilon transitions of the current node.</returns>
    public static LexemeRegex[] GetEpsilonLexemes(this AutomatonState node)
    {
        return node.Regexes
            .Where(x => x.Regex.ContainsEpsilon)
            .ToArray();
    }

    /// <summary>
    /// Computes the alphabet of the current node.
    /// </summary>
    /// <param name="node">The <see cref="AutomatonState"/> to compute the alphabet for.</param>
    /// <returns>An array of characters representing the alphabet of the current node.</returns>
    public static IReadOnlyList<char> ComputeAlphabet(this AutomatonState node)
    {
        if (node.AlphabetCache is not null)
        {
            return node.AlphabetCache;
        }

        node.AlphabetCache = node.Regexes
            .SelectMany(x => x.ComputeAlphabet())
            .Distinct()
            .OrderBy(x => x)
            .ToArray();

        return node.AlphabetCache;
    }

}
