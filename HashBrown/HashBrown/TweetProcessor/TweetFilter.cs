using System;
using Shared;
using Tweetinvi.Models;
using Tweetinvi.Models.Entities;
using System.Collections.Generic;

namespace TweetProcessor
{
    class TweetFilter : ITweetFilter
    {
        public Tweeter FilterTweet(ITweet tweet)
        {
            ITweetEntities media = tweet.Entities;
            if(media != null)
            { return null; }

            List<IHashtagEntity> hashTags = tweet.Hashtags;
            if (hashTags == null)
            { return null; }

            List<IUrlEntity> urls = tweet.Urls;
            if (urls != null)
            { return null; }

            //need to figureout what the difference is between text and full text
            Tweeter ourTweeter = new Tweeter(tweet.FullText, tweet.Hashtags);

            return ourTweeter;
        }
    }
}
