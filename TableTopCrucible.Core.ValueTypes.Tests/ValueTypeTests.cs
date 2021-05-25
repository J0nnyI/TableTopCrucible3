using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TableTopCrucible.Data.Library.DataTransfer.Models;

namespace TableTopCrucible.Core.ValueTypes.Tests
{
    [TestClass()]
    public class ValueTypeTests
    {

        private FileHash buildHash(int seed = 1)
            => FileHash.From(Enumerable.Range(seed, 64).Select(i => Convert.ToByte(i)).ToArray());
        private FileHashKey buildHashKey(int seed = 1)
            => FileHashKey.From((buildHash(seed), FileSize.From(seed)));

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

        [TestMethod()]
        public void FileHashTests()
        {
            testEquality(() => buildHash(1), () => buildHash(10));
        }
        [TestMethod]
        public void FileDataHashKeyTests()
        {
            testEquality(() => buildHashKey(1), () => buildHashKey(10));
        }
    }
}