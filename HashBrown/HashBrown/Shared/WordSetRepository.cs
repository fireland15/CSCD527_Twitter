using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class PostGreSqlRepository : IPipelineRepository
    {
        private readonly NpgsqlConnection _connection;

        private static readonly string _wordSetBaseName = "word_set";

        public PostGreSqlRepository(NpgsqlConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            _connection = connection;

            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        public void Insert(WordHashtagPair pair)
        {
            string sql = $"INSERT INTO word_hashtag_pairs (word, hashtag) VALUES (@word, @hashtag);";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, _connection))
            {
                //var transaction = _connection.BeginTransaction();
                //command.Transaction = transaction;

                command.Parameters.Add("@word", NpgsqlTypes.NpgsqlDbType.Varchar).Value = pair.Word;
                command.Parameters.Add("@hashtag", NpgsqlTypes.NpgsqlDbType.Varchar).Value = pair.HashTag;
                command.ExecuteNonQueryAsync();

                try
                {
                    //transaction.Commit();
                }
                catch (Exception)
                {
                    //transaction.Rollback();
                }
            }
        }

        public void InsertMany(IEnumerable<WordHashtagPair> pairs)
        {
            string sql = $"INSERT INTO word_hashtag_pairs (word, hashtag) VALUES (@word, @hashtag);";

            //using (NpgsqlTransaction transaction = _connection.BeginTransaction())
            //{
                using (NpgsqlCommand command = _connection.CreateCommand())
                {
                    //command.Transaction = transaction;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    command.Parameters.Add(new NpgsqlParameter("@word", NpgsqlTypes.NpgsqlDbType.Varchar));
                    command.Parameters.Add(new NpgsqlParameter("@hashtag", NpgsqlTypes.NpgsqlDbType.Varchar));

                    try
                    {
                        foreach (var pair in pairs)
                        {
                            command.Parameters[0].Value = pair.Word;
                            command.Parameters[1].Value = pair.HashTag;
                            command.ExecuteNonQueryAsync();
                        }

                        //transaction.Commit();
                    }
                    catch (InvalidProgramException)
                    {
                        /* Handle as we see fit */
                    }
                    catch (Exception)
                    {
                        //transaction.Rollback();
                    }
                }
            //}
        }

        public void InsertNTuple(string[] tupleOfWords)
        {
            uint length = (uint)tupleOfWords.Length;

            string sql = GetInsertIntoSQL(length);

            //using (NpgsqlTransaction transaction = _connection.BeginTransaction())
            //{
                using (NpgsqlCommand command = _connection.CreateCommand())
                {
                    //command.Transaction = transaction;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;

                    for (int i = 1; i <= length; i++)
                    {
                        command.Parameters.Add(new NpgsqlParameter($"@word{i}", NpgsqlTypes.NpgsqlDbType.Varchar));
                    }
                    for (int i = 0; i < length; i++)
                    {
                        command.Parameters[i].Value = tupleOfWords[i];
                    }

                    try
                    {

                        command.ExecuteNonQueryAsync();

                        //transaction.Commit();
                    }
                    catch (InvalidProgramException)
                    {
                        Console.WriteLine($"Failed to insert");
                        Console.WriteLine(sql);
                    }
                    catch (Exception ex)
                    {
                        //transaction.Rollback();
                        throw ex;
                    }
                }
            //}
        }

        private string GetInsertIntoSQL(uint n)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"INSERT INTO {GetNTupleStreamName(n)} (");

            for (uint i = 1; i < n; i++)
            {
                sb.Append($"word{i}, "); // add column to stream for each word in tuple.
            }
            sb.Append($"word{n}) VALUES (");

            for (uint i = 0; i < n - 1; i++)
            {
                sb.Append($"@word{i + 1}, ");
            }
            sb.Append($"@word{n});");

            return sb.ToString();
        }

        private string GetNTupleStreamName(uint n)
        {
            return $"{_wordSetBaseName}_{n}";
        }

        public void AddTweet()
        {
            throw new NotImplementedException();
        }
    }
}
