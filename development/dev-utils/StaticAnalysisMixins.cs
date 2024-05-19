using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using YamlPrompt.Development.Rules;

namespace YamlPrompt.Development.Utils;

public static class StaticAnalysisMixins
{
	public static string[] GetKnownTestCategories()
	{
		return TestCategoryRules.KnownTestCategories.Split(';', StringSplitOptions.RemoveEmptyEntries);
	}
	
	public static IEnumerable<(Project project, Compilation compilation)> CompileProjects(string solutionPath)
    {
        var workspace = MSBuildWorkspace.Create();
        var solution = workspace.OpenSolutionAsync(solutionPath).Result;
        var projects = solution.Projects;
        
        foreach (var project in projects)
        {
            var compilation = project.GetCompilationAsync().Result;
            if (compilation != null)
            {
                yield return (project, compilation);
            }
        }
    }
	
	public static bool IsTestMethod(this MethodDeclarationSyntax method)
	{
		return method.AttributeLists
			.SelectMany(a => a.Attributes)
			.Any(a => a.Name.ToString() == "Fact" || a.Name.ToString() == "Theory");
	}
    
    public static IEnumerable<MethodDeclarationSyntax> SelectTestMethods(this Project project)
    {
        return project.GetCompilationAsync().Result?.SyntaxTrees.SelectMany(w => {
			var root = w.GetRoot();
			return root.DescendantNodes().OfType<MethodDeclarationSyntax>();
		}).Where(IsTestMethod) ?? [];
    }
    
    public static string? IdentifyTestCategory(this MethodDeclarationSyntax method)
    {
        return (method as MemberDeclarationSyntax).IdentifyTestCategory()
            ?? (method.Parent as ClassDeclarationSyntax)?.IdentifyTestCategory();
    }
    
    private static string? IdentifyTestCategory(this MemberDeclarationSyntax memberDeclaration)
    {
        const string traitAttributeName = "Trait";
        const string testCategoryTraitName = "TestCategory";
        
        var testCategories = memberDeclaration.AttributeLists
            .SelectMany(attributesList => attributesList.Attributes.Where(a => 
                a.Name.ToString() == traitAttributeName && 
                a.GetLiteralStringArgumentValue(0) == testCategoryTraitName))
            .Select(a => a.GetLiteralStringArgumentValue(1))
            .ToList();
        
        if (testCategories.Count > 1) {
            var memberDeclarationName = memberDeclaration.GetMemberDeclarationName();
            throw new Exception($"Multiple '{testCategoryTraitName}' attributes found for member '{memberDeclarationName}'.");
        }
        
        return testCategories.SingleOrDefault();
    }
    
    private static string? GetMemberDeclarationName(this MemberDeclarationSyntax memberDeclaration)
    {
        return memberDeclaration switch
        {
            MethodDeclarationSyntax method => method.Parent is ClassDeclarationSyntax classDeclaration
                ? $"{classDeclaration.Identifier.Text}.{method.Identifier.Text}"
                : method.Identifier.Text,
            ClassDeclarationSyntax @class => @class.Identifier.Text,
            _ => null
        };
    }
    
    private static string GetLiteralStringArgumentValue(this AttributeSyntax attribute, int argumentIndex)
    {
        var nonNullArgumentList = attribute.ArgumentList 
            ?? throw new ArgumentException("Attribute does not have an argument list.");
        
        var argumentAsLiteralString = nonNullArgumentList.Arguments[argumentIndex].Expression as LiteralExpressionSyntax
            ?? throw new ArgumentException($"Attribute argument at position '{argumentIndex}' is not a literal string.");
        
        return argumentAsLiteralString.Token.ValueText;
    }
}
