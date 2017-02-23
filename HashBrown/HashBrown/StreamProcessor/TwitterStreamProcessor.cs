using Shared;
using System;
using System.Linq;
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

        private readonly IPipelineRepository _repo;

        private DateTime _startOn;

        private DateTime _stopOn;

        private Thread _streamThread;

        public uint TweetsReceived { get; set; }

        public uint TweetsAccepted { get; set; }

        public uint WordHashtagPairsStored { get; set; }

        public int MilliSecondsRunning => DateTime.Now.Subtract(_startOn).Milliseconds;

        public uint AverageTweetThroughput => TweetsReceived / (uint)(_stopOn.Subtract(_startOn).TotalSeconds);

        public uint TweetThroughput => TweetsReceived / (uint)DateTime.Now.Subtract(_startOn).TotalSeconds;

        public TwitterStreamProcessor(ITwitterCredentials credentials, ITweetFilter tweetFilter, ITweetTrim tweetTrimmer, IHashPairGenerator hashPairGenerator, IPipelineRepository repo)
        {
            _credentials = credentials;
            _tweetFilter = tweetFilter;
            _tweetTrimmer = tweetTrimmer;
            _hashPairGenerator = hashPairGenerator;
            _repo = repo;

            _stream = Stream.CreateSampleStream(_credentials);
            _stream.AddTweetLanguageFilter(LanguageFilter.English);
        }

        public void Start()
        {
            RegisterEventHandlers();

            _streamThread = new Thread(() =>
            {
                Console.WriteLine("StartingStream");
                _stream.StartStream();
            });

            _startOn = DateTime.Now;
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
            _stopOn = DateTime.Now;
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
            //if (!trimmedTweeter.IsValid())
            //{
            //    return;
            //}

            TweetsAccepted++;

            try
            {
                // Todo: Make sure to remove duplicate words and hashtags
                ICollection<WordHashtagPair> wordHashPairs = _hashPairGenerator.GenerateHashPairs(trimmedTweeter);
                if (wordHashPairs.Count != 0)
                {
                    PersistWordHashPairs(wordHashPairs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                PersistTuples(trimmedTweeter, 3);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void PersistWordHashPairs(ICollection<WordHashtagPair> wordHashPairs)
        {
            _repo.InsertMany(wordHashPairs);
            WordHashtagPairsStored += (uint)wordHashPairs.Count;
        }

        private void PersistTuples(Tweeter tweeter, int maxTupleLength = 1)
        {
            var allWords = tweeter.WordSet.Distinct();

            // make sure we arent trying to make combinations longer than the original
            maxTupleLength = maxTupleLength > allWords.Count() ? allWords.Count() : maxTupleLength;

            IEnumerable<IOrderedEnumerable<string>> nWordSets = allWords.Combinations(1).Select(set => set.OrderBy(s => s));

            for (int i = 2; i <= maxTupleLength; i++)
            {
                var combos = allWords.Combinations(i).Select(set => set.OrderBy(s => s));
                nWordSets = nWordSets.Union(combos);
            }

            foreach (var set in nWordSets)
            {
                _repo.InsertNTuple(set.ToArray());
            }
        }
    }
}
