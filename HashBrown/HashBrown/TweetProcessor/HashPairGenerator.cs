using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using System.Collections.ObjectModel;

namespace TweetProcessor
{
    class HashPairGenerator : IHashPairGenerator
    {
        public ICollection<WordHashtagPair> GenerateHashPairs(Tweeter tweet)
        {
            //we should decide if we want to store the text as a string or an array of words
            //What would Word set look like at this point?

            var text = tweet.WordSet;
            var arrayOfWords = tweet.WordSet;
            var hashTags = tweet.HashtagSet;

            ICollection<WordHashtagPair> list = new Collection<WordHashtagPair>();
            
            //for each word

            //for each hashtag
            list.Add(new WordHashtagPair());


            return list;

        }
    }
}
