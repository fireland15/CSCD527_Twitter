using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TweetTrim;
using System.Linq;

namespace Test.TweetTrimTest
{
    [TestClass]
    public class TweetTrimTest
    {
        [TestMethod]
        public void getStopList()
        {
            var t = new TweetTrim.TweetTrim();
            var list = t.CreateStopList();
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
