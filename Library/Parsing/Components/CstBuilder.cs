using Microsoft.CodeAnalysis;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Tokenization;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.Parsing.Components;

internal class TokenCollection : IEnumerable<Token>
{
    internal Token[] Tokens { get; }

    public TokenCollection(params Token[] tokens)
    {
        Tokens = tokens;
    }

    public int Length => Tokens.Length;

    public static TokenCollection FromNodes(params TokenCollection[] nodes)
    {
        return new TokenCollection(nodes.SelectMany(x => x.Tokens).ToArray());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<Token> GetEnumerator()
    {
        return ((IEnumerable<Token>)Tokens).GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<Token>)Tokens).GetEnumerator();
    }

}

/// <summary>
/// Represents a builder for constructing a Concrete Syntax Tree (CST).
/// </summary>
public class CstBuilder
{
    private List<TokenCollection> TokenAccumulator { get; }
    private List<CstNode> NodeAccumulator { get; }
    private bool IncludeEpsilons { get; set; }

    private Token[] SingleTokenBuffer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CstBuilder"/> class.
    /// </summary>
    /// <param name="includeEpsilons">Indicates whether to include epsilon nodes in the final CST.</param>
    public CstBuilder(bool includeEpsilons = false)
    {
        TokenAccumulator = new List<TokenCollection>(50);
        NodeAccumulator = new List<CstNode>(50);
        IncludeEpsilons = includeEpsilons;
        SingleTokenBuffer = new Token[1];
    }

    /// <summary>
    /// Gets the number of nodes in the accumulator.
    /// </summary>
    public int AccumulatorCount => NodeAccumulator.Count;

    /// <summary>
    /// Creates a leaf node in the accumulator.
    /// </summary>
    /// <param name="terminal">The terminal collection to add.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CreateLeaf(Token token)
    {
        TokenAccumulator.Add(new TokenCollection(token));
        NodeAccumulator.Add(new CstLeafNode(token: token, metadata: GetLeafMetadata(token)));
    }

    /// <summary>
    /// Creates an internal node in the accumulator.
    /// </summary>
    /// <param name="production"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CreateInternal(ref ProductionRule production)
    {
        var length = production.Length;

        var children = PopNodes(length);
        var tokens = ReduceTokens(length);
        var metadata = GetInternalMetadata(tokens);

        if (!IncludeEpsilons)
        {
            children = children
                 .Where(x => x is CstInternalNode node ? !node.IsEpsilon : true)
                 .ToArray();
        }

        var node = new CstInternalNode(
            name: production.Head.Name,
            children: children,
            metadata: metadata,
            isEpsilon: production.IsEpsilonProduction
        );

        NodeAccumulator.Add(node);
    }

    /// <summary>
    /// Creates an epsilon internal node in the accumulator.
    /// </summary>
    /// <param name="nonTerminal">The non-terminal associated with the epsilon collection.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CreateEpsilonInternal(ref ProductionRule production)
    {
        if (!production.IsEpsilonProduction)
        {
            throw new InvalidOperationException("Production rule is not an epsilon production.");
        }

        TokenAccumulator.Add(new TokenCollection(Array.Empty<Token>()));

        // Adds the epsilon collection to the collection accumulator.
        // The epsilon collection has no children and is marked as an epsilon collection.
        // This ensures that reduction length is consistent when reducing non-terminals that contain epsilon nodes.
        // Ex:
        //  function_body -> '{' statement statement_tail '}'
        //  statement_tail -> statement statement_tail | epsilon
        // 
        // In this case when reducing function_body, the epsilon collection is added to the collection accumulator,
        // and the length of the reduction is consistent, 2 in this case to form the function_body collection.
        // It also helps to debug the parser by showing where an epsilon reduction occurred.
        //
        // The metadata for the epsilon collection is derived from the last token in the token accumulator.
        NodeAccumulator.Add(new CstInternalNode(
            name: production.Head.Name,
            children: Array.Empty<CstNode>(),
            metadata: GetEpsilonInternalMetadata(),
            isEpsilon: true
        ));
    }

