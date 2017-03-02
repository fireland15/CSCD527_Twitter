using System.Collections.Generic;
using System.Linq;

namespace Shared.Interfaces
{
    public interface IAprioriRepository
    {
        /// <summary>
        /// Get the count of transactions that contain the provided word set
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        int GetCountSingle(string word);

        /// <summary>
        /// Get count of two word wordsets
        /// </summary>
        /// <param name="word1"></param>
        /// <param name="word2"></param>
        /// <returns></returns>
        int GetCountDouble(string word1, string word2);


       /// <summary>
       /// Get the count of three word wordsets
       /// </summary>
       /// <param name="word1"></param>
       /// <param name="word2"></param>
       /// <param name="word3"></param>
       /// <returns></returns>
        int GetCountTriple(string word1, string word2, string word3);

        /// <summary>
        /// Get the total count of transactions in the database (The number of tweets)
        /// </summary>
        /// <returns></returns>
        int GetTotal();

        /// <summary>
        /// Get all of the two word item sets that contain the words passed in
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        List<IOrderedEnumerable<string>> GetAll2ItemSets(IList<string> words);
    }
}