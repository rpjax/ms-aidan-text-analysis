using System.Text;
using Aidan.TextAnalysis.RegularExpressions.Ast;

namespace Aidan.TextAnalysis.RegularExpressions.Derivative;

/// <summary>
/// Stores the history of derivative calculations and simplifications for debugging purposes.
/// </summary>
public class CalculatorHistory
{
    /// <summary>
    /// Gets the list of records in the history.
    /// </summary>
    public List<object> Records { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CalculatorHistory"/> class.
    /// </summary>
    public CalculatorHistory()
    {
        Records = new List<object>();
    }

    /// <summary>
    /// Adds a derivative calculation to the history.
    /// </summary>
    /// <param name="regex">The regular expression node.</param>
    /// <param name="character">The character with respect to which the derivative is calculated.</param>
    /// <param name="derivative">The derivative of the regular expression node.</param>
    public void AddDerivative(RegExpr regex, char character, RegExpr derivative)
    {
        Records.Add(new Derivation(regex, character, derivative));
    }

    /// <summary>
    /// Adds a simplification to the history.
    /// </summary>
    /// <param name="regex">The original regular expression node.</param>
    /// <param name="simplifiedRegex">The simplified regular expression node.</param>
    public void AddSimplification(RegExpr regex, RegExpr simplifiedRegex)
    {
        Records.Add(new Simplification(regex, simplifiedRegex));
    }

    /// <summary>
    /// Clears the history.
    /// </summary>
    public void Clear()
    {
        Records.Clear();
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var record in Records)
        {
            sb.AppendLine(record.ToString());
        }

        return sb.ToString();
    }
}
