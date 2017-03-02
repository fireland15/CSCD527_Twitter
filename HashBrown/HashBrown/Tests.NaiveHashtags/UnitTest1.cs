using System;
using System.Collections.Generic;
using NaiveHashtag;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Interfaces;
using Shared.Naive;

namespace Tests.NaiveHashtags
{
    internal class TestNaiveRepository : INaiveRepository
    {
        public NaiveList GetNaiveList(IEnumerable<string> tweetWords)
        {
            //TODO: create it so it goes to correct Method. Right now I am going to use this to create a list
            // for testing later it will actually do the thing.
            Random rnd = new Random();
            NaiveList list = new NaiveList();
            foreach (string word in tweetWords)
            {
                ICollection<HashtagAndCount> hashList = new List<HashtagAndCount>();
                HashtagAndCount hashAndCount = new HashtagAndCount();
                hashAndCount.Count = rnd.Next(1, 50);
                hashAndCount.Hashtag = "#" + word;
                hashList.Add(hashAndCount);
                list.Add(word, hashList);
            }//end of foreach
            return list;
        }
    }

    [TestClass]
    public class TestingNaiveHashtags
    {
        [TestMethod]
        public void TestHashtagPairs()
        {
            INaiveRepository _repo = new TestNaiveRepository();
            ICollection<string> tweet = new List<string>();
            string[] tweetString = { "No", "new", "snl", "tonight!!!", "Instead", "I'll", "do", "as", "my", "Psychiatrist", "says", "by", "getting", "some", "more", "sleep", "to", "try", "to", "reduce", "my", "hallucinations", "&", "bed", "wetting.", "snl", "wet", "snl", "snl", "wet", "no" };
            for (int i = 0; i < tweetString.Length; i++)
                tweet.Add(tweetString[i]);
            NaiveHashtag.NaiveHashtag hashtag = new NaiveHashtag.NaiveHashtag(_repo); //change me to not be null
            tweet = hashtag.getHashtags(tweet, 1);
            int stopPoint = 0;
        }//end of HashtagPairs

        [TestMethod]
        public void TestDI()
        {
            INaiveRepository repo = new TestNaiveRepository();
            var naiveAlgo = new NaiveHashtag.NaiveHashtag(repo);
        }
    }
}
