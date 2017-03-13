using NaiveHashtag;
using System;
using System.Collections.Generic;
using System.IO;

namespace HashBrownConsole
{
    public class TestNaive
    {
        private readonly Naive _naive;

        private readonly NaiveRepository _repo;

        private readonly IEnumerable<IEnumerable<string>> _tweets;

        public TestNaive(IEnumerable<IEnumerable<string>> tweets, NaiveRepository repo)
        {
            _repo = repo;
            _naive = new Naive(_repo);
        }

        public void RunTests()
        {
            int count = 0;

            foreach(var tweet in _tweets)
            {
                using (StreamWriter sw = new StreamWriter($"naive_tweet_{count++}.txt"))
                {
                    IEnumerable<string> hashtags;

                    double time = RunNaive(tweet, out hashtags);

                    sw.WriteLine($"Completed in {time} seconds");

                    sw.WriteLine("=============================================================");
                    sw.WriteLine("All Suggested Hashtags in order of score");
                    foreach (var h in hashtags)
                    {
                        sw.WriteLine(h);
                    }
                }
            }
        }

        private double RunNaive(IEnumerable<string> tweet, out IEnumerable<string> hashtags)
        {
            var start = DateTime.Now;

            hashtags = _naive.getHashtags(tweet, 50);

            var end = DateTime.Now;

            return (end - start).TotalSeconds;
        }
    }
}
