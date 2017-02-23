using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using Tweetinvi.Models.Entities;

namespace Test.TweetTrimTest
{
    [TestClass]
    public class TweetTrimTest
    {
        [TestMethod]
        public void getStopList()
        {
            TweetTrim.TweetTrim t = new TweetTrim.TweetTrim("stopList.txt", "dictionary.txt");
            var list = t.StopList;
            if (list.ToList().Count == 0)
            {
                throw new Exception("Couldn't find stopList.txt");
            }
            list = t.ValidWords;
            if(list.ToList().Count == 0)
            {
                throw new Exception("Couldn't find dictionary.txt");
            }
        }
        [TestMethod]
        public void TrimWithStopWords()
        {
            TweetTrim.TweetTrim t = new TweetTrim.TweetTrim("stopList.txt", "dictionary.txt");
            ICollection<IHashtagEntity> hashTagSet = new List<IHashtagEntity>();
            Shared.Tweeter tweet = new Shared.Tweeter("No new #SNL tonight!!! Instead, I'll do as my Psychiatrist says by getting some more sleep to try to reduce my hallucinations & bed wetting.", hashTagSet);
            t.Trim(tweet);
        }
    }
}
