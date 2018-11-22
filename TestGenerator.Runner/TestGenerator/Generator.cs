using System.Collections.Generic;
using System.IO;
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
            var codeParser = new CodeParser();

            var reader = new TransformBlock<string, Task<string>>(inputData => ReadFileAsync(inputData),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = readingThreadAmout });

            var generator = new TransformBlock<Task<string>, Task<List<GeneratedTestResult>>>(
                inputData => codeParser.GenerateTestsAsync(inputData, outputDirectory),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxProcessingThreadAmout });

            var writer = new ActionBlock<Task<List<GeneratedTestResult>>>((generatedClass => WriteFileAsync(generatedClass)),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = writingThreadAmout });

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            reader.LinkTo(generator, linkOptions);
            generator.LinkTo(writer, linkOptions);

            foreach (var path in paths)
            {
                reader.Post(path);
            }
                
            reader.Complete();

            return writer.Completion;
        }

        private async Task<string> ReadFileAsync(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                return await sr.ReadToEndAsync();
            }
        }

        private async void WriteFileAsync(Task<List<GeneratedTestResult>> generateResult)
        {
            var results = await generateResult;
            foreach (var result in results)
            {
                using (StreamWriter sw = new StreamWriter(result.OutputPath))
                {
                    await sw.WriteAsync(result.Result);
                }
            }
        }
    }
}
