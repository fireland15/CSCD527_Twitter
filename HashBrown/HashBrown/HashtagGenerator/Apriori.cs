using StreamProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HashtagGenerator
{
    public class Apriori
    {
        public string[] CreateHashtagsApriori(string text, int minSupport)
        {
            string[] words = ProcessUserGeneratedText(text);

            var associations = GenerateAssociationRules(words);

            return null;

        }
        //get text string from user
        //trim the text using the tweet trim
        //generate single itemsets
        //go to database and find counts of each single itemset
        //union frequent single itemsets
        //do to database and find counts of 2 word sets
        //determine if 2 word sets are frequent
        //union 2 word sets into 3 word sets

        private string[] ProcessUserGeneratedText(string text)
        {
            string wordText = Regex.Replace(text, "[.!?\\-]", "");
            string[] words = Regex.Split(wordText, " ");
            return words;
        }

        private IList<IOrderedEnumerable<string>> UnionSets(IList<string> itemSet, int maxTupleLength)
        {
            return itemSet.Combinations(maxTupleLength).Select(set => set.OrderBy(s => s)).ToList();
        }

        private int GenerateAssociationRules(string[] words)
        {
            
            var frequentItemSets = GenerateFrequentItemSets(words, 3);
            //TODO Calculate Support and Confidence ??
            // support = # containing set/ total #
            // confidence = a -> b: # a and b / # a
            return 0;
        }

        private IEnumerable<IOrderedEnumerable<string>> GenerateFrequentItemSets(string[] words, int minSupport)
        {
            IList<string> frequentItems = new List<string>();
            foreach (string word in words)
            {
                int count = 1; // PipeLineRepository.GetCount(word);
                if (count >= minSupport)
                {
                    frequentItems.Add(word);
                }
            }

            var candidatePairs = UnionSets(frequentItems, 2);
            var frequentItemSet2 = new List<IOrderedEnumerable<string>>();

            foreach (IOrderedEnumerable<string> wordset in candidatePairs)
            {
                string firstword = wordset.First();
                int count = 1; // PipeLineRepository.GetCount(wordset.First()); //whatever method to search 2 n wordsets
                if (count >= minSupport)
                {
                    frequentItemSet2.Add(wordset);
                }
            }

            candidatePairs = UnionSets(frequentItems, 3);
            var frequentItemSet3 = new List<IOrderedEnumerable<string>>();

            foreach (IOrderedEnumerable<string> wordset in candidatePairs)
            {
                string firstword = wordset.First();
                int count = 1; // PipeLineRepository.GetCount(wordset.First()); //whatever method to search 3 n word sets
                if (count >= minSupport)
                {
                    frequentItemSet3.Add(wordset);
                }
            }

            candidatePairs = UnionSets(frequentItems, 4);
            var frequentItemSet4 = new List<IOrderedEnumerable<string>>();

            foreach (IOrderedEnumerable<string> wordset in candidatePairs)
            {
                string firstword = wordset.First();
                int count = 1; // PipeLineRepository.GetCount(wordset.First()); //whatever method to search 4 n word sets
                if (count >= minSupport)
                {
                    frequentItemSet4.Add(wordset);
                }
            }

            return frequentItemSet2.Union(frequentItemSet3).Union(frequentItemSet4);
        }

    }
    
}
