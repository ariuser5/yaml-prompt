namespace YamlPrompt.Shared.Composition;

/// <summary>
/// Represents a node in a tree structure.
/// </summary>
/// <typeparam name="T">The type of the value stored in the node.</typeparam>
public class TreeNode<T>
{
    private readonly List<TreeNode<T>> _children = new();

    /// <summary>
    /// Gets or sets the value of this node.
    /// </summary>
    public T Value { get; set; }

    /// <summary>
    /// Gets the collection of child nodes.
    /// </summary>
    public IReadOnlyList<TreeNode<T>> Children => _children;

    /// <summary>
    /// Gets the parent node of this node, or null if this is the root.
    /// </summary>
    public TreeNode<T>? Parent { get; private set; }

    /// <summary>
    /// Returns true if this node is the root of the tree.
    /// </summary>
    public bool IsRoot => Parent == null;

    /// <summary>
    /// Returns true if this node has no children.
    /// </summary>
    public bool IsLeaf => Children.Count == 0;

    /// <summary>
    /// Gets the depth of this node in the tree (root is 0).
    /// </summary>
    public int Depth => Parent == null ? 0 : Parent.Depth + 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="TreeNode{T}"/> class with the specified value.
    /// </summary>
    /// <param name="value">The value to be stored in the node.</param>
    public TreeNode(T value)
    {
        Value = value;
    }

    /// <summary>
    /// Adds a child node to this node.
    /// </summary>
    /// <param name="child">The child node to be added.</param>
    public void AddChild(TreeNode<T> child)
    {
        child.Parent = this;
        _children.Add(child);
    }

    /// <summary>
    /// Inserts a child node at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which the child should be inserted.</param>
    /// <param name="child">The child node to be inserted.</param>
    public void InsertChildAt(int index, TreeNode<T> child)
    {
        child.Parent = this;
        _children.Insert(index, child);
    }

    /// <summary>
    /// Removes the specified child node from this node.
    /// </summary>
    /// <param name="child">The child node to be removed.</param>
    /// <returns>True if the child was removed; otherwise, false.</returns>
    public bool RemoveChild(TreeNode<T> child)
    {
        if (_children.Remove(child))
        {
            child.Parent = null;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes all child nodes from this node.
    /// </summary>
    public void RemoveAllChildren()
    {
        foreach (var child in _children)
            child.Parent = null;
        _children.Clear();
    }

    /// <summary>
    /// Detaches this node from its parent, if attached.
    /// </summary>
    public void Detach()
    {
        Parent?.RemoveChild(this);
    }

    /// <summary>
    /// Attaches this node to a new parent node, detaching from the current parent if necessary.
    /// </summary>
    /// <param name="newParent">The new parent node to attach to.</param>
    public void AttachTo(TreeNode<T> newParent)
    {
        Detach();
        newParent.AddChild(this);
    }

    /// <summary>
    /// Traverses the tree starting from this node, yielding all nodes in depth-first order.
    /// </summary>
    public IEnumerable<TreeNode<T>> Traverse()
    {
        yield return this;
        foreach (var child in Children)
            foreach (var descendant in child.Traverse())
                yield return descendant;
    }

    /// <summary>
    /// Finds the first node in the tree that matches the given predicate.
    /// </summary>
    /// <param name="predicate">The predicate to match nodes against.</param>
    /// <returns>The first matching node, or null if no match is found.</returns>
    public TreeNode<T>? Find(Predicate<TreeNode<T>> predicate)
    {
        if (predicate(this)) return this;
        foreach (var child in Children)
        {
            var found = child.Find(predicate);
            if (found != null) return found;
        }
        return null;
    }

    /// <summary>
    /// Gets the root node of the tree containing this node.
    /// </summary>
    /// <returns>The root node.</returns>
    public TreeNode<T> GetRoot()
    {
        var node = this;
        while (node.Parent != null)
            node = node.Parent;
        return node;
    }

    /// <summary>
    /// Gets the index of the specified child node.
    /// </summary>
    /// <param name="child">The child node whose index is to be determined.</param>
    /// <returns>The zero-based index of the child node, or -1 if the child is not a direct child of this node.</returns>
    public int IndexOf(TreeNode<T> child) => _children.IndexOf(child);

    /// <summary>
    /// Gets all ancestor nodes of this node, starting from the parent up to the root.
    /// </summary>
    public IEnumerable<TreeNode<T>> GetAncestors()
    {
        return PathToRoot().Skip(1);
    }

    /// <summary>
    /// Gets all descendant nodes of this node (excluding this node).
    /// </summary>
    public IEnumerable<TreeNode<T>> GetDescendants()
    {
        foreach (var child in Children)
        {
            yield return child;
            foreach (var descendant in child.GetDescendants())
                yield return descendant;
        }
    }

    /// <summary>
    /// Gets the path from this node up to the root, starting with this node.
    /// </summary>
    public IEnumerable<TreeNode<T>> PathToRoot()
    {
        var node = this;
        while (node != null)
        {
            yield return node;
            node = node.Parent;
        }
    }

    /// <summary>
    /// Finds all nodes in the tree that match the given predicate.
    /// </summary>
    /// <param name="predicate">The predicate to match nodes against.</param>
    /// <returns>An enumerable collection of matching nodes.</returns>
    public IEnumerable<TreeNode<T>> FindAll(Predicate<TreeNode<T>> predicate)
    {
        if (predicate(this))
            yield return this;
        foreach (var child in Children)
            foreach (var match in child.FindAll(predicate))
                yield return match;
    }

    /// <summary>
    /// Applies the specified action to this node and all descendants.
    /// </summary>
    /// <param name="action">The action to be applied to each node.</param>
    public void ForEach(Action<TreeNode<T>> action)
    {
        action(this);
        foreach (var child in Children)
            child.ForEach(action);
    }

    /// <summary>
    /// Creates a deep clone of this node and all descendants.
    /// </summary>
    /// <param name="deep">If true, clones all descendants recursively; otherwise, only this node is cloned.</param>
    /// <returns>A clone of the node.</returns>
    public TreeNode<T> Clone(bool deep = true)
    {
        var clone = new TreeNode<T>(Value);
        if (deep)
        {
            foreach (var child in Children)
            {
                var childClone = child.Clone(true);
                clone.AddChild(childClone);
            }
        }
        return clone;
    }
}
