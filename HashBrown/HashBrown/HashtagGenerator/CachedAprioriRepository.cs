using Npgsql;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HashtagGenerator
{
    public class CachedAprioriRepository : IAprioriRepository
    {
        private readonly AprioriRepository _repo;

        private readonly IDictionary<string, int> _singleWordCounts = new Dictionary<string, int>();

        private readonly IDictionary<Tuple<string, string>, int> _doubleWordCounts = new Dictionary<Tuple<string, string>, int>();

        private readonly IDictionary<Tuple<string, string, string>, int> _tripleWordCounts = new Dictionary<Tuple<string, string, string>, int>();

        public CachedAprioriRepository(NpgsqlConnection connection)
        {
            _repo = new AprioriRepository(connection);
        }

        public List<IOrderedEnumerable<string>> GetAll2ItemSets(IList<string> words, int frequencyThreshold, int maxResults)
        {
            var wordsWithCounts = _repo.GetAll2ItemSetsWithCounts(words, frequencyThreshold, maxResults);

            List<IOrderedEnumerable<string>> sets = new List<IOrderedEnumerable<string>>();

            foreach (var w in wordsWithCounts)
            {
                _doubleWordCounts.Add(new Tuple<string, string>(w.Item1, w.Item2), w.Item3);

                IOrderedEnumerable<string> ordered = new List<string>
                {
                    w.Item1,
                    w.Item2
                }.OrderBy(x => x);

                sets.Add(ordered);
            }

            return sets;
        }

        public int GetCountDouble(string word1, string word2)
        {
            var tuple = new Tuple<string, string>(word1, word2);
            var revTuple = new Tuple<string, string>(word2, word1);

            if (_doubleWordCounts.ContainsKey(tuple))
            {
                return _doubleWordCounts[tuple];
            }
            else if (_doubleWordCounts.ContainsKey(revTuple))
            {
                return _doubleWordCounts[revTuple];
            }
            else
            {
                int count = _repo.GetCountDouble(word1, word2);
                _doubleWordCounts.Add(tuple, count);
                return count;
            }
        }

        public int GetCountSingle(string word)
        {
            if (_singleWordCounts.ContainsKey(word))
            {
                return _singleWordCounts[word];
            }
            else
            {
                int count = _repo.GetCountSingle(word);
                _singleWordCounts.Add(word, count);
                return count;
            }
        }

        public int GetCountTriple(string word1, string word2, string word3)
        {
            List<string> words = new List<string>
            {
                word1,
                word2,
                word3
            }.OrderBy(x => x).ToList();

            var key = new Tuple<string, string, string>(words[0], words[1], words[2]);

            if (_tripleWordCounts.ContainsKey(key))
            {
                return _tripleWordCounts[key];
            }
            else
            {
                int count = _repo.GetCountTriple(words[0], words[1], words[2]);
                _tripleWordCounts.Add(key, count);
                return count;
            }
        }

        public IEnumerable<Tuple<string, string, string, int>> GetCountTripleMany(IEnumerable<IOrderedEnumerable<string>> threeItemSets)
        {
            List<Tuple<string, string, string, int>> results = new List<Tuple<string, string, string, int>>();
            List<IOrderedEnumerable<string>> needToQueryFor = new List<IOrderedEnumerable<string>>();

            foreach (var set in threeItemSets)
            {
                var list = set.ToList();
                var words = new Tuple<string, string, string>(list[0], list[1], list[2]);
                if (_tripleWordCounts.ContainsKey(words))
                {
                    var count = _tripleWordCounts[words];
                    var tuple = new Tuple<string, string, string, int>(words.Item1, words.Item2, words.Item3, count);
                    results.Add(tuple);
                }
                else
                {
                    needToQueryFor.Add(set);
                }
            }

            var queryResults = _repo.GetCountTripleMany(needToQueryFor);

            foreach(var queryResult in queryResults)
            {
                var key = new Tuple<string, string, string>(queryResult.Item1, queryResult.Item2, queryResult.Item3);
                _tripleWordCounts.Add(key, queryResult.Item4);

                results.Add(queryResult);
            }

            return results;
        }

        public int GetTotal()
        {
            return _repo.GetTotal();
        }

        public void ClearCache()
        {
            _singleWordCounts.Clear();
            _doubleWordCounts.Clear();
            _tripleWordCounts.Clear();
        }
    }
}
