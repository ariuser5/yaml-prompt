using Microsoft.CodeAnalysis;
using YamlPrompt.Development.Utils;

namespace YamlPrompt.Development.DevTests;

public class StaticAnalysisTests
{
    private const string _solutionPath = @"..\..\..\..\..\yaml-prompt.sln";
    
    [Fact]
    [Trait("TestCategory", "CodeAnalysis")]
    public void AllTestsShouldHaveTestCategoryTrait()
    {
        var failures = new List<string>();
        var knownTestCategories = StaticAnalysisMixins.GetKnownTestCategories();
        var projects = StaticAnalysisMixins.CompileProjects(_solutionPath);
        
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
                else if (!knownTestCategories.Contains(category)) {
                    failures.Add($"Test method '{fullMethodName}' has an unknown 'TestCategory' trait: '{category}'.");
                }
            }
        }
        
        Assert.True(failures.Count == 0, $"The following test methods have issues:\n{string.Join("\n", failures)}");
    }
}