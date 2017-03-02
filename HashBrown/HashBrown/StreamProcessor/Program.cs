using Npgsql;
using Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
            string stopWordFileName = config.AppSettings.Settings["StopWordFile"].Value;
            string connectionStringPipeline = config.AppSettings.Settings["connectionStringPipeline"].Value;
            string connectionStringPostGre = config.AppSettings.Settings["connectionStringPostGre"].Value;
            string performanceLog = config.AppSettings.Settings["performanceLog"].Value;
            Credentials = Auth.CreateCredentials(consumerKey, consumerSecret, apiToken, apiTokenSecret);

            TwitterStreamProcessor streamProcessor = null;

            bool stopMonitoring = false;

            var st = new Thread(() =>
            {
                while (!stopMonitoring)
                {
                    try
                    {
                        using (NpgsqlConnection conPipeline = new NpgsqlConnection(connectionStringPipeline))
                        {
                            ITweetFilter tweetFilter = new TweetFilter();
                            ITweetTrim tweetTrimmer = new TweetTrim.TweetTrim(stopWordFileName);
                            IHashPairGenerator pairGenerator = new HashPairGenerator();
                            IPipelineRepository repo = new PipelineRepository(conPipeline);

                            if (conPipeline.State == System.Data.ConnectionState.Open)
                            {
                                streamProcessor = new TwitterStreamProcessor(Credentials, tweetFilter, tweetTrimmer, pairGenerator, repo);
                                streamProcessor.Start();
                            }
                        }
                        Thread.Sleep(5000);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            });

            st.Start();

            var t = new Thread(() =>
            {
                int lastTweetsReceived = 0;
                int lastTweetsAccepted = 0;
                int lastPairsStored = 0;
                Dictionary<int, int> lastWordSetsStored = new Dictionary<int, int> { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 } };

                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

                while (!stopMonitoring)
                {
                    try
                    {
                        int tweetsReceived = (int)streamProcessor.TweetsReceived;
                        int newTweets = tweetsReceived - lastTweetsReceived;
                        lastTweetsReceived = tweetsReceived;

                        int tweetsAccepted = (int)streamProcessor.TweetsAccepted;
                        int newTweetsAccepted = tweetsAccepted - lastTweetsAccepted;
                        lastTweetsAccepted = tweetsAccepted;

                        int pairsStored = (int)streamProcessor.WordHashtagPairsStored;
                        int newPairsStored = pairsStored - lastPairsStored;
                        lastPairsStored = pairsStored;

                        int[] newWordSetsStored = new int[5];

                        for (int i = 1; i <= 5; i++)
                        {
                            int wordSetsStored = (int)streamProcessor.WordSetsStored[i];
                            newWordSetsStored[i - 1] = wordSetsStored - lastWordSetsStored[i];
                            lastWordSetsStored[i] = wordSetsStored;
                        }

                        double timestamp = (DateTime.Now.ToUniversalTime() - origin).TotalSeconds;

                        using (StreamWriter sw = new FileInfo(performanceLog).AppendText())
                        {
                            sw.WriteLine($"{DateTime.Now.ToLongTimeString()},{newTweets},{newTweetsAccepted},{newPairsStored},{newWordSetsStored[0]},{newWordSetsStored[1]},{newWordSetsStored[2]},{newWordSetsStored[3]},{newWordSetsStored[4]}, {streamProcessor.State}");
                        }
                                
                    }
                    catch (Exception)
                    {
                        /* Do nothing */
                    }
                    Thread.Sleep(5000);
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

            st.Join();

            Console.WriteLine($"Average Tweet throughput {streamProcessor.TweetThroughput} tweets/s.");
            Console.WriteLine($"Received {streamProcessor.TweetsReceived} tweets.");
            Console.WriteLine($"Accepted {streamProcessor.TweetsAccepted} tweets.");
            Console.WriteLine($"Stored {streamProcessor.WordHashtagPairsStored} word-hashtag pairs.");
            Console.WriteLine($"Completed in {streamProcessor.MilliSecondsRunning} ms");
        }
    }
}
