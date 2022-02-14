using System;
using System.Linq;
using NUnit.Framework;

namespace TableTopCrucible.Core.ValueTypes.Tests;

[TestFixture]
public class ValueTypeTests
{
    private FileHash buildHash(int seed = 1) =>
        FileHash.From(Enumerable.Range(seed, 64).Select(Convert.ToByte).ToArray());

    private FileHashKey buildHashKey(int seed = 1) => FileHashKey.From(FileSize.From(seed), buildHash(seed));

    private void testEquality<T>(Func<T> factory, Func<T> factoryVariant, string description = null)
    {
        var a = factory();
        var b = factory();
        var variant = factoryVariant();
        Assert.IsTrue(a.Equals(b), description);
        Assert.AreEqual(a, b, description);

        Assert.IsFalse(a.Equals(variant), description);
        Assert.AreNotEqual(a, variant, description);
    }

    [Test]
    public void FileHashTests()
    {
        testEquality(() => buildHash(), () => buildHash(10));
    }

    [Test]
    public void FileDataHashKeyTests()
    {
        testEquality(() => buildHashKey(), () => buildHashKey(10));
    }
}