using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HashtagGenerator;
using Moq;
using Enumerable = System.Linq.Enumerable;
using Shared.Interfaces;

namespace Test.HashtagGenerator
{
    [TestClass]
    public class AprioriTest
    {
        [TestMethod]
        public void CreateHashTagsAprioriTest()
        {
        }

        [TestMethod]
        public void ProcessUserGeneratedTextTest()
        {
            var text = "This, is my! super Cool Tweet!!!";  
            var sut = new Apriori(null, null);
            var result = sut.ProcessUserGeneratedText(text);

            Assert.AreEqual(result[0], "this");
            Assert.AreEqual(result[1], "is");
            Assert.AreEqual(result[2], "my");
            Assert.AreEqual(result[3], "super");
            Assert.AreEqual(result[4], "cool");
            Assert.AreEqual(result[5], "tweet");
        }

//        [TestMethod]
//        public void UnionSetsTest_2Items()
//        {
//            var set = new List<string> {"banana", "apple", "potato"};
//            var sut = new Apriori(null);
//            
//            var result = sut.UnionSets(set, 2);
//
//            Assert.AreEqual(result[0].ElementAt(0) , "apple");
//            Assert.AreEqual(result[0].ElementAt(1), "banana");
//            Assert.AreEqual(result[1].ElementAt(0), "banana");
//            Assert.AreEqual(result[1].ElementAt(1), "potato");
//            Assert.AreEqual(result[2].ElementAt(0), "apple");
//            Assert.AreEqual(result[2].ElementAt(1), "potato");   
//        }
//
//        [TestMethod]
//        public void UnionSetsTest_3Items()
//        {
//            var set = new List<string> { "banana", "apple", "potato", "grape" };
//            var sut = new Apriori(null);
//
//            var results = sut.UnionSets(set, 3);
//
//            Assert.AreEqual(results.Count, 4);
//        }

        [TestMethod]
        public void GenerateAssociationRules()
        {

        }

        [TestMethod]
        public void GenerateFrequentItemSets()
        {
            var sut = new Apriori(null, null);
            var test = new string[] {"banana","apple","potato","grape"};

            var result = sut.GenerateFrequentItemSets(test);
        }

        [TestMethod]
        public void CalculateAssociationRulesFor2ItemSetsTest__NoSupport()
        {
            var mockrepo = new Mock<IAprioriRepository>();
            mockrepo.Setup(m => m.GetCountSingle("banana")).Returns(10);
            mockrepo.Setup(m => m.GetCountSingle("apple")).Returns(12);
            mockrepo.Setup(m => m.GetCountDouble("apple", "banana")).Returns(12);

            var sut = new Apriori(mockrepo.Object, null);

            var test = new List<string> {"apple", "banana"};

            var result = sut.CalculateAssociationRulesFor2ItemSets(test, 5);
            //Assert ??????
        }

        [TestMethod]
        public void CalculateAssociationRulesFor2ItemSetsTest__1Rule()
        {
            var mockrepo = new Mock<IAprioriRepository>();
            mockrepo.Setup(m => m.GetCountSingle("banana")).Returns(50);
            mockrepo.Setup(m => m.GetCountSingle("apple")).Returns(20);
            mockrepo.Setup(m => m.GetCountDouble("apple", "banana")).Returns(55);

            var sut = new Apriori(mockrepo.Object, null);

            var test = new List<string> { "apple", "banana" };

            var result = sut.CalculateAssociationRulesFor2ItemSets(test, 5);
            //Assert ??????
        }



    }
}
