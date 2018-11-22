﻿using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace TestGenerator
{
    public class CodeParser
    {
        private async Task<List<GeneratedTestResult>> GenerateTestsAsync(Task<string> taskInputFile, string outputDirectory)
        {
            string source = await taskInputFile;
            var res = new List<GeneratedTestResult>();

            var syntaxTree = CSharpSyntaxTree.ParseText(source);
            var compilationUnitSyntax = syntaxTree.GetCompilationUnitRoot();

            var classes = compilationUnitSyntax.DescendantNodes().OfType<ClassDeclarationSyntax>();

            foreach (var classDeclaration in classes)
            {
                var publicMethods = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>()
                    .Where(x => x.Modifiers.Any(y => y.ValueText == "public"));

                var ns = (classDeclaration.Parent as NamespaceDeclarationSyntax)?.Name.ToString();
                var className = classDeclaration.Identifier.ValueText;
                var methodsName = new List<string>();
                foreach (var method in publicMethods)
                {
                    var name = GetMethodName(methodsName, method.Identifier.ToString(), 0);
                    methodsName.Add(name);
                }

                NamespaceDeclarationSyntax namespaceDeclarationSyntax = NamespaceDeclaration(QualifiedName(
                    IdentifierName(ns), IdentifierName("Test")));

                CompilationUnitSyntax compilationUnit = CompilationUnit()
                    .WithUsings(GetUsings())
                    .WithMembers(SingletonList<MemberDeclarationSyntax>(namespaceDeclarationSyntax
                        .WithMembers(SingletonList<MemberDeclarationSyntax>(ClassDeclaration(className + "Test")
                            .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("TestClass"))))))
                            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                            .WithMembers(GetMethodsSyntaxList(methodsName))))));



                var outputPath = Path.Combine(outputDirectory, className + "Test.cs");

                res.Add(new GeneratedTestResult
                {
                    Result = compilationUnit.NormalizeWhitespace().ToFullString(),
                    OutputPath = outputPath
                });
            }

            return res;
        }

        private SyntaxList<UsingDirectiveSyntax> GetUsings()
        {
            List<UsingDirectiveSyntax> usingDirective = new List<UsingDirectiveSyntax>()
            {
                UsingDirective(
                    QualifiedName(
                        QualifiedName(
                            QualifiedName(
                                IdentifierName("Microsoft"),
                                IdentifierName("VisualStudio")),
                            IdentifierName("TestTools")),
                        IdentifierName("UnitTesting")))
            };

            return List(usingDirective);
        }

        private string GetMethodName(List<string> methods, string method, int count)
        {
            var res = method + (count == 0 ? "" : count.ToString());
            if (methods.Contains(res)) return GetMethodName(methods, method, count + 1);

            return res;
        }

        private SyntaxList<MemberDeclarationSyntax> GetMethodsSyntaxList(List<string> methods)
        {
            var result = new List<MemberDeclarationSyntax>();
            foreach (var method in methods) result.Add(GetMethod(method));

            return List(result);
        }

        private MethodDeclarationSyntax GetMethod(string name)
        {
            return MethodDeclaration(
                    PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier(name + "Test"))
                .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(
                                Attribute(IdentifierName("TestMethod"))))))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithBody(Block(ExpressionStatement(InvocationExpression(
                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("Assert"), IdentifierName("Fail")))
                            .WithArgumentList(ArgumentList(SingletonSeparatedList(
                                Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,
                                    Literal("autogenerated")))))))));
        }
    }
}
