using System;
using System.Collections.Generic;
using System.IO;

namespace Part1
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Invalid command");
                return; 
            }
            if (args[0] == "gen_num")
            {
                ProcessGenerate(args);
            }
            else if (args[0] == "num")
            {
                ProcessNum(args);
            }
            else if (args[0] == "num_uni")
            {
                ProcessNumUni(args);
            }
            else 
            {
                Console.WriteLine("Invalid command");
                return; 
            }
        }

        static void ProcessGenerate(string[] args)
        {
            if (args.Length != 5)
            {
                Console.WriteLine("Invalid command length");
                return;
            }
            string filePath = args[1];

            int a = 0;
            int b = 0;
            int n = 0;

            if(!int.TryParse(args[2], out a) || !int.TryParse(args[3], out b) || !int.TryParse(args[4], out n))
            {
                Console.WriteLine("Invalid numbers");
            } 

            Random random = new Random();
            int num;

            for (int i = 0; i < n; i++)
            {
                num = random.Next(a, b+1); 

                StreamWriter writer = File.AppendText(filePath) ; 
                writer.Write(num) ;
                if ( i != n - 1)
                {
                    writer.WriteLine() ; 
                }
                writer.Close();
            }
        }

        static void ProcessNum(string[] args)
        {
            if ( args.Length != 2)
            {
                Console.WriteLine("Invalid command length");
                return;
            }

            string filePath = args[1];
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File path {filePath} does not exist");
                return;
            }
            
            StreamReader reader = new StreamReader(filePath);
            HashSet<int> set = new HashSet<int>();
            string line = "";

            while (true)
            {
                line = reader.ReadLine();

                if (line == null)
                {
                    break; 
                }
                int num = int.Parse(line);

                if ( num % 2 == 0)
                {
                    set.Add(num);
                }
            }
            Console.WriteLine($"Number of even unique numbers: {set.Count}");
        }

        static void ProcessNumUni(string[] args)
        {
            if ( args.Length != 3)
            {
                Console.WriteLine("Invalid command");
                return;
            }

            string inputPath = args[1];
            string outputPath= args[2];
            if (!File.Exists(inputPath))
            {
                Console.WriteLine($"File path {inputPath} does not exist");
                return;
            }
            
            StreamReader reader = new StreamReader(inputPath);
            HashSet<int> set = new HashSet<int>();
            string line = "";

            while (true)
            {
                line = reader.ReadLine();

                if (line == null)
                {
                    break; 
                }
                int num = int.Parse(line);

                if (set.Add(num))
                {   
                    StreamWriter writer = File.AppendText(outputPath) ; 
                    if (set.Count > 1)
                    {
                        writer.WriteLine();
                    }
                    writer.Write(num);
                    writer.Close();
                }
            }
        }
    }
}
