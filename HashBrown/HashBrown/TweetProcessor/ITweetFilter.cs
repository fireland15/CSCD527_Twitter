using Shared;
using Tweetinvi.Models;

namespace TweetProcessor
{
    public interface ITweetFilter
    {
        /// <summary>
        /// Accepts a tweet object and determines if it is worth keeping.
        /// </summary>
        /// <param name="tweet"></param>
        /// <returns>Null if tweet is rejected, returns a tweeter object if accepted.</returns>
        Tweeter FilterTweet(ITweet tweet);
    }
}