    /// <summary>
    /// Creates a root node in the accumulator.
    /// </summary>
    /// <param name="production"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CreateRoot(ref ProductionRule production)
    {
        var length = production.Length;

        var children = PopNodes(length);
        var tokens = ReduceTokens(length);
        var metadata = GetInternalMetadata(tokens);

        if (!IncludeEpsilons)
        {
            children = children
                 .Where(x => x is CstInternalNode node ? !node.IsEpsilon : true)
                 .ToArray();
        }

        var node = new CstRootNode(
            name: production.Head.Name,
            children: children,
            metadata: metadata
        );

        NodeAccumulator.Add(node);
    }

    /// <summary>
    /// Builds the Concrete Syntax Tree (CST) from the accumulator.
    /// </summary>
    /// <returns>The root collection of the CST.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the CST is empty or not complete.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CstRootNode Build()
    {
        if (NodeAccumulator.Count == 0)
        {
            throw new InvalidOperationException("CST is empty.");
        }
        if (NodeAccumulator.Count != 1)
        {
            throw new InvalidOperationException("CST is not complete.");
        }

        if (NodeAccumulator.Single() is not CstRootNode root)
        {
            throw new InvalidOperationException("CST is not complete.");
        }

        return root;
    }

    /*
     * private helper methods.
     */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private TokenCollection ReduceTokens(int length)
    {
        if (length == 0)
        {
            throw new InvalidOperationException("Token array is empty.");
        }

        var offset = TokenAccumulator.Count - length;
        var tokenNodes = TokenAccumulator
            .Skip(offset)
            .ToArray();

        var tokens = TokenCollection.FromNodes(tokenNodes);

        TokenAccumulator.RemoveRange(offset, length);
        TokenAccumulator.Add(tokens);
        return tokens;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private CstNode[] PopNodes(int length)
    {
        if (length == 0)
        {
            return Array.Empty<CstNode>();
        }

        var offset = NodeAccumulator.Count - length;
        var nodes = NodeAccumulator
            .Skip(offset)
            .ToArray();

        NodeAccumulator.RemoveRange(offset, length);
        return nodes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private CstNodeMetadata GetLeafMetadata(Token token)
    {
        var position = new SyntaxElementPosition(
            start: new LexicalCoordinate(
                index: token.Metadata.Position.StartIndex,
                line: token.Metadata.Position.Line,
                column: token.Metadata.Position.Column - token.Value.Length + 1
            ),
            end: new LexicalCoordinate(
                index: token.Metadata.Position.EndIndex,
                line: token.Metadata.Position.Line,
                column: token.Metadata.Position.Column + 1
            )
        );

        return new CstNodeMetadata(
            position: position
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private CstNodeMetadata GetInternalMetadata(TokenCollection collection)
    {
        if (collection.Length == 0)
        {
            throw new InvalidOperationException("Token array is empty.");
        }

        var firstToken = collection.First();
        var lastToken = collection.Last();

        var position = new SyntaxElementPosition(
            start: new LexicalCoordinate(
                index: firstToken.Metadata.Position.StartIndex,
                line: firstToken.Metadata.Position.Line,
                column: firstToken.Metadata.Position.Column - firstToken.Value.Length + 1
            ),
            end: new LexicalCoordinate(
                index: lastToken.Metadata.Position.EndIndex,
                line: lastToken.Metadata.Position.Line,
                column: lastToken.Metadata.Position.Column + 1
            )
        );

        return new CstNodeMetadata(
            position: position
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private CstNodeMetadata GetEpsilonInternalMetadata()
    {
        var lastToken = TokenAccumulator.LastOrDefault()?.FirstOrDefault();

        var position = new SyntaxElementPosition(
            start: new LexicalCoordinate(
                index: lastToken?.Metadata.Position.EndIndex ?? 0,
                line: lastToken?.Metadata.Position.Line ?? 0,
                column: lastToken?.Metadata.Position.Column - lastToken?.Value.Length + 1 ?? 0
            ),
            end: new LexicalCoordinate(
                index: lastToken?.Metadata.Position.EndIndex ?? 0,
                line: lastToken?.Metadata.Position.Line ?? 0,
                column: lastToken?.Metadata.Position.Column + 1 ?? 0
            )
        );

        return new CstNodeMetadata(
            position: position
        );
    }

}

