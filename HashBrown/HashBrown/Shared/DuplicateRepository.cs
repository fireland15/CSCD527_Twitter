using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class DuplicateRepository : IPipelineRepository
    {
        private readonly IPipelineRepository _pipelineRepo;

        private readonly IPipelineRepository _postgreRepo;

        public DuplicateRepository(PipelineRepository pipelineRepo, PostGreSqlRepository postgreRepo)
        {
            _pipelineRepo = pipelineRepo;
            _postgreRepo = postgreRepo;
        }

        public void Insert(WordHashtagPair pair)
        {
            _pipelineRepo.Insert(pair);
            _postgreRepo.Insert(pair);
        }

        public void InsertMany(IEnumerable<WordHashtagPair> pairs)
        {
            _pipelineRepo.InsertMany(pairs);
            _postgreRepo.InsertMany(pairs);
        }

        public void InsertNTuple(string[] tupleOfWords)
        {
            _pipelineRepo.InsertNTuple(tupleOfWords);
            _postgreRepo.InsertNTuple(tupleOfWords);
        }
    }
}
