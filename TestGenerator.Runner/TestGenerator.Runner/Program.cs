using System;
using System.Collections.Generic;
using System.IO;

namespace TestGenerator.Runner
{
    class Program
    {

        static void Main(string[] args)
        {
            List<string> inputPaths = new List<string>();
            string outputDirectory = null;
            int readingThreadAmout = 0;
            int writingThreadAmout = 0;
            int maxProcessingThreadAmout = 0;

            //Test Data
            string filePathTest1 = @"D:\study\3_course\My\严蟎4lab_spp\TestClasses\TwoClasses.cs";
            string filePathTest2 = @"D:\study\3_course\My\严蟎4lab_spp\TestClasses\Method.cs";
            string filePathTest3 = @"D:\study\3_course\My\严蟎4lab_spp\TestClasses\Faker.cs";
            string outputDirectoryTest = @"D:\study\3_course\My\严蟎4lab_spp\Output";
            int readingThreadAmoutTest = 2;
            int writingThreadAmoutTest = 2;
            int maxProcessingThreadAmoutTest = 2;
            var isTestMode = true;
            //

            if (isTestMode)
            {
                Generator generator = new Generator(outputDirectoryTest, readingThreadAmoutTest, writingThreadAmoutTest, maxProcessingThreadAmoutTest);
                var pathList = new List<string>();
                pathList.Add(filePathTest1);
                pathList.Add(filePathTest2);
                pathList.Add(filePathTest3);

                generator.Generate(pathList).Wait();

                Console.WriteLine("Done");
                Console.ReadKey();
            }
            else
            {

                initializeInputPaths(inputPaths);

                if (inputPaths.Count == 0)
                {
                    Console.WriteLine("Not found correct path to c# class file.");
                    initializeInputPaths(inputPaths);
                }

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

        private static void initializeInputPaths(List<string> inputPaths)
        {
            string filePath = null;

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
        }
    }
}
