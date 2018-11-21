using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGenerator.Runner
{
    class Program
    {

        static void Main(string[] args)
        {
            List<string> inputPaths = new List<string>();
            string outputDirectory = null;
            string filePath = null;
            int readingThreadAmout = 0;
            int writingThreadAmout = 0;
            int maxProcessingThreadAmout = 0;

            do
            {
                Console.WriteLine("Write path to input file(.cs) (To complete the entry press Enter)");

                filePath = Console.ReadLine();

                if (File.Exists(filePath))
                {
                    inputPaths.Add(filePath);
                }
                else if (filePath != "")
                {
                    Console.WriteLine("File not found: " + filePath);
                }

            } while (filePath != "");

            Console.WriteLine("Write path to output directory");

            outputDirectory = Console.ReadLine();

            if (!Directory.Exists(outputDirectory))
            {
                Console.WriteLine("Output directory not found: " + outputDirectory);
                outputDirectory = Directory.GetCurrentDirectory();
            }

            do
            {
                Console.WriteLine("Write amount of reading threads");

            } while (!Int32.TryParse(Console.ReadLine(), out readingThreadAmout));

            do
            {
                Console.WriteLine("Write amount of writing threads");

            } while (!Int32.TryParse(Console.ReadLine(), out writingThreadAmout));

            do
            {
                Console.WriteLine("Write max amount of processing threads");

            } while (!Int32.TryParse(Console.ReadLine(), out maxProcessingThreadAmout));

            Console.ReadKey();
        }
    }
}
