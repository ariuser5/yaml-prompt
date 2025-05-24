using System;
using System.Collections.Generic;
using Xunit;
using YamlPrompt.Shared.Collections;

namespace YamlPrompt.Shared.Tests.Collections;

[Trait("TestCategory", "Unit")]
public class AddOnlyDictionaryTests
{
    [Fact]
    public void Add_AddsNewKeyValuePair()
    {
        var dict = new AddOnlyDictionary<string, int>();
        dict.Add("a", 1);
        Assert.Equal(1, dict["a"]);
        Assert.Single(dict);
    }

    [Fact]
    public void Add_ThrowsIfKeyExists()
    {
        var dict = new AddOnlyDictionary<string, int>();
        dict.Add("a", 1);
        Assert.Throws<ArgumentException>(() => dict.Add("a", 2));
    }

    [Fact]
    public void Indexer_ReturnsValue()
    {
        var dict = new AddOnlyDictionary<string, int>();
        dict.Add("a", 1);
        Assert.Equal(1, dict["a"]);
    }

    [Fact]
    public void TryAdd_AddsIfNotExists_ReturnsTrue()
    {
        var dict = new AddOnlyDictionary<string, int>();
        var result = dict.TryAdd("a", 1);
        Assert.True(result);
        Assert.Equal(1, dict["a"]);
    }

    [Fact]
    public void TryAdd_DoesNotAddIfExists_ReturnsFalse()
    {
        var dict = new AddOnlyDictionary<string, int>();
        dict.Add("a", 1);
        var result = dict.TryAdd("a", 2);
        Assert.False(result);
        Assert.Equal(1, dict["a"]);
    }

    [Fact]
    public void ContainsKey_ReturnsTrueIfExists()
    {
        var dict = new AddOnlyDictionary<string, int>();
        dict.Add("a", 1);
        Assert.True(dict.ContainsKey("a"));
        Assert.False(dict.ContainsKey("b"));
    }

    [Fact]
    public void TryGetValue_ReturnsTrueAndValueIfExists()
    {
        var dict = new AddOnlyDictionary<string, int>();
        dict.Add("a", 1);
        var found = dict.TryGetValue("a", out var value);
        Assert.True(found);
        Assert.Equal(1, value);
    }

    [Fact]
    public void TryGetValue_ReturnsFalseIfNotExists()
    {
        var dict = new AddOnlyDictionary<string, int>();
        var found = dict.TryGetValue("a", out var value);
        Assert.False(found);
    }

    [Fact]
    public void Keys_And_Values_EnumerateCorrectly()
    {
        var dict = new AddOnlyDictionary<string, int>();
        dict.Add("a", 1);
        dict.Add("b", 2);
        Assert.Contains("a", dict.Keys);
        Assert.Contains("b", dict.Keys);
        Assert.Contains(1, dict.Values);
        Assert.Contains(2, dict.Values);
    }

    [Fact]
    public void Enumerator_EnumeratesAllPairs()
    {
        var dict = new AddOnlyDictionary<string, int>();
        dict.Add("a", 1);
        dict.Add("b", 2);
        var pairs = new List<KeyValuePair<string, int>>(dict);
        Assert.Contains(new KeyValuePair<string, int>("a", 1), pairs);
        Assert.Contains(new KeyValuePair<string, int>("b", 2), pairs);
    }
}
