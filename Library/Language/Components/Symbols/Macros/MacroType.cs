namespace Aidan.TextAnalysis.Language.Components;

/*
    Macros are expanded recursively, one at a time, left to right. The order of expansion is as follows:
    1- Pipe macros are expanded into alternation macros.
    2- All other macros are expanded, left to right, until only alternation macros, or no macros are left.
    3- Alternation macros are expanded last.
    4- The process is repeated until no macros are left in the production rules.

    # Option macros are expanded following this rule:

    S -> A [ b ] C
    S -> A C | A b C

    # Repetition macros are expanded following this rule:

    S -> A { b } C
    S' -> b S' | ε
    S -> A S' C

    # Pipe macros are expanded by transforming them into a single alternation macro, using the pipes in the sentece as split points. Each alternative specified by the pipes becomes a child sentence of a newly created alternation macro:
    
    S -> Ab | Bc | Cd
    is transformed into:
    S -> (Ab, Bc, Cd)

    Where the sentence (Ab, Bc, Cd) is a single symbol (alternation macro), and Ab, Bc, Cd are child sentences of the alternation macro.
    This ensures that nested macros are expanded correctly.

    # Alternation macros within a sentence are expanded following this rule:      
    1- Determine the prefix of the sentence that contains the alternation macro, represented here by alpha (α).
    2- Determine the suffix of the sentence that contains the alternation macro, represented here by beta (β).
    3- For each child in the alternation macro, create a new production by concatenating the prefix, the child, and the suffix.

    For example:
    S -> Fg (Ab, Bc, Cd) Ef

    Where:
    α = Fg
    β = Ef

    Is transformed into:
    S -> Fg Ab Ef 
    S -> Fg Bc Ef 
    S -> Fg Cd Ef

    # Some complex examples:

    - Repetition macro, with pipe macro inside:
    S -> A { a | b } C

    First the pipe macro is expanded:
    S -> A { (a, b) } C

    Then, the repetition macro is expanded:
    S -> A S' C
    S' -> (a, b) S' 
    S' -> ε

    Then the alternation macro is expanded:
    S' -> (a, b) S' 
    S' -> a S' 
    S' -> b S'

    Observe that in this case alpha is empty, and beta is S'. The alternation macro is expanded by concatenating the prefix, the child, and the suffix.

    The final grammar should look like this:
    S -> A S' C
    S' -> a S'
    S' -> b S'
    S' -> ε

*/

/// <summary>
/// Represents the type of a macro symbol. 
/// </summary>
public enum MacroType
{
    /// <summary>
    /// Represents a grouping macro.
    /// </summary>
    Grouping,

    /// <summary>
    /// Represents an option macro.
    /// </summary>
    Option,

    /// <summary>
    /// Represents a repetition macro.
    /// </summary>
    Repetition,

    /// <summary>
    /// Represents a pipe macro. 
    /// </summary>
    Alternative,

    /// <summary>
    /// Represents an expanded alternative macro.
    /// </summary>
    ExpandedAlternative
}
