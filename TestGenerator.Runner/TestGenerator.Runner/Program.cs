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

                inputPaths.Add(filePathTest1);
                inputPaths.Add(filePathTest2);
                inputPaths.Add(filePathTest3);

                generator.Generate(inputPaths).Wait();
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

                readingThreadAmout = getThreadAmount("Write amount of reading threads");
                writingThreadAmout = getThreadAmount("Write amount of writing threads");
                maxProcessingThreadAmout = getThreadAmount("Write max amount of processing threads");

                Generator generator = new Generator(outputDirectory, readingThreadAmout, writingThreadAmout, maxProcessingThreadAmout);

                generator.Generate(inputPaths).Wait();
            }

            Console.WriteLine("Done");

            Console.ReadKey();
        }

        private static int getThreadAmount(string message)
        {
            int res = 0;

            do
            {
                Console.WriteLine(message);

            } while (!int.TryParse(Console.ReadLine(), out res));

            return res;
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
