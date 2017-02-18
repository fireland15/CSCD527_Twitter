using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Test.TweetTrimTest
{
    [TestClass]
    public class TweetTrimTest
    {
        [TestMethod]
        public void getStopList()
        {
            var t = new TweetTrim.TweetTrim("stopList.txt");
            var list = t.CreateStopList("stopList.txt");
            if (list.ToList().Count == 0)
            {
                throw new Exception();
            }
        }
        [TestMethod]
        public void TrimWithStopWords()
        {

        }
    }
}
