using YamlPrompt.Shared.Composition;

namespace YamlPrompt.Shared.Tests.Composition;

[Trait("TestCategory", "Unit")]
public class TreeNodeTests
{
    [Fact]
    public void AddChild_SetsParentAndAddsToChildren()
    {
        var parent = new TreeNode<string>("parent");
        var child = new TreeNode<string>("child");
        parent.AddChild(child);
        Assert.Contains(child, parent.Children);
        Assert.Equal(parent, child.Parent);
    }

    [Fact]
    public void RemoveChild_RemovesChildAndClearsParent()
    {
        var parent = new TreeNode<int>(1);
        var child = new TreeNode<int>(2);
        parent.AddChild(child);
        var removed = parent.RemoveChild(child);
        Assert.True(removed);
        Assert.DoesNotContain(child, parent.Children);
        Assert.Null(child.Parent);
    }

    [Fact]
    public void RemoveAllChildren_ClearsChildrenAndParents()
    {
        var parent = new TreeNode<int>(1);
        var child1 = new TreeNode<int>(2);
        var child2 = new TreeNode<int>(3);
        parent.AddChild(child1);
        parent.AddChild(child2);
        parent.RemoveAllChildren();
        Assert.Empty(parent.Children);
        Assert.Null(child1.Parent);
        Assert.Null(child2.Parent);
    }

    [Fact]
    public void InsertChildAt_InsertsAtCorrectIndex()
    {
        var parent = new TreeNode<string>("root");
        var child1 = new TreeNode<string>("a");
        var child2 = new TreeNode<string>("b");
        parent.AddChild(child1);
        parent.InsertChildAt(0, child2);
        Assert.Equal(child2, parent.Children[0]);
        Assert.Equal(child1, parent.Children[1]);
        Assert.Equal(parent, child2.Parent);
    }

    [Fact]
    public void Detach_RemovesFromParent()
    {
        var parent = new TreeNode<int>(1);
        var child = new TreeNode<int>(2);
        parent.AddChild(child);
        child.Detach();
        Assert.Null(child.Parent);
        Assert.Empty(parent.Children);
    }

    [Fact]
    public void AttachTo_MovesNodeToNewParent()
    {
        var parent1 = new TreeNode<int>(1);
        var parent2 = new TreeNode<int>(2);
        var child = new TreeNode<int>(3);
        parent1.AddChild(child);
        child.AttachTo(parent2);
        Assert.Equal(parent2, child.Parent);
        Assert.DoesNotContain(child, parent1.Children);
        Assert.Contains(child, parent2.Children);
    }

    [Fact]
    public void Traverse_YieldsAllNodes()
    {
        var root = new TreeNode<int>(1);
        var child1 = new TreeNode<int>(2);
        var child2 = new TreeNode<int>(3);
        root.AddChild(child1);
        root.AddChild(child2);
        var all = root.Traverse().ToList();
        Assert.Equal(3, all.Count);
        Assert.Contains(root, all);
        Assert.Contains(child1, all);
        Assert.Contains(child2, all);
    }

    [Fact]
    public void Find_ReturnsFirstMatch()
    {
        var root = new TreeNode<int>(1);
        var child = new TreeNode<int>(2);
        root.AddChild(child);
        var found = root.Find(n => n.Value == 2);
        Assert.Equal(child, found);
    }

    [Fact]
    public void GetRoot_ReturnsRootNode()
    {
        var root = new TreeNode<int>(1);
        var child = new TreeNode<int>(2);
        root.AddChild(child);
        Assert.Equal(root, child.GetRoot());
    }

    [Fact]
    public void IndexOf_ReturnsCorrectIndex()
    {
        var parent = new TreeNode<int>(1);
        var child1 = new TreeNode<int>(2);
        var child2 = new TreeNode<int>(3);
        parent.AddChild(child1);
        parent.AddChild(child2);
        Assert.Equal(0, parent.IndexOf(child1));
        Assert.Equal(1, parent.IndexOf(child2));
    }

    [Fact]
    public void GetAncestors_ReturnsAncestorsInOrder()
    {
        var root = new TreeNode<int>(1);
        var child = new TreeNode<int>(2);
        var grandchild = new TreeNode<int>(3);
        root.AddChild(child);
        child.AddChild(grandchild);
        var ancestors = grandchild.GetAncestors().ToList();
        Assert.Equal(new[] { child, root }, ancestors);
    }

    [Fact]
    public void GetDescendants_ReturnsAllDescendants()
    {
        var root = new TreeNode<int>(1);
        var child = new TreeNode<int>(2);
        var grandchild = new TreeNode<int>(3);
        root.AddChild(child);
        child.AddChild(grandchild);
        var descendants = root.GetDescendants().ToList();
        Assert.Contains(child, descendants);
        Assert.Contains(grandchild, descendants);
        Assert.DoesNotContain(root, descendants);
    }

    [Fact]
    public void PathToRoot_ReturnsPathFromNodeToRoot()
    {
        var root = new TreeNode<int>(1);
        var child = new TreeNode<int>(2);
        var grandchild = new TreeNode<int>(3);
        root.AddChild(child);
        child.AddChild(grandchild);
        var path = grandchild.PathToRoot().ToList();
        Assert.Equal(new[] { grandchild, child, root }, path);
    }

    [Fact]
    public void FindAll_ReturnsAllMatchingNodes()
    {
        var root = new TreeNode<int>(1);
        var child1 = new TreeNode<int>(2);
        var child2 = new TreeNode<int>(2);
        root.AddChild(child1);
        root.AddChild(child2);
        var matches = root.FindAll(n => n.Value == 2).ToList();
        Assert.Contains(child1, matches);
        Assert.Contains(child2, matches);
        Assert.DoesNotContain(root, matches);
    }

    [Fact]
    public void ForEach_AppliesActionToAllNodes()
    {
        var root = new TreeNode<int>(1);
        var child = new TreeNode<int>(2);
        root.AddChild(child);
        var values = new List<int>();
        root.ForEach(n => values.Add(n.Value));
        Assert.Contains(1, values);
        Assert.Contains(2, values);
    }

    [Fact]
    public void Clone_DeepClonesTree()
    {
        var root = new TreeNode<int>(1);
        var child = new TreeNode<int>(2);
        root.AddChild(child);
        var clone = root.Clone();	
        Assert.NotSame(root, clone);
        Assert.Equal(root.Value, clone.Value);
        Assert.Single(clone.Children);
        Assert.Equal(child.Value, clone.Children[0].Value);
        Assert.NotSame(child, clone.Children[0]);
    }
}
