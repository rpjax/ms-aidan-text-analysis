namespace Aidan.TextAnalysis.RegularExpressions.Automata;

/// <summary>
/// Represents a node in an automaton used for regular expression analysis.
/// </summary>
public class AutomatonNode : IEquatable<AutomatonNode>
{
    /// <summary>
    /// Gets the parent node of the current node.
    /// </summary>
    public AutomatonNode? Parent { get; private set; }

    /// <summary>
    /// Gets the regexes associated with the current node.
    /// </summary>
    public LexemeRegex[] Regexes { get; }

    /// <summary>
    /// Gets the transitions from the current node to other nodes.
    /// </summary>
    public List<AutomatonTransition> Transitions { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutomatonNode"/> class.
    /// </summary>
    /// <param name="regexes">The regexes associated with the node.</param>
    /// <param name="transitions">The transitions from the current node to other nodes.</param>
    public AutomatonNode(
        LexemeRegex[] regexes,
        IEnumerable<AutomatonTransition>? transitions = null)
    {
        Regexes = regexes;
        Transitions = transitions?.ToList() ?? new List<AutomatonTransition>();
    }

    /// <summary>
    /// Returns a string representation of the current node.
    /// </summary>
    /// <returns>A string representation of the current node.</returns>
    public override string ToString()
    {
        var stringifiedLexemes = Regexes.Select(x => $"/{x.Regex}/ ({x.Name})");
        return string.Join(" | ", stringifiedLexemes);
    }

    /// <summary>
    /// Determines whether the specified <see cref="AutomatonNode"/> is equal to the current <see cref="AutomatonNode"/>.
    /// </summary>
    /// <param name="other">The <see cref="AutomatonNode"/> to compare with the current <see cref="AutomatonNode"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="AutomatonNode"/> is equal to the current <see cref="AutomatonNode"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(AutomatonNode? other)
    {
        if (other is null)
        {
            return false;
        }

        if (Regexes.Length != other.Regexes.Length)
        {
            return false;
        }

        for (int i = 0; i < Regexes.Length; i++)
        {
            if (!Regexes[i].Equals(other.Regexes[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current <see cref="AutomatonNode"/>.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;

            foreach (var item in Regexes)
            {
                hash = hash * 23 + item.GetHashCode();
            }

            return hash;
        }
    }

    /// <summary>
    /// Adds a transition from the current node to another node.
    /// </summary>
    /// <param name="transition">The transition to add.</param>
    public void AddTransition(AutomatonTransition transition)
    {
        transition.NextState.Parent = this;
        Transitions.Add(transition);
    }

    /// <summary>
    /// Adds multiple transitions from the current node to other nodes.
    /// </summary>
    /// <param name="transitions">The transitions to add.</param>
    public void AddChildren(IEnumerable<AutomatonTransition> transitions)
    {
        foreach (var item in transitions)
        {
            AddTransition(item);
        }
    }

    /// <summary>
    /// Determines whether the current node is a recursive state with respect to the specified node.
    /// </summary>
    /// <param name="node">The node to check for recursion.</param>
    /// <returns><c>true</c> if the current node is a recursive state; otherwise, <c>false</c>.</returns>
    public bool IsRecursiveState(AutomatonNode node)
    {
        if (Equals(node))
        {
            return true;
        }

        if (Parent is null)
        {
            return false;
        }

        return Parent.IsRecursiveState(node);
    }

    /// <summary>
    /// Finds the recursive state with respect to the specified node.
    /// </summary>
    /// <param name="node">The node to check for recursion.</param>
    /// <returns>The recursive state if found; otherwise, <c>null</c>.</returns>
    public AutomatonNode? FindRecursiveState(AutomatonNode node)
    {
        if (Equals(node))
        {
            return this;
        }

        if (Parent is null)
        {
            return null;
        }

        return Parent.FindRecursiveState(node);
    }
}
