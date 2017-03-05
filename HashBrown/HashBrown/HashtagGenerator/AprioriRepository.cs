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
        public List<IOrderedEnumerable<string>> GetAll2ItemSets(IList<string> words, int frequencyThreshold)
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

                    for (int i = 0; i < words.Count; i++)
                    {
                        command.Parameters[i].Value = words[i];
                    }

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

            return $"SELECT word1, word2 FROM word_set_2_1_day WHERE word1 IN {paramList} OR word{2} IN {paramList};";
        }
    }
}
