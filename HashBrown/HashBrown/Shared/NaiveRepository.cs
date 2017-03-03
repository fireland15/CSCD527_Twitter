using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Naive;
using Npgsql;

namespace NaiveHashtag
{
    public class NaiveRepository : INaiveRepository
    {
        private readonly NpgsqlConnection _connection;

        public NaiveRepository(NpgsqlConnection connection)
        {
            _connection = connection;

            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        public NaiveList GetNaiveList(IEnumerable<string> words)
        {
            NaiveList list = new NaiveList();

            string sql = "SELECT hashtag, count FROM word_hashtag_pairs_1_day WHERE word = @word;";

            using (NpgsqlTransaction transaction = _connection.BeginTransaction())
            {
                using (NpgsqlCommand command = _connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;

                    command.Parameters.Add(new NpgsqlParameter("@word", NpgsqlTypes.NpgsqlDbType.Varchar));

                    foreach (var word in words)
                    {
                        NaiveWord naiveWord = new NaiveWord
                        {
                            Word = word,
                            Hashtags = new List<HashtagAndCount>()
                        };

                        command.Parameters[0].Value = word;

                        using (NpgsqlDataReader rdr = command.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                naiveWord.Hashtags.Add(new HashtagAndCount
                                {
                                    Hashtag = rdr[0].ToString(),
                                    Count = Int32.Parse(rdr[1].ToString())
                                });
                            }
                        }

                        list.Words.Add(naiveWord);
                    }
                }
            }

            return list;
        }
    }
}
