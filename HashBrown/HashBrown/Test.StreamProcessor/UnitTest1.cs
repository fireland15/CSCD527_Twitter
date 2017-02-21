using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StreamProcessor;

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
    }
}
