using Npgsql;
using System;
using System.Configuration;

namespace PipelineConsole
{
    class Program
    {
        static void PrintOpeningMessage(string conStr)
        {
            Console.WriteLine($"Connecting using the connection string: \n{conStr}");
            Console.WriteLine();
            Console.WriteLine("Enter sql commands for pipelinedb");
            Console.WriteLine();
            Console.WriteLine("COMMANDS");
            Console.WriteLine("\texec  - Allows you to enter a sql statement that doesn't return stuff");
            Console.WriteLine("\tquery - Allows you to query for results");
            Console.WriteLine("\tquit  - Closes the connection and terminates program");
            Console.WriteLine("=========================================================");
            Console.WriteLine("EXAMPLE\n");
            Console.WriteLine(">exec");
            Console.WriteLine("CREATE STREAM stream_name (x integer, y integer, z integer);");
            Console.WriteLine(">query");
            Console.WriteLine("SELECT * FROM continuous_view_name WHERE x = 1;");
            Console.WriteLine("=========================================================");

        }

        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();

            PrintOpeningMessage(connectionString);

            using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
            {
                Console.WriteLine("Connecting...\n");

                con.Open();

                Console.WriteLine("Connection ready.\n");
                
                PipelineCommander commander = new PipelineCommander(con);

                commander.Run();
            }


        }
    }
}
