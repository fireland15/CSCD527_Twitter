using HashtagGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HashBrownConsole
{
    public class TestTweets
    {
        private readonly IEnumerable<IEnumerable<string>> _tweets;

        private readonly Apriori _apriori;

        private readonly CachedAprioriRepository _repo;

        public TestTweets(IEnumerable<IEnumerable<string>> tweets, AprioriOptions options, CachedAprioriRepository repo)
        {
            _tweets = tweets;
            _repo = repo;
            _apriori = new Apriori(repo, options);
        }

        public void RunTests()
        {
            int count = 0;

            foreach (var tweet in _tweets)
            {
                using (StreamWriter sw = new StreamWriter($"apriori_tweet_{count++}.txt"))
                {
                    IEnumerable<AssociationRule> associationRules;

                    _repo.ClearCache();
                    double time = RunApriori(tweet, out associationRules);

                    sw.WriteLine($"Completed in {time} seconds");

                    sw.WriteLine("=================================================================");
                    sw.WriteLine("All Association Rules");
                    foreach (var ar in associationRules)
                    {
                        sw.WriteLine(ar.ToString());
                    }

                    var matchingAssociationRules = associationRules.Where(x => x.MatchAntecedents(tweet));
                    sw.WriteLine("=================================================================");
                    sw.WriteLine("Matching Association Rules");
                    foreach (var ar in matchingAssociationRules)
                    {
                        sw.WriteLine(ar.ToString());
                    }

                    var suggestedHashtags = matchingAssociationRules.SelectMany(x => x.Consequents);
                    sw.WriteLine("=================================================================");
                    sw.WriteLine("Suggested Hashtags");
                    foreach (var hashtag in suggestedHashtags)
                    {
                        sw.WriteLine(hashtag.ToString());
                    }
                }
            }
        }

        private double RunApriori(IEnumerable<string> tweet, out IEnumerable<AssociationRule> associationRules)
        {
            var start = DateTime.Now;

            associationRules = _apriori.GenerateAssociationRules(tweet.ToArray());

            var end = DateTime.Now;

            return (end - start).TotalSeconds;
        }
    }
}
