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

namespace HashBrownConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();

            using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
            {                
                IAprioriRepository repo = new CachedAprioriRepository(con);

                var options = new AprioriOptions
                {
                    MaximumTwoItemResults = 50,
                    ItemSetMinimumSupport = 5,
                    RuleMinimumConfidence = 0.1,
                    RuleMinimumSupport = 0.1
                };

				var Apriori = new Apriori(repo, options);

                var tweets = new List<IEnumerable<string>>();
                tweets.Add(new List<string>
                {
                    "women",
                    "awards"
                });

                var testRunner = new TestTweets(tweets, Apriori);
                testRunner.RunTests();
            }
        }
    }
}
