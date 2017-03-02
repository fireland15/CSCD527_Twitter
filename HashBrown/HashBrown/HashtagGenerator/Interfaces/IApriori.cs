using System;
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

        IList<IOrderedEnumerable<string>> UnionSets(IList<string> itemSet, int maxTupleLength);

        List<string> GenerateAssociationRules(string[] words, int minSupportFrequentItems, double minSupportRules, double minConfidenceRules);

        IEnumerable<IOrderedEnumerable<string>> GenerateFrequentItemSets(string[] words, int minSupport);

        List<string> CalculateAssociationRulesFor3ItemSets(List<string> itemList, double minSupportRules, double minConfidenceRules, int total);

        List<string> CalculateAssociationRulesFor2ItemSets(List<string> itemList, double minSupportRules, double minConfidenceRules, int total);
    }

}
