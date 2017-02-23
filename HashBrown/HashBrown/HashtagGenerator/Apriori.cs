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

            GenerateAssociationRules(words, 3, 1, 1);

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

        public void GenerateAssociationRules(string[] words, int minSupportFrequentItems, double minSupportRules, double minConfidenceRules)
        {

            var frequentItemSets = GenerateFrequentItemSets(words, minSupportFrequentItems);

            int total = 100; //get the total number of transactions not sure if this should be all tweet or what

            foreach (var itemset in frequentItemSets)
            {
                switch (itemset.Count() - 1)
                {
                    //2 n itemsets
                    case 1:
                        int numOfTotalSet = 1; //database.get(AB)
                            
                        //calculate support

                        var support = (double) numOfTotalSet/total;

                        if (support > minSupportRules) //no need to continue, it doesnt meet support threshold
                        {break;}

                        
                        //calculate confidence that A->B
                        double confidenceAB = numOfTotalSet/1; //database.get(A)
                        if (confidenceAB >= minConfidenceRules)
                        {
                            //add the A->B to the ruleSet
                        }

                        //calculate confidence that B->A
                        double confidenceBA = numOfTotalSet/1; //database.get(B)
                        if (confidenceBA >= minConfidenceRules)
                        {
                            //add the A->B to the ruleSet
                        }
                        break;

                    //3 n itemsets
                    case 2:
                        numOfTotalSet = 1; //database.get(ABC)

                        //calculate support

                        support = (double)numOfTotalSet / total;

                        if (support > minSupportRules) //no need to continue, it doesnt meet support threshold
                        { break; }


                        //calculate confidence that A->BC
                        double confidenceAiBC = numOfTotalSet / 1; //database.get(A)

                        //calculate confidence that B->AC
                        double confidenceBiAC = numOfTotalSet / 1; //database.get(B)

                        //calculate confidence that C->AB
                        double confidenceCiAB = numOfTotalSet / 1; //database.get(C)

                        //calculate confidence that AB->C
                        double confidenceABiC = numOfTotalSet / 1; //database.get(AB)

                        //calculate confidence that BC->A
                        double confidenceBCiA = numOfTotalSet / 1; //database.get(BC)

                        //calculate confidence that AC->B
                        double confidenceACiB = numOfTotalSet / 1; //database.get(AC)

                        if (confidenceAiBC >= minConfidenceRules)
                        {
                            //add the A->BC to the ruleSet
                        }
                        if (confidenceBiAC >= minConfidenceRules)
                        {
                            //add the B->AC to the ruleSet
                        }
                        if (confidenceCiAB >= minConfidenceRules)
                        {
                            //add the C->AB to the ruleSet
                        }
                        if (confidenceABiC >= minConfidenceRules)
                        {
                            //add the AB->C to the ruleSet
                        }
                        if (confidenceBCiA >= minConfidenceRules)
                        {
                            //add the BC->A to the ruleSet
                        }
                        if (confidenceACiB >= minConfidenceRules)
                        {
                            //add the AC->B to the ruleSet
                        }
                        break;

                    //4 n itemsets
                    case 3:
                      //same as above.... am i forgetting a way to prune?????????
                        break;

                    //5 n itemsets
                    case 4:

                        break;
                }

            }

            // support = # containing set/ total #
            // confidence = a -> b: # a and b / # a
        }

        public IEnumerable<IOrderedEnumerable<string>> GenerateFrequentItemSets(string[] words, int minSupport)
        {
            IList<string> frequentItemsL1 = new List<string>();
            foreach (var word in words)
            {
                var count = 1; // PipeLineRepository.GetCount(word);
                if (count >= minSupport)
                {
                    frequentItemsL1.Add(word);
                }
            }

            var candidatePairs = UnionSets(frequentItemsL1, 2); //There is a better way to do this, I should not be passing items that are not frequent to be included in larger item sets
                                                                //but I am running into issues with the data types. This will work, it will just make more database calls
            var frequentItemsL2 = new List<IOrderedEnumerable<string>>();

            foreach (var wordset in candidatePairs)
            {
                var count = 1; // PipeLineRepository.GetCount(wordset.First()); //whatever method to search 2 n wordsets
                if (count >= minSupport)
                {
                    frequentItemsL2.Add(wordset);
                }
            }

            candidatePairs = UnionSets(frequentItemsL1, 3);
            var frequentItemsL3 = new List<IOrderedEnumerable<string>>();

            foreach (var wordset in candidatePairs)
            {
                var count = 1; // PipeLineRepository.GetCount(wordset.First()); //whatever method to search 3 n word sets
                if (count >= minSupport)
                {
                    frequentItemsL3.Add(wordset);
                }
            }

            candidatePairs = UnionSets(frequentItemsL1, 4);
            var frequentItemsL4 = new List<IOrderedEnumerable<string>>();

            foreach (var wordset in candidatePairs)
            {
                var count = 1; // PipeLineRepository.GetCount(wordset.First()); //whatever method to search 4 n word sets
                if (count >= minSupport)
                {
                    frequentItemsL4.Add(wordset);
                }
            }

            candidatePairs = UnionSets(frequentItemsL1, 5);
            var frequentItemsL5 = new List<IOrderedEnumerable<string>>();

            foreach (var wordset in candidatePairs)
            {
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