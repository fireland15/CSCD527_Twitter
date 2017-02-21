using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StreamProcessor;
using System.Collections.Generic;
using System.Linq;

namespace Test.StreamProcessor
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void FastPowerSetExtensionTest()
        {
            string[] seq = new string[]{ "a", "b", "c", "d" };
            string[][] powerSet = seq.PowerSet();

            Assert.AreEqual(0, powerSet[0].Length);
        }

        [TestMethod]
        public void TestCombinations()
        {
            string[] seq = { "banana", "apple", "orange", "biscuit" };


            IEnumerable<IEnumerable<string>> combos1 = seq.Combinations(1);
            IEnumerable<IEnumerable<string>> combos2 = seq.Combinations(2);
            IEnumerable<IEnumerable<string>> combos3 = seq.Combinations(3);

            Assert.AreEqual(4, combos1.Count());
            Assert.AreEqual(6, combos2.Count());
            Assert.AreEqual(4, combos3.Count());
        }
    }
}
