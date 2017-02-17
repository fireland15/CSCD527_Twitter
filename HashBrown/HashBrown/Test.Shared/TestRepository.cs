using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Shared
{
    [TestClass]
    public class TestRepository
    {
        [TestMethod]
        public void TestSingleInsert()
        {
            string conStr = "Server=www.forrestireland.com;Port=5432;User Id=CharlieBrown;Password=AcademicCoffeeBean59943;Database=pipeline;";

            using (NpgsqlConnection con = new NpgsqlConnection(conStr))
            {
                IPipelineRepository repo = new PipelineRepository(con);
                WordHashtagPair pair = new WordHashtagPair
                {
                    Word = "banana",
                    HashTag = "yellow"
                };

                repo.Insert(pair);
            }

            // Manual verification that insert appeared in database;
            // Made by Forrest
        }

        [TestMethod]
        public void TestInsertMany()
        {
            string conStr = "Server=www.forrestireland.com;Port=5432;User Id=CharlieBrown;Password=AcademicCoffeeBean59943;Database=pipeline;";

            using (NpgsqlConnection con = new NpgsqlConnection(conStr))
            {
                IPipelineRepository repo = new PipelineRepository(con);
                IEnumerable<WordHashtagPair> pairs = new List<WordHashtagPair>
                {
                    new WordHashtagPair
                    {
                        Word = "banana",
                        HashTag = "yellow"
                    },

                    new WordHashtagPair
                    {
                        Word = "banana",
                        HashTag = "curvy"
                    },

                    new WordHashtagPair
                    {
                        Word = "banana",
                        HashTag = "yellow"
                    }
                };

                repo.InsertMany(pairs);
            }
        }
    }
}
