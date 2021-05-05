using System;
using Microsoft.Data.Sqlite;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string dbPath = "C:/Users/Sofia/projects/progbase3/data/database.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            Console.WriteLine("Please, enter your command:");
            string[] command = Console.ReadLine().Trim().Split();
            if(command[0] == "generate")
            {
                // generate {movie} {5}
                UserInterface.ProcessGenerate(command, connection);
            }
            connection.Close();
        }
    }
}
