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
                
                IAprioriRepository repo = new AprioriRepository(con);

               var Apriori = new Apriori(repo);

                var tweet = new string[] {"trump"};
                var stuff = Apriori.GenerateAssociationRules(tweet, 100, .01, .01);
                Console.Write(stuff);

//
//                // Test get all two item sets
//                var twoItemSets = repo.GetAll2ItemSets(new List<string>
//                {
//                    "trump",
//                    "wall",
//                    "potus"
//                });
//
//                foreach (var twoItemSet in twoItemSets)
//                {
//                    var listOfWords = twoItemSet.ToList();
//                    foreach (var word in listOfWords)
//                    {
//                        Console.Write(word);
//                        Console.Write(" ");
//                    }
//
//                    Console.WriteLine();
//                }
//
//                // Test getting count of a specific word
//                int bananaCount = repo.GetCountSingle("banana");
//                Console.WriteLine(bananaCount);
//
//                // Test getting count of a two words only
//                int trumpWallCount = repo.GetCountDouble("wall", "trump");
//                Console.WriteLine(trumpWallCount);
//
//                // Test getting count of three words only
//                int trumpWallCountPotus = repo.GetCountTriple("wall", "trump", "banana");
//                Console.WriteLine(trumpWallCountPotus);
//
//                // Test getting count of three words only
//                int tweetCount = repo.GetTotal();
//                Console.WriteLine(tweetCount);
//                
//
//                INaiveRepository naiveRepo = new NaiveRepository(con);
//                Naive naive = new Naive(naiveRepo);
//
//                var hashtags = naive.getHashtags(new string[] { "trump" }, 20);
//                foreach (var tag in hashtags)
//                {
//                    Console.WriteLine(tag);
//                }
            }
        }
    }
}
