using Aidan.Core;
using Aidan.Core.Exceptions;
using Aidan.TextAnalysis.GDef.Components;
using Aidan.TextAnalysis.GDef.Tokenization;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Parsing.Extensions;
using Aidan.TextAnalysis.Parsing.Tree;
using Aidan.TextAnalysis.RegularExpressions.Tree;

namespace Aidan.TextAnalysis.GDef;

/// <summary>
/// Represents a translator for converting a Concrete Syntax Tree (CST) to a grammar.
/// </summary>
public class GDefTranslator
{
    public static Charset DefaultCharset { get; } = Charset.Compute(CharsetType.ExtendedAscii);

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
    public static GdefGrammar TranslateGrammar(CstRootNode root)
    {
        var lexemes = TranslateLexemes(root);
        var ignoredChars = TranslateIgnoredChars(root);
        var productions = root.Children
            .Where(x => x.Type == CstNodeType.Internal)
            .Where(x => x.Name == "production")
            .Select(x => (CstInternalNode)x)
            .Select(TranslateProductionRule)
            .ToArray();

        var start = productions.First().Head;

        return new GdefGrammar(
            lexemes: lexemes,
            ignoredChars: ignoredChars,
            productions: productions,
            start: start);
    }

    private static CstInternalNode? GetLexerSettings(CstRootNode node)
    {
        return node.Children
            .FirstOrDefault(x => x.Name == "lexer_settings") as CstInternalNode;
    }

    private static GDefLexeme[] TranslateLexemes(CstRootNode node)
    {
        var lexerSettings = GetLexerSettings(node);

        if (lexerSettings is null)
        {
            return Array.Empty<GDefLexeme>();
        }

        var lexemeLiterals = node.Children
            .Skip(1)
            .SelectMany(x => x.GetAllTerminals())
            .Select(x => new { Value = x.GetValue().ToString() })
            .Select(x => x.Value.Substring(1, x.Value.Length - 2))
            .Select(x => new GDefLexeme(
                isIgnored: false,
                charset: DefaultCharset,
                name: x,
                pattern: RegExpr.FromString(x)))
            .ToArray();

        var lexemeDeclarations = lexerSettings.Children
            .Where(x => x.Name == "lexeme_declaration")
            .Cast<CstInternalNode>()
            .Select(x => TranslateLexemeDeclaration(x))
            .ToArray();

        var lexemes = lexemeLiterals
            .Concat(lexemeDeclarations)
            .ToArray();

        var nameGroups = lexemes
            .GroupBy(x => x.Name)
            .Where(x => x.Count() > 1)
            .ToArray();

        if (nameGroups.Any())
        {
            var builder = Error.Create()
                .WithTitle("Duplicate lexemes.");

            foreach (var nameGroup in nameGroups)
            {
                builder.WithDetail(nameGroup.Key, $"{nameGroup.Count()} occurrences.");
            }

            throw new ErrorException(builder.Build());
        }

        return lexemes;
    }

    private static GDefLexeme TranslateLexemeDeclaration(CstInternalNode node)
    {
        var annotationList = node.Children
            .Where(x => x.Name == "lexeme_annotation_list")
            .FirstOrDefault();

        CstNode[] children = node.Children;
        CstLeafNode nameNode;
        CstLeafNode patternNode;
        LexemeAnnotation? annotation;

        if (annotationList is null)
        {
            annotation = null;
            nameNode = children[1].AsLeaf();
            patternNode = children[3].AsLeaf();
        }
        else
        {
            annotation = TranslateLexemesAnnotation(children[0].AsInternal());
            nameNode = children[2].AsLeaf();
            patternNode = children[4].AsLeaf();
        }

        string pattern = patternNode.Token.Value.ToString();
        /* trim the string quotes*/
        string normalizedPattern = patternNode.GetValue()
            .ToString()
            .Substring(1, pattern.Length - 2);

        return new GDefLexeme(
            isIgnored: annotation?.IsIgnored ?? false,
            charset: annotation?.Charset ?? DefaultCharset,
            name: nameNode.GetValue().ToString(),
            pattern: RegExpr.FromPattern(normalizedPattern));
    }

    private static LexemeAnnotation TranslateLexemesAnnotation(CstInternalNode node)
    {
        var annotationNodes = node.Children
            .Where(x => x.Name == "lexeme_annotation")
            .Cast<CstInternalNode>()
            .ToArray();

        Charset charset = DefaultCharset;
        bool isIgnored = false;

        foreach (var annotationNode in annotationNodes)
        {
            var key = annotationNode.Children[0].AsLeaf().GetValue().ToString();
            var value = annotationNode.Children[2].AsLeaf().GetValue().ToString();

            switch (key)
            {
                case "charset":
                    var normalizedValue = value.Substring(1, value.Length - 2);
                    switch (normalizedValue)
                    {
                        case "ascii":
                            charset = Charset.Compute(CharsetType.Ascii);
                            break;

                        case "extended ascii":
                            charset = Charset.Compute(CharsetType.ExtendedAscii);
                            break;

                        case "utf8":
                            charset = Charset.Compute(CharsetType.Utf8);
                            break;

                        default:
                            throw new Exception();
                    }
                    break;

                case "ignore":
                    switch (value)
                    {
                        case "true":
                            isIgnored = true;
                            break;

                        case "false":
                            isIgnored = false;
                            break;

                        default:
                            throw new Exception();
                    }
                    break;

                default:
                    throw new Exception();
            }
        }

        return new LexemeAnnotation(charset, isIgnored);
    }

