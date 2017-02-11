using Shared;
using System.Collections.Generic;

namespace TweetProcessor
{
    public interface IHashPairGenerator
    {
        /// <summary>
        /// Takes a tweeter object and computes the Word and Hashtag pairs from it.
        /// </summary>
        /// <param name="tweet"></param>
        /// <returns></returns>
        ICollection<WordHashtagPair> GenerateHashPairs(Tweeter tweet);
    }
}
