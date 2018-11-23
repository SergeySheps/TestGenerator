using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestGenerator.Tests
{
    [TestClass]
    public class GeneratorTest
    {
        private string outputDirectory = "../../Output";
        private Generator generator;
        private List<string> paths;
        private CompilationUnitSyntax compilationUnit;

        [TestInitialize]
        public void Initialize()
        {
            generator = new Generator(outputDirectory, 2, 2, 2);

            paths = new List<string>();

            paths.Add("../../../../TestClasses/TwoClasses.cs");

            generator.Generate(paths).Wait();
            
            string testedFile = generator.ReadFileAsync(Path.Combine(outputDirectory, "TraceResultTest.cs")).Result;

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(testedFile);

            compilationUnit = syntaxTree.GetCompilationUnitRoot();
        }

        [TestMethod]
        public void TestUsing()
        {
            var expected = "Microsoft.VisualStudio.TestTools.UnitTesting";
            var actual = compilationUnit.DescendantNodes().OfType<UsingDirectiveSyntax>().First().Name.ToFullString();
                
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestNameSpace()
        {
          
            var expected = "ClassLibrary1lab_spp.Test";
            var actual = (compilationUnit.DescendantNodes().OfType<ClassDeclarationSyntax>().First().Parent as NamespaceDeclarationSyntax)?.Name.ToString(); ;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestClassName()
        {

            var expected = "TraceResultTest";
            var actual = compilationUnit.DescendantNodes().OfType<ClassDeclarationSyntax>().First().Identifier.ValueText;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMethodAttributes()
        {
            var expected = "[TestMethod]";
            var actual = compilationUnit.DescendantNodes().OfType<ClassDeclarationSyntax>().First().
                DescendantNodes().OfType<MethodDeclarationSyntax>().First().AttributeLists.First().ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestClassAttribute()
        {
            var expected = "[TestClass]";
            var actual = compilationUnit.DescendantNodes().OfType<ClassDeclarationSyntax>().First().AttributeLists.First().ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestCleanup]
        public void CleanUp()
        {
           Thread.Sleep(50);
        }

    }
}
