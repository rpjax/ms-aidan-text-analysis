using Aidan.TextAnalysis.GDef.Tokenization;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Parsing.Components;

namespace Aidan.TextAnalysis.GDef;

/// <summary>
/// Represents a translator for converting a Concrete Syntax Tree (CST) to a grammar.
/// </summary>
public class GDefTranslator
{
    /// <summary>
    /// Enum representing the type of symbol.
    /// </summary>
    enum SymbolType
    {
        Terminal,
        NonTerminal,
        Macro
    }

    /// <summary>
    /// Translates a CST root node to a Grammar object.
    /// </summary>
    /// <param name="root">The root node of the CST.</param>
    /// <returns>A Grammar object.</returns>
    public static Grammar TranslateGrammar(CstRootNode root)
    {
        var productions = root.Children
            .Where(x => x.Type == CstNodeType.Internal)
            .Select(x => (CstInternalNode)x)
            .Select(TranslateProductionRule)
            .ToArray();
        ;

        var start = productions.First().Head;

        return new Grammar(start, productions);
    }

    /// <summary>
    /// Translates a CST internal node representing a production rule to a ProductionRule object.
    /// </summary>
    /// <param name="node">The CST internal node.</param>
    /// <returns>A ProductionRule object.</returns>
    /// <exception cref="Exception">Thrown when the node name is not "production".</exception>
    /// <exception cref="InvalidOperationException">Thrown when the node has an invalid structure.</exception>
    public static ProductionRule TranslateProductionRule(CstInternalNode node)
    {
        if (node.Name != "production")
        {
            throw new Exception();
        }
        if (node.Children.Length < 4)
        {
            throw new InvalidOperationException();
        }

        var children = node.Children;

        if (children[0] is not CstLeafNode leaf)
        {
            throw new InvalidOperationException();
        }

        if (leaf.Token.Type != GDefLexemes.Identifier)
        {
            throw new InvalidOperationException();
        }

        var head = new NonTerminal(leaf.Token.Value.ToString());

        var bodyNodes = children
            .Skip(2)
            .Take(children.Length - 3)
            .ToArray();

        var body = TranslateSentence(bodyNodes);

        return new ProductionRule(head, body);
    }

    /// <summary>
    /// Translates an array of CST nodes to a Sentence object.
    /// </summary>
    /// <param name="nodes">The array of CST nodes.</param>
    /// <returns>A Sentence object.</returns>
    public static Sentence TranslateSentence(CstNode[] nodes)
    {
        var symbols = nodes
            .Where(x => x is CstInternalNode node ? !node.IsEpsilon : true)
            .SelectMany(x => TranslateSymbol(x))
            .ToArray();

        return new Sentence(symbols);
    }

