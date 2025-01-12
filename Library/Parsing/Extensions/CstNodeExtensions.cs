using Aidan.TextAnalysis.Parsing.Tree;

namespace Aidan.TextAnalysis.Parsing.Extensions;

/// <summary>
/// Provides extension methods for <see cref="CstNode"/> to enhance its functionality.
/// </summary>
public static class CstNodeExtensions
{
    /// <summary>
    /// Converts a <see cref="CstNode"/> into an HTML tree view representation.
    /// </summary>
    /// <param name="node">The CST node to convert.</param>
    /// <returns>An HTML string representing the tree view of the node.</returns>
    public static string ToHtmlTreeView(this CstNode node)
    {
        return new CstNodeHtmlBuilder(node)
            .Build();
    }

    /// <summary>
    /// Casts the specified <see cref="CstNode"/> to a <see cref="CstRootNode"/>.
    /// </summary>
    /// <param name="node">The node to cast.</param>
    /// <returns>The node as a <see cref="CstRootNode"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the node is not a root node.</exception>
    public static CstRootNode AsRoot(this CstNode node)
    {
        if (node is not CstRootNode root)
        {
            throw new InvalidOperationException("Node is not a root node");
        }

        return root;
    }

    /// <summary>
    /// Casts the specified <see cref="CstNode"/> to a <see cref="CstInternalNode"/>.
    /// </summary>
    /// <param name="node">The node to cast.</param>
    /// <returns>The node as a <see cref="CstInternalNode"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the node is not an internal node.</exception>
    public static CstInternalNode AsInternal(this CstNode node)
    {
        if (node is not CstInternalNode internalNode)
        {
            throw new InvalidOperationException("Node is not an internal node");
        }

        return internalNode;
    }

    /// <summary>
    /// Casts the specified <see cref="CstNode"/> to a <see cref="CstLeafNode"/>.
    /// </summary>
    /// <param name="node">The node to cast.</param>
    /// <returns>The node as a <see cref="CstLeafNode"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the node is not a leaf node.</exception>
    public static CstLeafNode AsLeaf(this CstNode node)
    {
        if (node is not CstLeafNode leaf)
        {
            throw new InvalidOperationException("Node is not a leaf node");
        }

        return leaf;
    }

    /// <summary>
    /// Retrieves all properties of the given <see cref="CstNode"/>.
    /// </summary>
    /// <param name="node">The node whose properties are retrieved.</param>
    /// <returns>An enumerable collection of key-value pairs representing the properties.</returns>
    public static IEnumerable<KeyValuePair<string, object>> GetProperties(this CstNode node)
    {
        return node.Properties;
    }

    /// <summary>
    /// Checks whether the given <see cref="CstNode"/> has a property with the specified key.
    /// </summary>
    /// <param name="node">The node to check.</param>
    /// <param name="key">The key to look for.</param>
    /// <returns>True if the property exists; otherwise, false.</returns>
    public static bool HasProperty(this CstNode node, string key)
    {
        return node.Properties.ContainsKey(key);
    }

    /// <summary>
    /// Retrieves the value of a property by key.
    /// </summary>
    /// <param name="node">The node whose property is being retrieved.</param>
    /// <param name="key">The key of the property.</param>
    /// <returns>The value of the property.</returns>
    public static object GetProperty(this CstNode node, string key)
    {
        return node.Properties[key];
    }

    /// <summary>
    /// Attempts to retrieve a property by key, returning null if not found.
    /// </summary>
    /// <param name="node">The node whose property is being retrieved.</param>
    /// <param name="key">The key of the property.</param>
    /// <returns>The value of the property if found; otherwise, null.</returns>
    public static object? TryGetProperty(this CstNode node, string key)
    {
        return node.Properties.TryGetValue(key, out var value) ? value : null;
    }

    /// <summary>
    /// Attempts to retrieve a property by key and cast it to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to cast the property value to.</typeparam>
    /// <param name="node">The node whose property is being retrieved.</param>
    /// <param name="key">The key of the property.</param>
    /// <returns>The value of the property if found and successfully cast; otherwise, the default value of <typeparamref name="T"/>.</returns>
    public static T? TryGetProperty<T>(this CstNode node, string key)
    {
        var value = node.TryGetProperty(key);

        if (value is T result)
        {
            return result;
        }

        return default;
    }

    /// <summary>
    /// Sets a property on the given <see cref="CstNode"/>.
    /// </summary>
    /// <param name="node">The node to set the property on.</param>
    /// <param name="key">The key of the property.</param>
    /// <param name="value">The value of the property.</param>
    public static void SetProperty(this CstNode node, string key, object value)
    {
        node.Properties[key] = value;
    }

    public static CstRootNode GetRoot(this CstNode node)
    {
        CstNode? current = node;

        while (true)
        {
            if (current is null)
            {
                throw new InvalidOperationException();
            }

            if (current is CstRootNode root)
            {
                if (root.Parent is not null)
                {
                    throw new InvalidOperationException();
                }

                return root;
            }

            current = current.Parent;
        }
    }

    public static CstLeafNode[] GetAllLeaves(this CstNode self)
    {
        var leaves = new List<CstLeafNode>();
        var stack = new Stack<CstNode>();

        stack.Push(self);

        while (stack.TryPop(out var node))
        {
            var children = node.GetChildren();
            var leafChildren = children
                .Where(x => x is CstLeafNode)
                .Cast<CstLeafNode>()
                .ToArray();

            var rest = children
                .Except(leafChildren)
                .ToArray();

            leaves.AddRange(leafChildren);

            foreach (var item in rest)
            {
                stack.Push(item);
            }
        }

        return leaves.ToArray();
    }

    public static CstNode[] GetAllNodes(this CstNode self)
    {
        var nodes = new List<CstNode>();
        var stack = new Stack<CstNode>();

        stack.Push(self);

        while (stack.TryPop(out var node))
        {
            var children = node.GetChildren();

            foreach (var item in children)
            {
                if (nodes.Contains(item))
                {
                    continue;
                }
                nodes.Add(item);
                stack.Push(item);
            }
        }

        return nodes
            .ToArray();
    }

}
