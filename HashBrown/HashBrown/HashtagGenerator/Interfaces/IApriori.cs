﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashtagGenerator.Interfaces
{
    public interface IApriori
    {
        string[] CreateHashtagsApriori(string text, int minSupport);

        string[] ProcessUserGeneratedText(string text);
        
        IList<IOrderedEnumerable<string>> UnionSets(IList<IOrderedEnumerable<string>> itemSet, int maxTupleLength);

        List<string> GenerateAssociationRules(string[] words);

        IEnumerable<IOrderedEnumerable<string>> GenerateFrequentItemSets(string[] words);

        List<string> CalculateAssociationRulesFor3ItemSets(List<string> itemList, int total);

        List<string> CalculateAssociationRulesFor2ItemSets(List<string> itemList, int total);
    }

}