    private static char[] TranslateIgnoredChars(CstRootNode node)
    {
        var lexerSettings = GetLexerSettings(node);

        if (lexerSettings is null)
        {
            return Array.Empty<char>();
        }

        var ignoredCharsDeclaration = lexerSettings.Children
            .FirstOrDefault(x => x.Name == "ignored_chars_declaration");

        if (ignoredCharsDeclaration is null)
        {
            return Array.Empty<char>();
        }

        var stringLeaf = ignoredCharsDeclaration.AsInternal().Children.ElementAt(2).AsLeaf();
        var tokenValue = stringLeaf.GetValue().ToArray();
        var chars = tokenValue
            .Skip(1)
            .Take(tokenValue.Length - 2)
            .ToArray();

        return chars;
    }

    /// <summary>
    /// Translates a CST internal node representing a production rule to a ProductionRule object.
    /// </summary>
    /// <param name="node">The CST internal node.</param>
    /// <returns>A ProductionRule object.</returns>
    /// <exception cref="Exception">Thrown when the node name is not "production".</exception>
    /// <exception cref="InvalidOperationException">Thrown when the node has an invalid structure.</exception>
    private static ProductionRule TranslateProductionRule(CstInternalNode node)
    {
        if (node.Name != "production")
        {
            throw new InvalidOperationException();
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

        if (leaf.Token.Type != GDefTokenizerBuilder.Identifier)
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
    private static Sentence TranslateSentence(CstNode[] nodes)
    {
        var symbols = nodes
            .Where(x => x is CstInternalNode node)
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
    private static IEnumerable<ISymbol> TranslateSymbol(CstNode node)
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
    private static IEnumerable<ISymbol> TranslateTerminal(CstInternalNode node)
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
    private static IEnumerable<ISymbol> TranslateNonTerminal(CstInternalNode node)
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

        if (leaf.Token.Type != GDefTokenizerBuilder.Identifier)
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
    private static IEnumerable<ISymbol> TranslateMacro(CstInternalNode node)
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

            case MacroType.Nullable:
                return TranslateNullableMacro(macroNode);

            case MacroType.ZeroOrMore:
                return TranslateZeroOrMoreMacro(macroNode);

            case MacroType.OneOrMore:
                return TranslateOneOrMoreMacro(macroNode);

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
    private static IEnumerable<ISymbol> TranslateGroupingMacro(CstInternalNode node)
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
    /// Translates a CST internal node representing an nullable macro to a collection of ISymbol objects.
    /// </summary>
    /// <param name="node">The CST internal node.</param>
    /// <returns>A collection of ISymbol objects.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the node is not a valid nullable macro node.</exception>
    private static IEnumerable<ISymbol> TranslateNullableMacro(CstInternalNode node)
    {
        if (node.Name != "nullable")
        {
            throw new InvalidOperationException();
        }

        if (node.Children.Length != 2)
        {
            throw new InvalidOperationException();
        }

        if (node.Children[1] is not CstLeafNode operatorNode)
        {
            throw new InvalidOperationException();
        }

        if (operatorNode.Name != "?")
        {
            throw new InvalidOperationException();
        }

        var child = node.Children[0];

        yield return new NullableMacro(TranslateSymbol(child).ToArray());
    }

    /// <summary>
    /// Translates a CST internal node representing a repetition macro to a collection of ISymbol objects.
    /// </summary>
    /// <param name="node">The CST internal node.</param>
    /// <returns>A collection of ISymbol objects.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the node is not a valid repetition macro node.</exception>
    private static IEnumerable<ISymbol> TranslateZeroOrMoreMacro(CstInternalNode node)
    {
        if (node.Name != "zero_or_more")
        {
            throw new InvalidOperationException();
        }

        if (node.Children.Length != 2)
        {
            throw new InvalidOperationException();
        }

        if (node.Children[1] is not CstLeafNode operatorNode)
        {
            throw new InvalidOperationException();
        }

        if (operatorNode.Name != "*")
        {
            throw new InvalidOperationException();
        }

        var child = node.Children[0];

        yield return new ZeroOrMoreMacro(TranslateSymbol(child).ToArray());
    }

    /// <summary>
    /// Translates a CST internal node representing a repetition macro to a collection of ISymbol objects.
    /// </summary>
    /// <param name="node">The CST internal node.</param>
    /// <returns>A collection of ISymbol objects.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the node is not a valid repetition macro node.</exception>
    private static IEnumerable<ISymbol> TranslateOneOrMoreMacro(CstInternalNode node)
    {
        if (node.Name != "one_or_more")
        {
            throw new InvalidOperationException();
        }

        if (node.Children.Length != 2)
        {
            throw new InvalidOperationException();
        }

        if (node.Children[1] is not CstLeafNode operatorNode)
        {
            throw new InvalidOperationException();
        }

        if (operatorNode.Name != "+")
        {
            throw new InvalidOperationException();
        }

        var child = node.Children[0];

        yield return new OneOrMoreMacro(TranslateSymbol(child).ToArray());
    }

    /// <summary>
    /// Translates a CST internal node representing an alternative macro to a collection of ISymbol objects.
    /// </summary>
    /// <param name="node">The CST internal node.</param>
    /// <returns>A collection of ISymbol objects.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the node is not a valid alternative macro node.</exception>
    private static IEnumerable<ISymbol> TranslateAlternativeMacro(CstInternalNode node)
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
    private static MacroType GetMacroType(CstInternalNode node)
    {
        switch (node.Name)
        {
            case "grouping":
                return MacroType.Grouping;

            case "nullable":
                return MacroType.Nullable;

            case "zero_or_more":
                return MacroType.ZeroOrMore;

            case "one_or_more":
                return MacroType.OneOrMore;

            case "alternative":
                return MacroType.Alternative;

            default:
                throw new InvalidOperationException();
        }
    }

}
