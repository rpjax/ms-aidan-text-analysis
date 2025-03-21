﻿using System.Collections;
using System.Runtime.CompilerServices;
using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Language.Extensions;
using Aidan.TextAnalysis.Parsing.Components;
using Aidan.TextAnalysis.Tokenization.Components;

namespace Aidan.TextAnalysis.Parsing.Tree;

internal class TokenCollection : IEnumerable<IToken>
{
    internal IToken[] Tokens { get; }

    public TokenCollection(params IToken[] tokens)
    {
        Tokens = tokens;
    }

    public int Length => Tokens.Length;

    public static TokenCollection FromNodes(params TokenCollection[] nodes)
    {
        return new TokenCollection(nodes.SelectMany(x => x.Tokens).ToArray());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<IToken> GetEnumerator()
    {
        return ((IEnumerable<IToken>)Tokens).GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<IToken>)Tokens).GetEnumerator();
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

    /// <summary>
    /// Initializes a new instance of the <see cref="CstBuilder"/> class.
    /// </summary>
    /// <param name="includeEpsilons">Indicates whether to include epsilon nodes in the final CST.</param>
    public CstBuilder(bool includeEpsilons = false)
    {
        TokenAccumulator = new List<TokenCollection>(50);
        NodeAccumulator = new List<CstNode>(50);
        IncludeEpsilons = includeEpsilons;
    }

    /// <summary>
    /// Gets the number of nodes in the accumulator.
    /// </summary>
    public int AccumulatorCount => NodeAccumulator.Count;

    /// <summary>
    /// Creates a leaf node in the accumulator.
    /// </summary>
    /// <param name="token">The terminal collection to add.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CreateLeaf(IToken token)
    {
        var tokenCollection = new TokenCollection(token);
        var leafMetadata = GetLeafMetadata(token);
        var leaf = new CstLeafNode(
            name: token.Type,
            metadata: leafMetadata,
            token: token);

        TokenAccumulator.Add(tokenCollection);
        NodeAccumulator.Add(leaf);
    }

    /// <summary>
    /// Creates an internal node in the accumulator.
    /// </summary>
    /// <param name="production"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CreateInternal(IProductionRule production)
    {
        var length = production.Body.Length;

        var children = PopNodes(length);
        var tokens = ReduceTokens(length);
        var metadata = GetInternalMetadata(tokens);

        var node = new CstInternalNode(
            name: production.Head.Name,
            children: children,
            metadata: metadata
        );

        NodeAccumulator.Add(node);
    }

    /// <summary>
    /// Creates an epsilon internal node in the accumulator.
    /// </summary>
    /// <param name="production">The non-terminal associated with the epsilon collection.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CreateEpsilonInternal(IProductionRule production)
    {
        if (!production.IsEpsilonProduction())
        {
            throw new InvalidOperationException("Production rule is not an epsilon production.");
        }
        if (!IncludeEpsilons)
        {
            //return;
        }

        TokenAccumulator.Add(new TokenCollection(Array.Empty<IToken>()));

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
            metadata: GetEpsilonInternalMetadata()
        ));
    }

    /// <summary>
    /// Creates a root node in the accumulator.
    /// </summary>
    /// <param name="production"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CreateRoot(IProductionRule production)
    {
        var length = production.Body.Length;

        var children = PopNodes(length);
        var tokens = ReduceTokens(length);
        var metadata = GetInternalMetadata(tokens);

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
            //return new TokenCollection(Array.Empty<IToken>());
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
    private CstNodeMetadata GetLeafMetadata(IToken token)
    {
        var position = new SyntaxElementPosition(
            start: new LexicalCoordinate(
                index: token.Metadata.Position.Start,
                line: token.Metadata.Position.Line,
                column: token.Metadata.Position.Column - token.Value.Length + 1
            ),
            end: new LexicalCoordinate(
                index: token.Metadata.Position.End,
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
                index: firstToken.Metadata.Position.Start,
                line: firstToken.Metadata.Position.Line,
                column: firstToken.Metadata.Position.Column - firstToken.Value.Length + 1
            ),
            end: new LexicalCoordinate(
                index: lastToken.Metadata.Position.End,
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
                index: lastToken?.Metadata.Position.End ?? 0,
                line: lastToken?.Metadata.Position.Line ?? 0,
                column: lastToken?.Metadata.Position.Column - lastToken?.Value.Length + 1 ?? 0
            ),
            end: new LexicalCoordinate(
                index: lastToken?.Metadata.Position.End ?? 0,
                line: lastToken?.Metadata.Position.Line ?? 0,
                column: lastToken?.Metadata.Position.Column + 1 ?? 0
            )
        );

        return new CstNodeMetadata(
            position: position
        );
    }

}
