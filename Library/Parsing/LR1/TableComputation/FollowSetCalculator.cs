using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Parsing.LR1.TableComputation;

/// <summary>
/// Represents the follow set of a grammar.
/// </summary>
public class FollowSet
{

}

/// <summary>
/// Calculates the follow sets for a given grammar.
/// </summary>
public class FollowSetCalculator
{
    /// <summary>
    /// Gets the grammar for which the follow sets are calculated.
    /// </summary>
    private IGrammar Grammar { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FollowSetCalculator"/> class with the specified grammar.
    /// </summary>
    /// <param name="grammar">The grammar for which to calculate the follow sets.</param>
    public FollowSetCalculator(IGrammar grammar)
    {
        Grammar = grammar;
    }
}
