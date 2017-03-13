using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HashtagGenerator;
using Npgsql;
using System.Configuration;
using Shared.Interfaces;
using NaiveHashtag;
using System.IO;

namespace HashBrownConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();
            string pathToTestTweets = ConfigurationManager.AppSettings["pathToTestTweets"].ToString();

            var tweets = ReadTestTweets(pathToTestTweets);

            TestApriori(connectionString, tweets);

            TestNaive(connectionString, tweets);
        }

        static IEnumerable<IEnumerable<string>> ReadTestTweets(string path)
        {
            var tweets = new List<IEnumerable<string>>();

            using (StreamReader sr = new StreamReader(path))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    var words = line.Split(' ');
                    tweets.Add(words);
                }
            }

            return tweets;
        }

        static void TestApriori(string connectionString, IEnumerable<IEnumerable<string>> tweets)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
            {
                CachedAprioriRepository repo = new CachedAprioriRepository(con);

                var options = new AprioriOptions
                {
                    MaximumTwoItemResults = 20,
                    ItemSetMinimumSupport = 5,
                    RuleMinimumConfidence = 0.1,
                    RuleMinimumSupport = 0.1
                };

                var testRunner = new TestTweets(tweets, options, repo);
                testRunner.RunTests();
            }
        }

        static void TestNaive(string connectionString, IEnumerable<IEnumerable<string>> tweets)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
            {
                NaiveRepository repo = new NaiveRepository(con);

                var testRunner = new TestNaive(tweets, repo);
                testRunner.RunTests();
            }
        }
    }
}
