using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using YamlPrompt.Development.Utils;

namespace YamlPrompt.Development.CodeAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class TestsCodeAnalyzer : DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor Rule = new(
        id: "TestCategoryAnalyzer",
        title: "Test methods must have a TestCategory attribute with a valid value",
        messageFormat: "Test method '{0}' does not have a TestCategory attribute with a valid value (valid values are: {1})",
        category: "TestCategory",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
	
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(AnalyzeTestMethod, SyntaxKind.MethodDeclaration);
    }

    private void AnalyzeTestMethod(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        if (!method.IsTestMethod()) return;

        var testCategoryAttribute = method.IdentifyTestCategory();
		
        if (testCategoryAttribute == null) {
            ReportMissingTestCategory(context, method);
            return;
        }
		
		var knownTestCategories = StaticAnalysisMixins.GetKnownTestCategories();
		
		if (!knownTestCategories.Contains(testCategoryAttribute)) {
			ReportMissingTestCategory(context, method);
			return;
		}
    }
    
    private static void ReportMissingTestCategory(
        SyntaxNodeAnalysisContext context,
        MethodDeclarationSyntax method)
    {
        var knownTestCategories = string.Join(", ", StaticAnalysisMixins.GetKnownTestCategories());
        var diagnostic = Diagnostic.Create(Rule, method.GetLocation(), method.Identifier.Text, knownTestCategories);
        context.ReportDiagnostic(diagnostic);
    }
}
