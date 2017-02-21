using System;
using Shared;
using Tweetinvi.Models;
using Tweetinvi.Models.Entities;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

            string cleanText = Regex.Replace(tweet.FullText, @"[^\u0020,^\u0030-\u0039,^\u0041-\u005A,^\u0061-\u007A]+", string.Empty);

            //need to figureout what the difference is between text and full text
            Tweeter ourTweeter = new Tweeter(cleanText, tweet.Hashtags);

            return ourTweeter;
        }


    }
}
