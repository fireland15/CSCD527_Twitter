using HashtagGenerator.Interfaces;
using StreamProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HashtagGenerator
{
    public class Apriori : IApriori
    {
        public string[] CreateHashtagsApriori(string text, int minSupport)
        {
            var words = ProcessUserGeneratedText(text);

            var associations = GenerateAssociationRules(words, 3, 1, 1);

            return null;
        }

       public string[] ProcessUserGeneratedText(string text)
        {
            var wordText = Regex.Replace(text, "[!@#$%^&*()+=,.]", "");
            wordText = wordText.ToLower();
            var words = Regex.Split(wordText, " ");

            return words;
        }

        public IList<IOrderedEnumerable<string>> UnionSets(IList<string> itemSet, int maxTupleLength)
        {
            return itemSet.Count == 0 ? null : itemSet.Combinations(maxTupleLength).Select(set => set.OrderBy(s => s)).ToList();
        }

        public int GenerateAssociationRules(string[] words, int minSupportFrequentItems, double minSupportRules, double minConfidenceRules)
        {

            var frequentItemSets = GenerateFrequentItemSets(words, minSupportFrequentItems);

            //TODO Calculate Support and Confidence ?????
            // support = # containing set/ total #
            // confidence = a -> b: # a and b / # a
            return 0;
        }

        public IEnumerable<IOrderedEnumerable<string>> GenerateFrequentItemSets(string[] words, int minSupport)
        {
            IList<string> frequentItemsL1 = new List<string>();
            foreach (string word in words)
            {
                var count = 1; // PipeLineRepository.GetCount(word);
                if (count >= minSupport)
                {
                    frequentItemsL1.Add(word);
                }
            }

            var candidatePairs = UnionSets(frequentItemsL1, 2);
            var frequentItemsL2 = new List<IOrderedEnumerable<string>>();

            foreach (IOrderedEnumerable<string> wordset in candidatePairs)
            {
                var count = 1; // PipeLineRepository.GetCount(wordset.First()); //whatever method to search 2 n wordsets
                if (count >= minSupport)
                {
                    frequentItemsL2.Add(wordset);
                }
            }

            candidatePairs = UnionSets(frequentItemsL1, 3);
            var frequentItemsL3 = new List<IOrderedEnumerable<string>>();

            foreach (IOrderedEnumerable<string> wordset in candidatePairs)
            {
                var count = 1; // PipeLineRepository.GetCount(wordset.First()); //whatever method to search 3 n word sets
                if (count >= minSupport)
                {
                    frequentItemsL3.Add(wordset);
                }
            }

            candidatePairs = UnionSets(frequentItemsL1, 4);
            var frequentItemsL4 = new List<IOrderedEnumerable<string>>();

            foreach (IOrderedEnumerable<string> wordset in candidatePairs)
            {
                var count = 1; // PipeLineRepository.GetCount(wordset.First()); //whatever method to search 4 n word sets
                if (count >= minSupport)
                {
                    frequentItemsL4.Add(wordset);
                }
            }

            candidatePairs = UnionSets(frequentItemsL1, 5);
            var frequentItemsL5 = new List<IOrderedEnumerable<string>>();

            foreach (IOrderedEnumerable<string> wordset in candidatePairs)
            {
                var firstword = wordset.First();
                var count = 1; // PipeLineRepository.GetCount(wordset.First()); //whatever method to search 5 n word sets
                if (count >= minSupport)
                {
                    frequentItemsL5.Add(wordset);
                }
            }

            return frequentItemsL2.Union(frequentItemsL3).Union(frequentItemsL4).Union(frequentItemsL5);
        }

    }
}