using Aidan.Core;
using Aidan.Core.Exceptions;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Transformations;

namespace Aidan.TextAnalysis.Language.Extensions;

public static partial class ProductionSetTransformationsExtensions
{
    /*
        Main API methods.
    */
    public static void AutoClean(this ProductionSet set)
    {
        set.ExpandMacros();
        set.RemoveUnreachableProductions();
        set.ExpandUnitProductions();
        set.RemoveDuplicates();
    }

    public static void RecursiveAutoClean(this ProductionSet set)
    {
        var counter = 0;

        while (true)
        {
            set.ExpandMacros();
            set.RemoveUnreachableProductions();
            set.ExpandUnitProductions();
            set.RemoveDuplicates();

            var transformationsCreated = set.Transformations.Count - counter;

            if (transformationsCreated == 0)
            {
                break;
            }

            counter = set.Transformations.Count;
        }
    }

    public static void AutoFix(this ProductionSet set)
    {
        set.RemoveLeftRecursion();
        set.LeftFactorProductions();
    }

    public static void RecursiveAutoFix(this ProductionSet set)
    {
        var counter = 0;

        while (true)
        {
            set.RemoveLeftRecursion();
            set.LeftFactorProductions();

            var transformationsCreated = set.Transformations.Count - counter;

            if (transformationsCreated == 0)
            {
                break;
            }

            counter = set.Transformations.Count;
        }
    }

    public static void AutoTransformLL1(this ProductionSet set)
    {
        var counter = 0;

        while (true)
        {
            set.RecursiveAutoClean();
            set.AutoFix();

            var transformationsCreated = set.Transformations.Count - counter;

            if (transformationsCreated == 0)
            {
                break;
            }

            counter = set.Transformations.Count;
        }

        var errors = set.GetLL1Errors();

        if (errors.Length > 0)
        {
            throw new ErrorException(errors);
        }
    }

    public static void ExpandMacros(this ProductionSet set)
    {
        new MacroExpansion()
            .ExecuteTransformations(set);
    }

    public static void RemoveLeftRecursion(this ProductionSet set)
    {
        new LeftRecursionRemoval()
            .ExecuteTransformations(set);
    }

    public static void RemoveDuplicates(this ProductionSet set)
    {
        new DuplicateProductionsRemoval()
            .ExecuteTransformations(set);
    }

    public static void RemoveUnreachableProductions(this ProductionSet set)
    {
        new RemoveUnreachableProductions()
            .ExecuteTransformations(set);
    }

    public static void ExpandUnitProductions(this ProductionSet set)
    {
        new UnitProductionExpansion()
            .ExecuteTransformations(set);
    }

    public static void LeftFactorProductions(this ProductionSet set)
    {
        new TerminalLeftFactorization()
            .ExecuteTransformations(set);

        new NonTerminalLeftFactorization()
            .ExecuteTransformations(set);
    }

    /*
     * LR1 specific transformations.
     */

    public static void AutoTransformLR1(this ProductionSet set)
    {
        set.ExpandMacros();
        set.AugmentStart();

        var errors = set.GetLR1Errors();

        if (errors.Length > 0)
        {
            throw new ErrorException(errors);
        }
    }


    public static void AugmentStart(this ProductionSet set)
    {
        new AugmentGrammarTransformation()
            .ExecuteTransformations(set);
    }

    
}
