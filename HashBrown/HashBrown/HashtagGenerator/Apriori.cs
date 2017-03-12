using System;
using HashtagGenerator.Interfaces;
using StreamProcessor;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Shared;
using Shared.Interfaces;

namespace HashtagGenerator
{
    public class AprioriOptions
    {
        public int ItemSetMinimumSupport { get; set; }
        public double RuleMinimumSupport { get; set; }
        public double RuleMinimumConfidence { get; set; }
        public int MaximumTwoItemResults { get; set; }
    }

    public class Apriori : IApriori
    {
        private IAprioriRepository _repository { get; }

        private readonly AprioriOptions _options;

        public Apriori(IAprioriRepository repo, AprioriOptions options)
        {
            _repository = repo;
            _options = options;
        }

        public string[] CreateHashtagsApriori(string text, int minSupport)
        {
            var words = ProcessUserGeneratedText(text);

            GenerateAssociationRules(words);

            return null;
        }

        public string[] ProcessUserGeneratedText(string text)
        {
            var wordText = Regex.Replace(text, "[!@#$%^&*()+=,.]", "");
            wordText = wordText.ToLower();
            var words = Regex.Split(wordText, " ");

            return words;
        }

        public IList<IOrderedEnumerable<string>> UnionSets(IList<IOrderedEnumerable<string>> itemSet, int maxTupleLength)
        {
            var pairs = itemSet.Combinations(2);

            var list = new List<IOrderedEnumerable<string>>();

            foreach (var pair in pairs)
            {
                var temp = pair.ToList();

                var unioned = temp[0].Union(temp[1]);

                list.Add(unioned.OrderBy(x => x));
            }

            return list ;
        }

        public List<AssociationRule> GenerateAssociationRules(string[] words)
        {

            var frequentItemSets = GenerateFrequentItemSets(words);

            var associationRulesList = new List<AssociationRule>();

            var total = 50;//_repository.GetTotal();

            foreach (var itemset in frequentItemSets)
            {
                var itemList = itemset.ToList();
                switch (itemset.Count() - 1)
                {
                    //2 n itemsets
                    case 1:
                        associationRulesList.AddRange(CalculateAssociationRulesFor2ItemSets(itemList, total));
                        break;

                    //3 n itemsets
                    case 2:
                        associationRulesList.AddRange(CalculateAssociationRulesFor3ItemSets(itemList, total));
                        break;
                }
            }
            return associationRulesList;
        }
        
        public List<AssociationRule> CalculateAssociationRulesFor2ItemSets(List<string> itemList, int total)
        {

            var associationRulesList = new List<AssociationRule>();
            int numOfTotalSet = _repository.GetCountDouble(itemList[0], itemList[1]); //database.get(AB)

            //calculate support

            var support = (double)numOfTotalSet / total; 

            if (support < _options.RuleMinimumSupport) //no need to continue, it doesnt meet support threshold
            {
                return associationRulesList;
            }


            //calculate confidence that A->B
            var confidenceAB = (double)numOfTotalSet / _repository.GetCountSingle(itemList[0]); //database.get(A)
            if (confidenceAB >= _options.RuleMinimumConfidence)
            {
                //add the A->B to the ruleSet
                //associationRulesList.Add(itemList[0] + " => " + itemList[1]);
                associationRulesList.Add(new AssociationRule
                {
                    Antecedents = new List<string>
                    {
                        itemList[0]
                    },
                    Consequents = new List<string>
                    {
                        itemList[1]
                    },
                    Confidence = confidenceAB,
                    Support = support
                });
            }

            //calculate confidence that B->A
            var confidenceBA = (double)numOfTotalSet / _repository.GetCountSingle(itemList[1]); //database.get(B)
            if (confidenceBA >= _options.RuleMinimumConfidence)
            {
                //add the B -> A to the ruleSet
                associationRulesList.Add(new AssociationRule
                {
                    Antecedents = new List<string>
                    {
                        itemList[1]
                    },
                    Consequents = new List<string>
                    {
                        itemList[0]
                    },
                    Confidence = confidenceAB,
                    Support = support
                });
            }

            return associationRulesList;
        }
        
