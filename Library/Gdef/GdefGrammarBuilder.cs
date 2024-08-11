using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Parsing.Components;
using Aidan.TextAnalysis.Tokenization;
using Aidan.TextAnalysis.Tokenization.Extensions;

namespace Aidan.TextAnalysis.Gdef;

/// <summary>
/// Represents a translator for converting a Concrete Syntax Tree (CST) to a grammar.
/// </summary>
public class GdefTranslator
{
    enum SymbolType
    {
        Terminal,
        NonTerminal,
        Macro
    }

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

        if (leaf.Token.Type != TokenType.Identifier)
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

    public static Sentence TranslateSentence(CstNode[] nodes)
    {
        var symbols = nodes
            .Where(x => x is CstInternalNode node ? !node.IsEpsilon : true)
            .SelectMany(x => TranslateSymbol(x))
            .ToArray();

        return new Sentence(symbols);
    }

    public static IEnumerable<Symbol> TranslateSymbol(CstNode node)
    {
        if(node is not CstInternalNode internalNode)
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

    /*
     * translate terminals
     */
    public static IEnumerable<Symbol> TranslateTerminal(CstInternalNode node)
    {
        if(node.Name != "terminal")
        {
            throw new InvalidOperationException();
        }
        if(node.Children.Length != 1)
        {
            throw new InvalidOperationException();
        }

        var child = node.Children[0];

        if(child is CstLeafNode leaf)
        {
            return TranslateTerminalString(leaf);
        }
        if(child is not CstInternalNode internalNode)
        {
            throw new InvalidOperationException();
        }

        if(internalNode.Name == "lexeme")
        {
            return TranslateTerminalLexeme(internalNode);
        }

        if(internalNode.Name == "epsilon")
        {
            return TranslateTerminalEpsilon(internalNode);
        }

        throw new InvalidOperationException();
    }

    public static IEnumerable<Symbol> TranslateTerminalString(CstLeafNode leaf)
    {
        var tokens = new List<Token>();

        if (leaf.Token.Type == TokenType.String)
        {
            var leafTokens = Tokenizer.Instance.Tokenize(
                source: leaf.Token.GetNormalizedStringValue(),
                includeEoi: false
            );

            tokens.AddRange(leafTokens);
        }
        else
        {
            tokens.Add(leaf.Token);
        }

        return tokens
            .Select(x => new Terminal(x.Type, x.Value.ToString()));
    }

    public static IEnumerable<Symbol> TranslateTerminalLexeme(CstInternalNode node)
    {
        yield return new Terminal(GetLexemeType(node), null);
        yield break;
    }

    public static IEnumerable<Symbol> TranslateTerminalEpsilon(CstInternalNode node)
    {
        if (node.Name != "epsilon")
        {
            throw new InvalidOperationException();
        }

        yield return new Epsilon();
        yield break;
    }

    /*
     * translate non terminal
     */
    public static IEnumerable<Symbol> TranslateNonTerminal(CstInternalNode node)
    {
        if(node.Name != "non_terminal")
        {
            throw new InvalidOperationException();
        }

        if(node.Children.Length != 1)
        {
            throw new InvalidOperationException();
        }

        if(node.Children[0] is not CstLeafNode leaf)
        {
            throw new InvalidOperationException();
        }

        if(leaf.Token.Type != TokenType.Identifier)
        {
            throw new InvalidOperationException();
        }

        yield return new NonTerminal(leaf.Token.Value.ToString());
    }

    /*
     * translate macros
     */
    public static IEnumerable<Symbol> TranslateMacro(CstInternalNode node)
    {
        if(node.Name != "macro")
        {
            throw new InvalidOperationException();
        }

        if(node.Children.Length != 1)
        {
            throw new InvalidOperationException();
        }

        if(node.Children[0] is not CstInternalNode macroNode)
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

    public static IEnumerable<Symbol> TranslateGroupingMacro(CstInternalNode node)
    {
        if(node.Name != "grouping")
        {
            throw new InvalidOperationException();
        }

        if(node.Children.Length < 3)
        {
            throw new InvalidOperationException();
        }

        var children = node.Children
            .Skip(1)
            .Take(node.Children.Length - 2)
            .ToArray();

        yield return new GroupingMacro(TranslateSentence(children));
    }

    public static IEnumerable<Symbol> TranslateOptionMacro(CstInternalNode node)
    {
        if(node.Name != "option")
        {
            throw new InvalidOperationException();
        }

        if(node.Children.Length < 3)
        {
            throw new InvalidOperationException();
        }

        var children = node.Children
            .Skip(1)
            .Take(node.Children.Length - 2)
            .ToArray();

        yield return new OptionMacro(TranslateSentence(children));
    }

    public static IEnumerable<Symbol> TranslateRepetitionMacro(CstInternalNode node)
    {
        if(node.Name != "repetition")
        {
            throw new InvalidOperationException();
        }

        if(node.Children.Length < 3)
        {
            throw new InvalidOperationException();
        }

        var children = node.Children
            .Skip(1)
            .Take(node.Children.Length - 2)
            .ToArray();

        yield return new RepetitionMacro(TranslateSentence(children));
    }

    public static IEnumerable<Symbol> TranslateAlternativeMacro(CstInternalNode node)
    {
        if(node.Name != "alternative")
        {
            throw new InvalidOperationException();
        }

        yield return new AlternativeMacro();
    }

    /*
     * private methods.
     */
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

    private static TokenType GetLexemeType(CstInternalNode node)
    {
        if (node.Name != "lexeme")
        {
            throw new InvalidOperationException();
        }
        if (node.Children.Length != 2)
        {
            throw new InvalidOperationException();
        }

        if (node.Children[1] is not CstLeafNode lexemeNode)
        {
            throw new InvalidOperationException();
        }

        switch (lexemeNode.Token.Value.ToString())
        {
            case "id":
                return TokenType.Identifier;

            case "string":
                return TokenType.String;

            case "int":
                return TokenType.Integer;

            case "float":
                return TokenType.Float;

            case "hex":
                return TokenType.Hexadecimal;

            default:
                throw new InvalidOperationException();
        }
    }

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
