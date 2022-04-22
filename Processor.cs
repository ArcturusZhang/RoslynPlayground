using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

namespace RoslynPlayground
{
    internal static class Processor
    {
        public static async Task<Project> Process(Project project)
        {
            await foreach (var c in GetPublicClasses(project))
            {
                if (c.Identifier.Text.Equals("A"))
                {
                    await FindReferences(project, c);
                }
            }
            return project;
        }

        private static async Task FindReferences(Project project, ClassDeclarationSyntax classDeclaration)
        {
            var compilation = await project.GetCompilationAsync();
            var sematicModel = compilation!.GetSemanticModel(classDeclaration.SyntaxTree);
            var symbol = sematicModel.GetDeclaredSymbol(classDeclaration);
            foreach (var reference in await SymbolFinder.FindReferencesAsync(symbol!, project.Solution))
            {
                foreach (var location in reference.Locations)
                {
                    Console.WriteLine($"SyntaxNode {classDeclaration.Identifier.Text} is referenced by document {location.Document.Name}");
                }
            }
        }

        private static async IAsyncEnumerable<ClassDeclarationSyntax> GetPublicClasses(Project project)
        {
            foreach (var document in project.Documents)
            {
                var root = await document.GetSyntaxRootAsync();
                if (root == null)
                    continue;
                var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                if (classes.Any())
                    yield return classes.First();
            }
        }
    }
}
