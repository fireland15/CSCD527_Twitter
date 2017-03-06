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

        //An itemset should have a count greater than or equal to the frequencyThreshold if returned
        public List<IOrderedEnumerable<string>> GetAll2ItemSets(IList<string> words, int frequencyThreshold, int maxResults)
        {
            List<IOrderedEnumerable<string>> twoItemSets = new List<IOrderedEnumerable<string>>();

            string sql = BuildTwoItemSetQuery(words.Count);

            Console.WriteLine(sql);

            using (NpgsqlTransaction transaction = _connection.BeginTransaction())
            {
                using (NpgsqlCommand command = _connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;

                    for (int i = 1; i <= words.Count; i++)
                    {
                        command.Parameters.Add(new NpgsqlParameter($"@word{i}", NpgsqlTypes.NpgsqlDbType.Varchar));
                    }

                    command.Parameters.Add(new NpgsqlParameter("@frequency", NpgsqlTypes.NpgsqlDbType.Bigint));
                    command.Parameters.Add(new NpgsqlParameter("@maxResults", NpgsqlTypes.NpgsqlDbType.Integer));

                    for (int i = 0; i < words.Count; i++)
                    {
                        command.Parameters[i].Value = words[i];
                    }

                    command.Parameters[words.Count].Value = frequencyThreshold;
                    command.Parameters[words.Count + 1].Value = maxResults;

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
            }

            return twoItemSets;
        }

        public IEnumerable<Tuple<string, string, string, int>> GetCountTripleMany(IEnumerable<IOrderedEnumerable<string>> threeItemSets)
        {
            int count = threeItemSets.Count();

            string sql = BuildTripleQuery(threeItemSets.Count());

            Console.WriteLine(sql);

            List<Tuple<string, string, string, int>> results = new List<Tuple<string, string, string, int>>();

            using (NpgsqlTransaction transaction = _connection.BeginTransaction())
            {
                using (NpgsqlCommand command = _connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;

                    for (int i = 0; i < count; i++)
                    {
                        command.Parameters.Add(new NpgsqlParameter($"@word{i}_1", NpgsqlTypes.NpgsqlDbType.Varchar));
                        command.Parameters.Add(new NpgsqlParameter($"@word{i}_2", NpgsqlTypes.NpgsqlDbType.Varchar));
                        command.Parameters.Add(new NpgsqlParameter($"@word{i}_3", NpgsqlTypes.NpgsqlDbType.Varchar));
                    }

                    int j = 0;
                    foreach (var itemSet in threeItemSets)
                    {
                        var list = itemSet.ToList();
                        command.Parameters[j++].Value = list[0];
                        command.Parameters[j++].Value = list[1];
                        command.Parameters[j++].Value = list[2];
                    }

                    using (NpgsqlDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string word1 = rdr[0].ToString();
                            string word2 = rdr[1].ToString();
                            string word3 = rdr[2].ToString();
                            int tupleCount = Int32.Parse(rdr[3].ToString());

                            var tuple = new Tuple<string, string, string, int>(word1, word2, word3, tupleCount);
                            results.Add(tuple);
                        }
                    }
                }
            }

            return results;
        }

        string BuildTripleQuery(int count)
        {
            StringBuilder b = new StringBuilder();

            b.Append("SELECT word1, word2, word3, count FROM word_set_3_1_day WHERE ");

            for (int i = 0; i < count - 1; i++)
            {
                b.Append($"(word1 = @word{i}_1 AND word2 = @word{i}_2 AND word3 = @word{i}_3) OR ");
            }

            b.Append($"(word1 = @word{count - 1}_1 AND word2 = @word{count - 1}_2 AND word3 = @word{count - 1}_3);");

            return b.ToString();
        }

        public int GetCountDouble(string word1, string word2)
        {
            int wordCount = 0;

            string sql = "SELECT count FROM word_set_2_1_day WHERE word1 = @word1 OR word2 = @word2 OR word1 = @word2 OR word2 = @word1;";

            using (NpgsqlTransaction transaction = _connection.BeginTransaction())
            {
                using (NpgsqlCommand command = _connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;

                    command.Parameters.Add(new NpgsqlParameter("@word1", NpgsqlTypes.NpgsqlDbType.Varchar));
                    command.Parameters.Add(new NpgsqlParameter("@word2", NpgsqlTypes.NpgsqlDbType.Varchar));

                    command.Parameters[0].Value = word1;
                    command.Parameters[1].Value = word2;

                    using (NpgsqlDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            wordCount = Int32.Parse(rdr[0].ToString());
                        }
                    }
                }
            }

            return wordCount;
        }

        public int GetCountSingle(string word)
        {
            int wordCount = 0;

            string sql = "SELECT count FROM word_set_1_1_day WHERE word1 = @word1;";

            using (NpgsqlTransaction transaction = _connection.BeginTransaction())
            {
                using (NpgsqlCommand command = _connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;

                    command.Parameters.Add(new NpgsqlParameter("@word1", NpgsqlTypes.NpgsqlDbType.Varchar));

                    command.Parameters[0].Value = word;

                    using (NpgsqlDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            wordCount = Int32.Parse(rdr[0].ToString());
                        }
                    }
                }
            }

            return wordCount;
        }

        public int GetCountTriple(string word1, string word2, string word3)
        {
            var wordList = new List<string>
            {
                word1,
                word2,
                word3
            }.OrderBy(word => word).ToList();
            

            int wordCount = 0;

            string sql = "SELECT count FROM word_set_3_1_day WHERE word1 = @word1 AND word2 = @word2 AND word3 = @word3;";

            using (NpgsqlTransaction transaction = _connection.BeginTransaction())
            {
                using (NpgsqlCommand command = _connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;

                    command.Parameters.Add(new NpgsqlParameter("@word1", NpgsqlTypes.NpgsqlDbType.Varchar));
                    command.Parameters.Add(new NpgsqlParameter("@word2", NpgsqlTypes.NpgsqlDbType.Varchar));
                    command.Parameters.Add(new NpgsqlParameter("@word3", NpgsqlTypes.NpgsqlDbType.Varchar));

                    command.Parameters[0].Value = wordList[0];
                    command.Parameters[1].Value = wordList[1];
                    command.Parameters[2].Value = wordList[2];

                    using (NpgsqlDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            wordCount = Int32.Parse(rdr[0].ToString());
                        }
                    }
                }
            }

            return wordCount;
        }

        public int GetTotal()
        {
            int tweetCount = 0;

            string sql = "SELECT * FROM tweet_count_1_day;";

            using (NpgsqlTransaction transaction = _connection.BeginTransaction())
            {
                using (NpgsqlCommand command = _connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;

                    using (NpgsqlDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            tweetCount = Int32.Parse(rdr[0].ToString());
                        }
                    }
                }
            }

            return tweetCount;
        }

        private string BuildTwoItemSetQuery(int wordCount)
        {
            StringBuilder paramBuilder = new StringBuilder();
            paramBuilder.Append("(");
            for (int i = 1; i < wordCount; i++)
            {
                paramBuilder.Append($"@word{i}, ");
            }
            paramBuilder.Append($"@word{wordCount})");
            string paramList = paramBuilder.ToString();

            return $"SELECT word1, word2 FROM word_set_2_1_day WHERE word1 IN {paramList} OR word{2} IN {paramList} AND count > @frequency ORDER BY count limit @maxResults;";
        }
    }
}
