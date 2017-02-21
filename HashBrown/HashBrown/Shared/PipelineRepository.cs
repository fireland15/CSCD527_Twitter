using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Shared
{
    public class PipelineRepository : IPipelineRepository
    {
        private readonly NpgsqlConnection _connection;

        private static readonly string _streamName = "twitter_stream";

        public PipelineRepository(NpgsqlConnection connection)
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
            string sql = $"INSERT INTO {_streamName} (word, hashtag) VALUES (@word, @hashtag);";
                        
            using (NpgsqlCommand command = new NpgsqlCommand(sql, _connection))
            {
                var transaction = _connection.BeginTransaction();
                command.Transaction = transaction;

                command.Parameters.Add("@word", NpgsqlTypes.NpgsqlDbType.Varchar).Value = pair.Word;
                command.Parameters.Add("@hashtag", NpgsqlTypes.NpgsqlDbType.Varchar).Value = pair.HashTag;
                command.ExecuteNonQuery();

                try
                {
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
        }

        public void InsertMany(IEnumerable<WordHashtagPair> pairs)
        {
            string sql = $"INSERT INTO {_streamName} (word, hashtag) VALUES (@word, @hashtag);";

            using (NpgsqlTransaction transaction = _connection.BeginTransaction())
            {
                using (NpgsqlCommand command = _connection.CreateCommand())
                {
                    command.Transaction = transaction;
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
                            if (command.ExecuteNonQuery() != 1)
                            {
                                throw new InvalidProgramException();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (InvalidProgramException)
                    {
                        /* Handle as we see fit */
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        public void InsertNTuple(string[] tupleOfWords)
        {
            uint length = (uint)tupleOfWords.Length;

            string sql = GetInsertIntoSQL(tupleOfWords);

            using (NpgsqlTransaction transaction = _connection.BeginTransaction())
            {
                using (NpgsqlCommand command = _connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;

                    try
                    {
                        if (command.ExecuteNonQuery() != 1)
                        {
                            throw new InvalidProgramException();
                        }

                        transaction.Commit();
                    }
                    catch (InvalidProgramException)
                    {
                        Console.WriteLine($"Failed to insert");
                        Console.WriteLine(sql);
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        private string GetInsertIntoSQL(string[] words)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"INSERT INTO {GetNTupleStreamName((uint)words.Length)} (");

            for (uint i = 1; i < words.Length; i++)
            {
                sb.Append($"word{i}, "); // add column to stream for each word in tuple.
            }
            sb.Append($"word{words.Length}) VALUES (");

            for (uint i = 0; i < words.Length - 1; i++)
            {
                sb.Append($"'{words[i]}', ");
            }
            sb.Append($"'{words[words.Length - 1]}');");

            return sb.ToString();
        }

        private string GetNTupleStreamName(uint n)
        {
            return $"{_streamName}_{n}";
        }
    }
}
