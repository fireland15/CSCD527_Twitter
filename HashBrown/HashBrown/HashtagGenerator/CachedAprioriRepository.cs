using Npgsql;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashtagGenerator
{
    public class CachedAprioriRepository : IAprioriRepository
    {
        private readonly AprioriRepository _repo;

        private readonly IDictionary<Tuple<string, string>, int> _doubleWordCounts = new Dictionary<Tuple<string, string>, int>();

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
            throw new NotImplementedException();
        }

        public int GetCountTriple(string word1, string word2, string word3)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<string, string, string, int>> GetCountTripleMany(IEnumerable<IOrderedEnumerable<string>> threeItemSets)
        {
            throw new NotImplementedException();
        }

        public int GetTotal()
        {
            throw new NotImplementedException();
        }
    }
}
