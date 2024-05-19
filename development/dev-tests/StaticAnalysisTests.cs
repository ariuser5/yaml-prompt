using Microsoft.CodeAnalysis;
using YamlPrompt.Development.Utils;

namespace YamlPrompt.Development.DevTests;

public class StaticAnalysisTests
{
    private const string solutionFileName = @"yaml-prompt.sln";
    
    private readonly string[] _knownTestCategories = StaticAnalysisMixins.GetKnownTestCategories();

    [Fact]
    [Trait("TestCategory", "CodeAnalysis")]
    public void AllTestsShouldHaveTestCategoryTrait()
    {
        var failures = new List<string>();
        var projects = CompileProjects();
        
        foreach (var compiledProject in projects)
        {
            (Project project, Compilation compilation) = compiledProject;
            var testMethods = project.SelectTestMethods();
            
            foreach (var method in testMethods)
            {
                var category = method.IdentifyTestCategory();
                
                var semanticModel = compilation.GetSemanticModel(method.SyntaxTree);
                var methodSymbol = semanticModel?.GetDeclaredSymbol(method);
                var format = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
                var fullMethodName = methodSymbol?.ToDisplayString(format) ?? method.Identifier.Text;
                
                if (category == null) {
                    failures.Add($"Test method '{fullMethodName}' does not have a 'TestCategory' trait.");
                } 
                else if (!_knownTestCategories.Contains(category)) {
                    failures.Add($"Test method '{fullMethodName}' has an unknown 'TestCategory' trait: '{category}'.");
                }
            }
        }
        
        Assert.True(failures.Count == 0, $"The following test methods have issues:\n{string.Join("\n", failures)}");
    }
    
    private IEnumerable<(Project project, Compilation compilation)> CompileProjects()
    {
        var solutonFile = FindSlnFile();
        Assert.True(solutonFile != null, $"Solution file '{solutionFileName}' not found.");
        return StaticAnalysisMixins.CompileProjects(solutonFile);
    }
    
    private static string? FindSlnFile()
    {
        var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (currentDir != null) {
            var file = Path.Combine(currentDir.FullName, solutionFileName);
            if (File.Exists(file)) return file;
            currentDir = currentDir.Parent;
        }

        return null;
    }
}