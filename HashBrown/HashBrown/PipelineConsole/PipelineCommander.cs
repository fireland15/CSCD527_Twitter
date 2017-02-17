using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace PipelineConsole
{
    public class PipelineCommander
    {
        private readonly NpgsqlConnection _connection;

        public PipelineCommander(NpgsqlConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            _connection = connection;

            if (_connection.FullState != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        public void Run()
        {
            while (true)
            {
                Console.Write(">");
                string command = Console.ReadLine();

                if (command.Equals("quit", StringComparison.CurrentCultureIgnoreCase))
                {
                    break;
                }
                else if (command.Equals("query", StringComparison.CurrentCultureIgnoreCase))
                {
                    string sql = Console.ReadLine();
                    using (NpgsqlCommand sqlCommand = new NpgsqlCommand(sql, _connection))
                    {

                        try
                        {
                            using (NpgsqlDataReader rdr = sqlCommand.ExecuteReader())
                            {

                                while (rdr.Read())
                                {
                                    for (int i = 0; i < rdr.FieldCount; i++)
                                    {
                                        Console.WriteLine(rdr[i]);
                                    }
                                }
                            }
                        }
                        catch (NpgsqlException ex)
                        {
                            Console.WriteLine($"Exception Encountered: {ex.Message}");
                        }
                    }
                }
                else if (command.Equals("exec", StringComparison.CurrentCultureIgnoreCase))
                {
                    string sql = Console.ReadLine();
                    using (NpgsqlCommand sqlCommand = new NpgsqlCommand(sql, _connection))
                    {
                        try
                        {
                            sqlCommand.ExecuteNonQuery();
                        }
                        catch (NpgsqlException ex)
                        {
                            Console.WriteLine($"Exception Encountered: {ex.Message}");
                        }
                    }
                }
            }

            Console.WriteLine("Quitting");
        }
    }
}
