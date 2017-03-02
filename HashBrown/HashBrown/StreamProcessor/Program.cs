using Npgsql;
using Shared;
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
            string stopWordFileName = config.AppSettings.Settings["StopWordFile"].Value;
            string connectionString = config.AppSettings.Settings["connectionString"].Value;
            Credentials = Auth.CreateCredentials(consumerKey, consumerSecret, apiToken, apiTokenSecret);

            using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
            {

                ITweetFilter tweetFilter = new TweetFilter();
                //TODO: I added a location to the file name. This should be fixed later. 
                ITweetTrim tweetTrimmer = new TweetTrim.TweetTrim(stopWordFileName, "dictionary.txt");
                IHashPairGenerator pairGenerator = new HashPairGenerator();
                IPipelineRepository pipelineRepo = new PipelineRepository(con);

                TwitterStreamProcessor streamProcessor = new TwitterStreamProcessor(Credentials, tweetFilter, tweetTrimmer, pairGenerator, pipelineRepo);

                streamProcessor.Start();

                bool stopMonitoring = false;

                var t = new Thread(() =>
                {
                    while (!stopMonitoring)
                    {
                        try
                        {
                            Console.WriteLine($"Receiving {streamProcessor.TweetThroughput} tweets/s.");
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
                Console.WriteLine($"Completed in {streamProcessor.MilliSecondsRunning} ms");
            }
        }
    }
}