    /// <summary>
    /// Translates a CST node to a collection of ISymbol objects.
    /// </summary>
    /// <param name="node">The CST node.</param>
    /// <returns>A collection of ISymbol objects.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the node is not a valid internal node.</exception>
    public static IEnumerable<ISymbol> TranslateSymbol(CstNode node)
    {
        if (node is not CstInternalNode internalNode)
        {
            throw new InvalidOperationException();
        }

        switch (GetSymbolType(internalNode))
        {
            case SymbolType.Terminal:
                return TranslateTerminal(internalNode);

            case SymbolType.NonTerminal:
                return TranslateNonTerminal(internalNode);

            case SymbolType.Macro:
                return TranslateMacro(internalNode);

            default:
                throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Translates a CST internal node representing a terminal to a collection of ISymbol objects.
    /// </summary>
    /// <param name="node">The CST internal node.</param>
    /// <returns>A collection of ISymbol objects.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the node is not a valid terminal node.</exception>
    public static IEnumerable<ISymbol> TranslateTerminal(CstInternalNode node)
    {
        if (node.Name != "terminal")
        {
            throw new InvalidOperationException();
        }

        var length = node.Children.Length;

        if (length == 1)
        {
            if (node.Children[0] is not CstLeafNode leaf)
            {
                throw new InvalidOperationException();
            }

            var value = leaf.Token.Value.ToString();
            /* trim the string quotes*/
            var normalizedValue = value.Substring(1, value.Length - 2);

            yield return new Terminal(normalizedValue);
            yield break;
        }

        if (length == 2)
        {
            if (node.Children[1] is not CstLeafNode leaf)
            {
                throw new InvalidOperationException();
            }

            yield return new Terminal(leaf.Token.Value.ToString());
            yield break;
        }

        throw new InvalidOperationException();
    }

    /// <summary>
    /// Translates a CST internal node representing a non-terminal to a collection of ISymbol objects.
    /// </summary>
    /// <param name="node">The CST internal node.</param>
    /// <returns>A collection of ISymbol objects.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the node is not a valid non-terminal node.</exception>
    public static IEnumerable<ISymbol> TranslateNonTerminal(CstInternalNode node)
    {
        if (node.Name != "non_terminal")
        {
            throw new InvalidOperationException();
        }

        if (node.Children.Length != 1)
        {
            throw new InvalidOperationException();
        }

        if (node.Children[0] is not CstLeafNode leaf)
        {
            throw new InvalidOperationException();
        }

        if (leaf.Token.Type != GDefLexemes.Identifier)
        {
            throw new InvalidOperationException();
        }

        yield return new NonTerminal(leaf.Token.Value.ToString());
    }

    /// <summary>
    /// Translates a CST internal node representing a macro to a collection of ISymbol objects.
    /// </summary>
    /// <param name="node">The CST internal node.</param>
    /// <returns>A collection of ISymbol objects.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the node is not a valid macro node.</exception>
    public static IEnumerable<ISymbol> TranslateMacro(CstInternalNode node)
    {
        if (node.Name != "macro")
        {
            throw new InvalidOperationException();
        }

        if (node.Children.Length != 1)
        {
            throw new InvalidOperationException();
        }

        if (node.Children[0] is not CstInternalNode macroNode)
        {
            throw new InvalidOperationException();
        }

        switch (GetMacroType(macroNode))
        {
            case MacroType.Grouping:
                return TranslateGroupingMacro(macroNode);

            case MacroType.Option:
                return TranslateOptionMacro(macroNode);

            case MacroType.Repetition:
                return TranslateRepetitionMacro(macroNode);

            case MacroType.Alternative:
                return TranslateAlternativeMacro(macroNode);

            default:
                throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Translates a CST internal node representing a grouping macro to a collection of ISymbol objects.
    /// </summary>
    /// <param name="node">The CST internal node.</param>
    /// <returns>A collection of ISymbol objects.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the node is not a valid grouping macro node.</exception>
    public static IEnumerable<ISymbol> TranslateGroupingMacro(CstInternalNode node)
    {
        if (node.Name != "grouping")
        {
            throw new InvalidOperationException();
        }

        if (node.Children.Length < 3)
        {
            throw new InvalidOperationException();
        }

        var children = node.Children
            .Skip(1)
            .Take(node.Children.Length - 2)
            .ToArray();

        yield return new GroupingMacro(TranslateSentence(children).ToArray());
    }

    /// <summary>
    /// Translates a CST internal node representing an option macro to a collection of ISymbol objects.
    /// </summary>
    /// <param name="node">The CST internal node.</param>
    /// <returns>A collection of ISymbol objects.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the node is not a valid option macro node.</exception>
    public static IEnumerable<ISymbol> TranslateOptionMacro(CstInternalNode node)
    {
        if (node.Name != "option")
        {
            throw new InvalidOperationException();
        }

        if (node.Children.Length < 3)
        {
            throw new InvalidOperationException();
        }

        var children = node.Children
            .Skip(1)
            .Take(node.Children.Length - 2)
            .ToArray();

        yield return new OptionMacro(TranslateSentence(children).ToArray());
    }

    /// <summary>
    /// Translates a CST internal node representing a repetition macro to a collection of ISymbol objects.
    /// </summary>
    /// <param name="node">The CST internal node.</param>
    /// <returns>A collection of ISymbol objects.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the node is not a valid repetition macro node.</exception>
    public static IEnumerable<ISymbol> TranslateRepetitionMacro(CstInternalNode node)
    {
        if (node.Name != "repetition")
        {
            throw new InvalidOperationException();
        }

        if (node.Children.Length < 3)
        {
            throw new InvalidOperationException();
        }

        var children = node.Children
            .Skip(1)
            .Take(node.Children.Length - 2)
            .ToArray();

        yield return new RepetitionMacro(TranslateSentence(children).ToArray());
    }

    /// <summary>
    /// Translates a CST internal node representing an alternative macro to a collection of ISymbol objects.
    /// </summary>
    /// <param name="node">The CST internal node.</param>
    /// <returns>A collection of ISymbol objects.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the node is not a valid alternative macro node.</exception>
    public static IEnumerable<ISymbol> TranslateAlternativeMacro(CstInternalNode node)
    {
        if (node.Name != "alternative")
        {
            throw new InvalidOperationException();
        }

        yield return new PipeMacro();
    }

    /// <summary>
    /// Determines the symbol type of a CST internal node.
    /// </summary>
    /// <param name="node">The CST internal node.</param>
    /// <returns>The symbol type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the node name is not recognized.</exception>
    private static SymbolType GetSymbolType(CstInternalNode node)
    {
        switch (node.Name)
        {
            case "terminal":
                return SymbolType.Terminal;

            case "non_terminal":
                return SymbolType.NonTerminal;

            case "macro":
                return SymbolType.Macro;

            default:
                throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Determines the macro type of a CST internal node.
    /// </summary>
    /// <param name="node">The CST internal node.</param>
    /// <returns>The macro type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the node name is not recognized.</exception>
    public static MacroType GetMacroType(CstInternalNode node)
    {
        switch (node.Name)
        {
            case "grouping":
                return MacroType.Grouping;

            case "option":
                return MacroType.Option;

            case "repetition":
                return MacroType.Repetition;

            case "alternative":
                return MacroType.Alternative;

            default:
                throw new InvalidOperationException();
        }
    }

}
