using Npgsql;
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
        private readonly NpgsqlConnection _connection;

        public AprioriRepository(NpgsqlConnection connection)
        {
            _connection = connection;

            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        public List<IOrderedEnumerable<string>> GetAll2ItemSets(IList<string> words)
        {
            List<IOrderedEnumerable<string>> twoItemSets = new List<IOrderedEnumerable<string>>();

            string sql = string.Empty;

            // Build some sql

            using(NpgsqlCommand command = new NpgsqlCommand(sql, _connection))
            {
                using (NpgsqlDataReader rdr = command.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        List<string> twoItemSet = new List<string>();

                        for (int i = 0; i < rdr.FieldCount; i++)
                        {
                            twoItemSet.Add((string)rdr[i]);
                        }

                        twoItemSets.Add(twoItemSet.OrderBy(word => word));
                    }
                }
            }

            return twoItemSets;
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
