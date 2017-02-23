using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashtagGenerator.Interfaces
{
    interface IApriori
    {
        string[] CreateHashtagsApriori(string text, int minSupport);

        string[] ProcessUserGeneratedText(string text);

        IList<IOrderedEnumerable<string>> UnionSets(IList<string> itemSet, int maxTupleLength);

        void GenerateAssociationRules(string[] words, int minSupportFrequentItems, double minSupportRules, double minConfidenceRules);

        IEnumerable<IOrderedEnumerable<string>> GenerateFrequentItemSets(string[] words, int minSupport);
    }

}
