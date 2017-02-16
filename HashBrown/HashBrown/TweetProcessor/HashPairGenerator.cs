using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using System.Collections.ObjectModel;

namespace TweetProcessor
{
    public class HashPairGenerator : IHashPairGenerator
    {
        public ICollection<WordHashtagPair> GenerateHashPairs(Tweeter tweet)
        {
            //we should decide if we want to store the text as a string or an array of words
            //What would Word set look like at this point?
            
            var words = tweet.WordSet;
            var hashTags = tweet.HashtagSet;

            ICollection<WordHashtagPair> list = new Collection<WordHashtagPair>();
            
            foreach (var word in words)
            {
                foreach (var hashtag in hashTags)
                {
                    list.Add(new WordHashtagPair
                    {
                        HashTag = hashtag,
                        Word = word
                    });
                }
            }

            return list;
        }
    }
}
