using System;
using System.Configuration;
using System.Threading;
using Tweetinvi;
using Tweetinvi.Models;
using TweetProcessor;
using TweetTrim;

namespace StreamProcessor
{
    class Program
    {
        static ITwitterCredentials Credentials;

        static void Main()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string consumerKey = config.AppSettings.Settings["ConsumerKey"].Value;
            string consumerSecret = config.AppSettings.Settings["ConsumerSecret"].Value;
            string apiToken = config.AppSettings.Settings["ApiToken"].Value;
            string apiTokenSecret = config.AppSettings.Settings["ApiTokenSecret"].Value;
            Credentials = Auth.CreateCredentials(consumerKey, consumerSecret, apiToken, apiTokenSecret);

            ITweetFilter tweetFilter = new TweetFilter();
            ITweetTrim tweetTrimmer = new TweetTrim.TweetTrim();
            IHashPairGenerator pairGenerator = new HashPairGenerator();



            TwitterStreamProcessor streamProcessor = new TwitterStreamProcessor(Credentials, tweetFilter, tweetTrimmer, pairGenerator);

            streamProcessor.Start();

            bool stopMonitoring = false;

            var t = new Thread(() =>
            {
                while (!stopMonitoring)
                {
                    try
                    {
                        Console.WriteLine($"Receiving {streamProcessor.TweetsAccepted} tweets/s.");
                    }
                    catch (DivideByZeroException)
                    {
                        /* Do nothing */
                    }
                    Thread.Sleep(1000);
                }
            });

            t.Start();

            while (true)
            {
                string s = Console.ReadLine();
                if (s == "q" || s == "quit")
                {
                    stopMonitoring = true;
                    break;
                }
            }

            Console.WriteLine(streamProcessor.Stop());

            Console.WriteLine($"Average Tweet throughput {streamProcessor.TweetThroughput} tweets/s.");
            Console.WriteLine($"Received {streamProcessor.TweetsReceived} tweets.");
            Console.WriteLine($"Accepted {streamProcessor.TweetsAccepted} tweets.");
            Console.WriteLine($"Stored {streamProcessor.WordHashtagPairsStored} word-hashtag pairs.");
        }
    }
}
