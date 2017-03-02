using HashtagGenerator.Interfaces;
using StreamProcessor;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Shared;
using Shared.Interfaces;

namespace HashtagGenerator
{
    public class Apriori : IApriori
    {
        private IAprioriRepository _repository { get; set; }

       public Apriori(IAprioriRepository repo)
        {
            _repository = repo;
        }

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

            var total = _repository.GetTotal();

            foreach (var itemset in frequentItemSets)
            {
                var itemList = itemset.ToList();
                switch (itemset.Count() - 1)
                {
                        
                    //2 n itemsets
                    case 1:
                        int numOfTotalSet = _repository.GetCountDouble(itemList[0],itemList[1]); //database.get(AB)
                            
                        //calculate support

                        var support = (double) numOfTotalSet/total;

                        if (support > minSupportRules) //no need to continue, it doesnt meet support threshold
                        {break;}

                        
                        //calculate confidence that A->B
                        var confidenceAB = (double) numOfTotalSet/_repository.GetCountSingle(itemList[0]); //database.get(A)
                        if (confidenceAB >= minConfidenceRules)
                        {
                            //add the A->B to the ruleSet
                        }

                        //calculate confidence that B->A
                        var confidenceBA = (double) numOfTotalSet/ _repository.GetCountSingle(itemList[1]); //database.get(B)
                        if (confidenceBA >= minConfidenceRules)
                        {
                            //add the A->B to the ruleSet
                        }
                        break;

                    //3 n itemsets
                    case 2:
                        numOfTotalSet = _repository.GetCountTriple(itemList[0], itemList[1], itemList[2]); //database.get(ABC)

                        //calculate support

                        support = (double)numOfTotalSet / total;

                        if (support > minSupportRules) //no need to continue, it doesnt meet support threshold
                        { break; }


                        //calculate confidence that A->BC
                        var confidenceAiBC = (double) numOfTotalSet / _repository.GetCountSingle(itemList[0]); //database.get(A)

                        //calculate confidence that B->AC
                        var confidenceBiAC = (double) numOfTotalSet / _repository.GetCountSingle(itemList[1]); //database.get(B)

                        //calculate confidence that C->AB
                        var confidenceCiAB = (double) numOfTotalSet / _repository.GetCountSingle(itemList[2]); //database.get(C)

                        //calculate confidence that AB->C
                        var confidenceABiC = (double) numOfTotalSet / _repository.GetCountDouble(itemList[0], itemList[1]); //database.get(AB)

                        //calculate confidence that BC->A
                        var confidenceBCiA = (double) numOfTotalSet / _repository.GetCountDouble(itemList[1], itemList[2]); //database.get(BC)

                        //calculate confidence that AC->B
                        var confidenceACiB = (double) numOfTotalSet / _repository.GetCountDouble(itemList[0], itemList[2]); //database.get(AC)

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
                var count = _repository.GetCountSingle(word); // PipeLineRepository.GetCount(word);
                if (count >= minSupport)
                {
                    frequentItemsL1.Add(word);
                }
            }

            var candidatePairs = UnionSets(frequentItemsL1, 2); 
            var frequentItemsL2 = new List<IOrderedEnumerable<string>>();

            foreach (var wordset in candidatePairs)
            {
                var count = _repository.GetCountDouble(wordset.ToList()[0], wordset.ToList()[1]); // PipeLineRepository.GetCount(wordset.First()); //whatever method to search 2 n wordsets
                if (count >= minSupport)
                {
                    frequentItemsL2.Add(wordset);
                }
            }

            candidatePairs = UnionSets(frequentItemsL1, 3);
            var frequentItemsL3 = new List<IOrderedEnumerable<string>>();

            foreach (var wordset in candidatePairs)
            {
                var count = _repository.GetCountTriple(wordset.ToList()[0], wordset.ToList()[1], wordset.ToList()[2]); // PipeLineRepository.GetCount(wordset.First()); //whatever method to search 3 n word sets
                if (count >= minSupport)
                {
                    frequentItemsL3.Add(wordset);
                }
            }

            return frequentItemsL2.Union(frequentItemsL3);
        }

    }
}