using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace Shared
{
    public class PipelineRepository : IPipelineRepository
    {
        private readonly NpgsqlConnection _connection;

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
            string sql = "INSERT INTO twitter_stream (word, hashtag) VALUES (@word, @hashtag);";
                        
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
            string sql = "INSERT INTO twitter_stream (word, hashtag) VALUES (@word, @hashtag);";

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
    }
}
