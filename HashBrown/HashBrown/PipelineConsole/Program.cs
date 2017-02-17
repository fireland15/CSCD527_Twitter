﻿using Npgsql;
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
            Console.WriteLine("type 'quit' when you are finished to close the connection");
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