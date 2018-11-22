using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TestGenerator
{
    public class Generator
    {
        string outputDirectory;
        int readingThreadAmout;
        int writingThreadAmout;
        int maxProcessingThreadAmout;

        Generator(string outputDirectory, int readingThreadAmout, int writingThreadAmout, int maxProcessingThreadAmout)
        {
            this.outputDirectory = outputDirectory;
            this.readingThreadAmout = readingThreadAmout;
            this.writingThreadAmout = writingThreadAmout;
            this.maxProcessingThreadAmout = maxProcessingThreadAmout;
        }

        public Task Generate(List<string> paths)
        {
            var reader = new TransformBlock<string, Task<string>>(inputData => ReadFileAsync(inputData),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = readingThreadAmout });
            return null;
        }

        private async Task<string> ReadFileAsync(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                return await sr.ReadToEndAsync();
            }
        }

    }
}
