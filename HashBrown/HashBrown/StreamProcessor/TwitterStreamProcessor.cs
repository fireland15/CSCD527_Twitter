using Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using Tweetinvi;
using Tweetinvi.Events;
using Tweetinvi.Models;
using Tweetinvi.Streaming;
using TweetProcessor;
using TweetTrim;

namespace StreamProcessor
{
    public class TwitterStreamProcessor
    {
        private readonly ISampleStream _stream;

        private readonly ITwitterCredentials _credentials;

        private readonly ITweetFilter _tweetFilter;

        private readonly ITweetTrim _tweetTrimmer;

        private readonly IHashPairGenerator _hashPairGenerator;

        private Thread _streamThread;

        public uint TweetsReceived { get; set; } = 0;

        public uint TweetsAccepted { get; set; } = 0;

        public uint WordHashtagPairsStored { get; set; } = 0;

        public TwitterStreamProcessor(ITwitterCredentials credentials, ITweetFilter tweetFilter, ITweetTrim tweetTrimmer, IHashPairGenerator hashPairGenerator)
        {
            _credentials = credentials;
            _tweetFilter = tweetFilter;
            _tweetTrimmer = tweetTrimmer;
            _hashPairGenerator = hashPairGenerator;

            _stream = Stream.CreateSampleStream(_credentials);
            _stream.AddTweetLanguageFilter(LanguageFilter.English);
        }

        public void Start()
        {
            RegisterEventHandlers();

            _streamThread = new Thread(() =>
            {
                _stream.StartStream();
            });

            _streamThread.Start();
        }

        public string Stop()
        {
            _stream.StopStream();
            _streamThread.Join();
            while (_stream.StreamState != StreamState.Stop)
            {
                /* Wait for stream to stop */
            }
            return "Stream stopped";
        }

        private void RegisterEventHandlers()
        {
            _stream.TweetReceived += ReceiveTweet;
        }

        private void ReceiveTweet(object sender, TweetReceivedEventArgs args)
        {
            TweetsReceived++;
            ITweet tweet = args.Tweet;
            ProcessTweet(tweet);
        }

        private void ProcessTweet(ITweet tweet)
        {
            if (tweet == null)
            {
                return;
            }

            Tweeter tweeter = _tweetFilter.FilterTweet(tweet);
            if (tweeter == null)
            {
                return;
            }

            Tweeter trimmedTweeter = _tweetTrimmer.Trim(tweeter);
            if (!trimmedTweeter.IsValid())
            {
                return;
            }

            TweetsAccepted++;

            ICollection<WordHashtagPair> wordHashPairs = _hashPairGenerator.GenerateHashPairs(trimmedTweeter);
            if (wordHashPairs.Count != 0)
            {
                PersistWordHashPairs(wordHashPairs);
            }
        }

        private void PersistWordHashPairs(ICollection<WordHashtagPair> wordHashPairs)
        {
            foreach (var pair in wordHashPairs)
            {
                //Console.WriteLine($"{pair.Word}, {pair.HashTag}");
                WordHashtagPairsStored++;
            }
        }
    }
}
