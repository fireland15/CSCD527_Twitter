using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashtagGenerator
{
    public class AprioriRepository : IAprioriRepository
    {
        public AprioriRepository(NpgsqlConnection con)
        {

        }

        public int GetCountDouble(string word1, string word2)
        {
            throw new NotImplementedException();
        }

        public int GetCountSingle(string word)
        {
            throw new NotImplementedException();
        }

        public int GetCountTriple(string word1, string word2, string word3)
        {
            throw new NotImplementedException();
        }

        public int GetTotal()
        {
            throw new NotImplementedException();
        }
    }
}
