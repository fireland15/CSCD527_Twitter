using System;
using System.Collections.Generic;
using NaiveHashtag;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.NaiveHashtags
{
    [TestClass]
    public class TestingNaiveHashtags
    {
        [TestMethod]
        public void TestHashtagPairs()
        {
            ICollection<string> tweet = new List<string>();
            string[] tweetString = { "No", "new", "#SNL", "tonight!!!", "Instead", "I'll", "do", "as", "my", "Psychiatrist", "says", "by", "getting", "some", "more", "sleep", "to", "try", "to", "reduce", "my", "hallucinations", "&", "bed", "wetting." };
            for (int i = 0; i < tweetString.Length; i++)
                tweet.Add(tweetString[i]);
            NaiveHashtag.NaiveHashtag hashtag = new NaiveHashtag.NaiveHashtag();
            tweet = hashtag.getHashtags(tweet, 3);
        }//end of HashtagPairs
    }
}
