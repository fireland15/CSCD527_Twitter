using System;
using Shared;
using Tweetinvi.Models;
using Tweetinvi.Models.Entities;
using System.Collections.Generic;

namespace TweetProcessor
{
    public class TweetFilter : ITweetFilter
    {
        public Tweeter FilterTweet(ITweet tweet)
        {
            ITweetEntities media = tweet.Entities;
            if(media.Medias.Count != 0)
            { return null; }

            List<IHashtagEntity> hashTags = tweet.Hashtags;
            if (hashTags.Count == 0)
            { return null; }

            List<IUrlEntity> urls = tweet.Urls;
            if (urls.Count > 0)
            { return null; }

            //need to figureout what the difference is between text and full text
            Tweeter ourTweeter = new Tweeter(tweet.FullText, tweet.Hashtags);

            return ourTweeter;
        }
    }
}
