using Aidan.TextAnalysis.Parsing.Components;

namespace Aidan.TextAnalysis.Parsing.Extensions;

public static class CstNodeExtensions
{
    public static string ToHtmlTreeView(this CstNode node)
    {
        return new CstNodeHtmlBuilder(node)
            .Build();
    }

    public static CstRootNode AsRoot(this CstNode node)
    {
        if(node is not CstRootNode root)
        {
            throw new InvalidOperationException("Node is not a root node");
        }

        return root;
    }

    public static CstInternalNode AsInternal(this CstNode node)
    {
        if(node is not CstInternalNode internalNode)
        {
            throw new InvalidOperationException("Node is not an internal node");
        }

        return internalNode;
    }

    public static CstLeafNode AsLeaf(this CstNode node)
    {
        if(node is not CstLeafNode leaf)
        {
            throw new InvalidOperationException("Node is not a leaf node");
        }

        return leaf;
    }

}
