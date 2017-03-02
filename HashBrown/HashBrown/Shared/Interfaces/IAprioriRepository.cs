using System.Linq;

namespace Shared.Interfaces
{
    public interface IAprioriRepository
    {
        /// <summary>
        /// Get the count of transactions that contain the provided word set
        /// </summary>
        /// <param name="wordset"></param>
        /// <returns></returns>
        int GetCount(IOrderedEnumerable<string> wordset);

        /// <summary>
        /// Get the total count of transactions in the database (The number of tweets)
        /// </summary>
        /// <returns></returns>
        int GetTotal();
    }
}