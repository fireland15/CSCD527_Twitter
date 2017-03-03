using System.Collections.Generic;

namespace Shared
{
    public interface IPipelineRepository
    {
        void Insert(WordHashtagPair pair);
        void InsertMany(IEnumerable<WordHashtagPair> pairs);

        void InsertNTuple(string[] tupleOfWords);

        void AddTweet();
    }
}
