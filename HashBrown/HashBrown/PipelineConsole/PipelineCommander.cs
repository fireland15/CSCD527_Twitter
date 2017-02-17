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

                NpgsqlCommand sqlCommand = new NpgsqlCommand(command, _connection);

                try
                {
                    NpgsqlDataReader rdr = sqlCommand.ExecuteReader();

                    while (rdr.Read())
                    {
                        Console.WriteLine(rdr.ToString());
                    }
                }
                catch (NpgsqlException ex)
                {
                    Console.WriteLine($"Exception Encountered: {ex.Message}");
                }
            }

            Console.WriteLine("Quitting");
        }
    }
}
