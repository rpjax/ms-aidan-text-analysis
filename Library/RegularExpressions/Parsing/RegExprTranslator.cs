using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Parsing.Components;
using Aidan.TextAnalysis.Parsing.Extensions;
using Aidan.TextAnalysis.Parsing.Tree;
using Aidan.TextAnalysis.RegularExpressions.Tree;
using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.RegularExpressions.Parsing;

public class RegExprTranslator
{
    private Charset Charset { get; }
    private Dictionary<string, RegExpr> Fragments { get; }
    private Dictionary<char, char> EscapedCharsMap { get; }

    public RegExprTranslator(
        Charset charset, 
        Dictionary<string, RegExpr>? fragments)
    {
        Charset = charset;
        Fragments = fragments ?? new();
        EscapedCharsMap = new Dictionary<char, char>()
        {
            { 'n', '\n' },  // Nova linha
            { 'r', '\r' },  // Retorno de carro
            { 't', '\t' },  // Tabulação
            { '\\', '\\' }, // Barra invertida
            { '"', '"' },   // Aspas duplas
            { '\'', '\'' }, // Aspas simples
            { 'b', '\b' },  // Backspace
            { 'f', '\f' },  // Alimentação de formulário
            { 'v', '\v' },  // Tabulação vertical
            { '0', '\0' }   // Nulo
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RegExpr Translate(CstRootNode root)
    {
        /* debug */
        var html = root.ToHtmlTreeView();

        if (root.Name != "regex")
        {
            throw new InvalidOperationException();
        }

        if (root.Children.Length != 1)
        {
            throw new InvalidOperationException();
        }

        if (root.Children[0] is not CstInternalNode union)
        {
            throw new InvalidOperationException();
        }

        return TranslateUnion(union);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr TranslateRegex(CstInternalNode node)
    {
        if (node.Name != "regex")
        {
            throw new InvalidOperationException();
        }

        if (node.Children.Length != 1)
        {
            throw new InvalidOperationException();
        }

        if (node.Children[0] is not CstInternalNode union)
        {
            throw new InvalidOperationException();
        }

        return TranslateUnion(union);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr TranslateUnion(CstInternalNode node)
    {
        if (node.Name != "union")
        {
            throw new InvalidOperationException();
        }

        if (node.Children.Length == 0)
        {
            throw new InvalidOperationException();
        }

        var concatenations = node.Children
            .Where(child => child.Name == "concatenation")
            .Select(child => child as CstInternalNode ?? throw new InvalidOperationException())
            .Select(child => TranslateConcatenation(child))
            .ToArray()
            ;

        if (concatenations.Length == 0)
        {
            throw new InvalidOperationException();
        }

        return RegExpr.Union(concatenations);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr TranslateConcatenation(CstInternalNode node)
    {
        if (node.Name != "concatenation")
        {
            throw new InvalidOperationException();
        }

        if (node.Children.Length == 0)
        {
            throw new InvalidOperationException();
        }

        var quantifiers = node.Children
            .Select(child => child as CstInternalNode ?? throw new InvalidOperationException())
            .Select(child => TranslateQuantifier(child))
            .ToArray()
            ;

        if (quantifiers.Length == 0)
        {
            throw new InvalidOperationException();
        }

        return RegExpr.Concatenation(quantifiers);      
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr TranslateQuantifier(CstInternalNode node)
    {
        if (node.Name != "quantifier")
        {
            throw new InvalidOperationException();
        }

        if (node.Children.Length == 0)
        {
            throw new InvalidOperationException();
        }

        if (node.Children.Length > 2)
        {
            throw new InvalidOperationException();
        }

        CstInternalNode primaryNode = node.Children[0].AsInternal();
        RegExpr primary = TranslatePrimary(primaryNode);

        string? quantifier = node.Children.Length == 2
            ? node.Children[1].AsLeaf().GetValue().ToString()
            : null;

        switch (quantifier)
        {
            case null:
                return primary;

            case "*":
                return new StarNode(primary);

            case "+":
                return new ConcatenationNode(
                    primary,
                    new StarNode(primary)
                );

            case "?":
                return new UnionNode(
                    primary,
                    new EpsilonNode()
                );

            default:
                throw new InvalidOperationException();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr TranslatePrimary(CstInternalNode node)
    {
        if (node.Name != "primary")
        {
            throw new InvalidOperationException();
        }

        if (node.Children.Length != 1)
        {
            throw new InvalidOperationException();
        }

        var child = node.Children[0];

        switch (child.Name)
        {
            case "group":
                return TranslateGroup(child.AsInternal());

            case "class":
                return TranslateClass(child.AsInternal());

            case "any":
                return TranslateAny(child.AsInternal());

            case "fragment":
                return TranslateFragment(child.AsInternal());

            case "char":
                return TranslateChar(child.AsLeaf());

            case "escaped_char":
                return TranslateEscapedChar(child.AsLeaf());

            default:
                throw new InvalidOperationException();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr TranslateGroup(CstInternalNode node)
    {
        if (node.Name != "group")
        {
            throw new InvalidOperationException();
        }

        if (node.Children.Length != 3)
        {
            throw new InvalidOperationException();
        }

        var regex = node.Children[1].AsInternal();

        return TranslateRegex(regex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr TranslateClass(CstInternalNode node)
    {
        if (node.Name != "class")
        {
            throw new InvalidOperationException();
        }

        if (node.Children.Length < 3)
        {
            throw new InvalidOperationException();
        }

        bool isNegated = false;

        var noParenthesisChildren = node.Children
            .Skip(1)
            .Take(node.Children.Length - 2)
            .ToArray();

        if (noParenthesisChildren[0] is CstLeafNode leaf)
        {
            if (leaf.GetValue().ToString() != "^")
            {
                throw new InvalidOperationException();
            }

            isNegated = true;
            noParenthesisChildren = noParenthesisChildren
                .Skip(1)
                .ToArray();
        }

        if (noParenthesisChildren.Length == 0)
        {
            throw new InvalidOperationException();
        }

        var classChildren = noParenthesisChildren
            .Select(x => x.AsInternal())
            .Select(x => TranslateClassChild(x))
            .ToArray();

        return new ClassNode(
            charset: Charset,
            isNegated: isNegated,
            children: classChildren
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ClassChild TranslateClassChild(CstInternalNode node)
    {
        switch (node.Name)
        {
            case "class_literal":
                return TranslateClassLiteral(node);

            case "class_range":
                return TranslateClassRange(node);

            default:
                throw new InvalidOperationException();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ClassChild TranslateClassLiteral(CstInternalNode node)
    {
        if (node.Name != "class_literal")
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

        char[] chars = leaf.GetValue().ToArray();
        char character;

        if (chars.Length == 1)
        {
            character = chars[0];
        }
        else
        {
            if (leaf.Name != "escaped_char")
            {
                throw new InvalidOperationException();
            }

            if (!EscapedCharsMap.TryGetValue(chars[1], out var result))
            {
                throw new InvalidOperationException();
            }

            character = result;
        }

        return new ClassLiteral(character);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ClassChild TranslateClassRange(CstInternalNode node)
    {
        if (node.Name != "class_range")
        {
            throw new InvalidOperationException();
        }

        if (node.Children.Length != 3)
        {
            throw new InvalidOperationException();
        }

        if (node.Children[0] is not CstLeafNode start)
        {
            throw new InvalidOperationException();
        }

        if (node.Children[2] is not CstLeafNode end)
        {
            throw new InvalidOperationException();
        }

        var startValue = start.GetValue().ToArray();
        var endValue = end.GetValue().ToArray();

        if (startValue.Length != 1)
        {
            throw new InvalidOperationException();
        }
        if (endValue.Length != 1)
        {
            throw new InvalidOperationException();
        }

        var startChar = startValue[0];
        var endChar = endValue[0];

        return new ClassRange(startChar, endChar);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr TranslateAny(CstInternalNode node)
    {
        if (node.Name != "any")
        {
            throw new InvalidOperationException();
        }

        return new AnythingNode(Charset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr TranslateFragment(CstInternalNode node)
    {
        if (node.Name != "fragment")
        {
            throw new InvalidOperationException();
        }

        if (node.Children.Length != 2)
        {
            throw new InvalidOperationException();
        }

        if (node.Children[1] is not CstLeafNode id)
        {
            throw new InvalidOperationException();
        }

        if (!Fragments.TryGetValue(node.Name, out var fragment))
        {
            throw new InvalidOperationException();
        }

        return fragment;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr TranslateChar(CstLeafNode node)
    {
        if (node.Name != "char")
        {
            throw new InvalidOperationException();
        }

        var value = node.GetValue().ToArray();

        if (value.Length != 1)
        {
            throw new InvalidOperationException();
        }

        var character = value[0];

        return new LiteralNode(character);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RegExpr TranslateEscapedChar(CstLeafNode node)
    {
        if (node.Name != "escaped_char")
        {
            throw new InvalidOperationException();
        }

        var value = node.GetValue().ToArray();

        if (value.Length != 2)
        {
            throw new InvalidOperationException();
        }

        var character = value[1];

        if (EscapedCharsMap.TryGetValue(character, out var result))
        {
            character = result;
        }

        return new LiteralNode(character);
    }

}