        public List<AssociationRule> CalculateAssociationRulesFor3ItemSets(List<string> itemList, int total)
        {
            var associationRulesList = new List<AssociationRule>();
            var numOfTotalSet = _repository.GetCountTriple(itemList[0], itemList[1], itemList[2]); //database.get(ABC)

            //calculate support

            var support = (double)numOfTotalSet / total;

            if (support > _options.RuleMinimumSupport) //no need to continue, it doesnt meet support threshold
            {
                return associationRulesList;
            }


            //calculate confidence that A->BC
            var confidenceAiBC = (double)numOfTotalSet / _repository.GetCountSingle(itemList[0]); //database.get(A)

            //calculate confidence that B->AC
            var confidenceBiAC = (double)numOfTotalSet / _repository.GetCountSingle(itemList[1]); //database.get(B)

            //calculate confidence that C->AB
            var confidenceCiAB = (double)numOfTotalSet / _repository.GetCountSingle(itemList[2]); //database.get(C)

            //calculate confidence that AB->C
            var confidenceABiC = (double)numOfTotalSet / _repository.GetCountDouble(itemList[0], itemList[1]); //database.get(AB)

            //calculate confidence that BC->A
            var confidenceBCiA = (double)numOfTotalSet / _repository.GetCountDouble(itemList[1], itemList[2]); //database.get(BC)

            //calculate confidence that AC->B
            var confidenceACiB = (double)numOfTotalSet / _repository.GetCountDouble(itemList[0], itemList[2]); //database.get(AC)

            if (confidenceAiBC >= _options.RuleMinimumConfidence)
            {
                //add the A->BC to the ruleSet
                associationRulesList.Add(new AssociationRule
                {
                    Antecedents = new List<string>
                    {
                        itemList[0]
                    },
                    Consequents = new List<string>
                    {
                        itemList[1],
                        itemList[2]
                    },
                    Confidence = confidenceAiBC,
                    Support = support
                });
            }
            if (confidenceBiAC >= _options.RuleMinimumConfidence)
            {
                //add the B->AC to the ruleSet
                associationRulesList.Add(new AssociationRule
                {
                    Antecedents = new List<string>
                    {
                        itemList[1]
                    },
                    Consequents = new List<string>
                    {
                        itemList[0],
                        itemList[2]
                    },
                    Confidence = confidenceBiAC,
                    Support = support
                });
            }
            if (confidenceCiAB >= _options.RuleMinimumConfidence)
            {
                //add the C->AB to the ruleSet
                associationRulesList.Add(new AssociationRule
                {
                    Antecedents = new List<string>
                    {
                        itemList[2]
                    },
                    Consequents = new List<string>
                    {
                        itemList[0],
                        itemList[1]
                    },
                    Confidence = confidenceCiAB,
                    Support = support
                });
            }
            if (confidenceABiC >= _options.RuleMinimumConfidence)
            {
                //add the AB->C to the ruleSet
                associationRulesList.Add(new AssociationRule
                {
                    Antecedents = new List<string>
                    {
                        itemList[0],
                        itemList[1]
                    },
                    Consequents = new List<string>
                    {
                        itemList[2]
                    },
                    Confidence = confidenceABiC,
                    Support = support
                });
            }
            if (confidenceBCiA >= _options.RuleMinimumConfidence)
            {
                //add the BC->A to the ruleSet
                associationRulesList.Add(new AssociationRule
                {
                    Antecedents = new List<string>
                    {
                        itemList[1],
                        itemList[2]
                    },
                    Consequents = new List<string>
                    {
                        itemList[0]
                    },
                    Confidence = confidenceBCiA,
                    Support = support
                });
            }
            if (confidenceACiB >= _options.RuleMinimumConfidence)
            {
                //add the AC->B to the ruleSet
                associationRulesList.Add(new AssociationRule
                {
                    Antecedents = new List<string>
                    {
                        itemList[0],
                        itemList[2]
                    },
                    Consequents = new List<string>
                    {
                        itemList[1]
                    },
                    Confidence = confidenceACiB,
                    Support = support
                });
            }
            return associationRulesList;
        }

        //one call
        public IEnumerable<IOrderedEnumerable<string>> GenerateFrequentItemSets(string[] words)
        {
            IList<string> frequentItemsL1 = new List<string>();
            foreach (var word in words)
            {
                var count = _repository.GetCountSingle(word); // PipeLineRepository.GetCount(word);
                if (count >= _options.ItemSetMinimumSupport)
                {
                    frequentItemsL1.Add(word);
                }
            }

            if (frequentItemsL1.Count == 0)
            {
                throw new System.ArgumentException("Tweet is not popular enough for Apriori Analysis");
            }
           
            var frequentItemsL2 = _repository.GetAll2ItemSets(frequentItemsL1, _options.ItemSetMinimumSupport, _options.MaximumTwoItemResults)
                .OrderByDescending(x => _repository.GetCountDouble(x.First(), x.Last())).ToList();
            var candidateTriples = UnionSets(frequentItemsL2, 3);
            var frequentItemsL3 = _repository
                .GetCountTripleMany(candidateTriples)
                .Select(x => new List<string>
                {
                    x.Item1,
                    x.Item2,
                    x.Item3
                }.OrderBy(y => y));
            
            return frequentItemsL2.Union(frequentItemsL3);
        }
    }
}