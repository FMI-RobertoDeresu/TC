﻿using System;
using System.IO;
using System.Linq;

namespace TC
{
    public class Program
    {
        private static readonly StreamWriter ErrorsWriter =
            new StreamWriter(new FileStream("Files/Errors.txt", FileMode.Append));

        public static void Main(string[] args)
        {
            try
            {
                var config = File.ReadAllLines("Files/AppSettings.txt");
                var algorithm = config.Single(c => c.StartsWith("Algorithm=")).Split('=')[1];

                switch (algorithm)
                {
                    case "PDA":
                        RunPDA();
                        break;
                    case "EarleyParser":
                        RunEarleyParser();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                ErrorsWriter.WriteLine();
                ErrorsWriter.WriteLine($"{DateTime.Now:yyyy.mm.dd} - {exception.Message}");
                ErrorsWriter.Flush();
            }
        }

        private static void RunPDA()
        {
            var writer = new StreamWriter(new FileStream("Files/PDA/Output.txt", FileMode.Truncate));
            var pda = FileParser.ParsePDAConfigFile("Files/PDA/Config.txt");

            foreach (var input in File.ReadAllLines("Files/PDA/Input.txt"))
            {
                writer.WriteLine($"{input}:");
                pda.Compute(input).ToList().ForEach(writer.WriteLine);
                writer.WriteLine();
            }

            writer.Flush();
        }

        private static void RunEarleyParser()
        {
            var writer = new StreamWriter(new FileStream("Files/EarleyParser/Output.txt", FileMode.Truncate));
            var earleyParser = FileParser.ParseEarleyParserConfigFile("Files/EarleyParser/Config.txt");

            foreach (var input in File.ReadAllLines("Files/EarleyParser/Input.txt"))
            {
                writer.WriteLine($"{input}:");
                earleyParser.Compute(input).ToList().ForEach(writer.WriteLine);
                writer.WriteLine();
            }

            writer.Flush();
        }
    }
}