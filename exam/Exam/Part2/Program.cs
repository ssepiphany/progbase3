using System;
using System.IO;

namespace Part2
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("PLease, input command: ");
                string[] parts = Console.ReadLine().Trim().Split(' ');

                if (parts[0] == "gen_db")
                {
                    ProcessGen(parts);
                }
                else if (parts[0] == "delete_year")
                {
                    ProcessDelete(parts);
                }
                else if (parts[0] == "merge_csv")
                {
                    ProcessMerge(parts);
                }
                else
                {
                    Console.Error.WriteLine("Invalid command");
                    return; 
                }
            }
        }

        static void ProcessMerge(string[] parts)
        {
            throw new NotImplementedException();
        }

        static void ProcessDelete(string[] parts)
        {
            throw new NotImplementedException();
        }

        static void ProcessGen(string[] parts)
        {
            if (parts.Length != 5)
            {
                Console.Error.WriteLine("Invalid command length");
                return;
            }
            string filePath = parts[1];

            if (!File.Exists(filePath))
            {
                Console.Error.WriteLine($"File {filePath} does not exist");
                return;
            }

            int n = 0;
            int m = 0;

            if(!int.TryParse(parts[3], out n) || !int.TryParse(parts[4], out m))
            {
                Console.WriteLine("Invalid numbers");
            } 

            
        }
    }
}
