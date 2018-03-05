namespace GCop.Common.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class UseInstanceMethodAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "129",
				Category = Category.Design,
				Message = "Change to an instance method, instead of taking a parameter '{0}' with the same type as the class.",
                Severity = DiagnosticSeverity.Warning
			};
		}

		/* Better to use "SyntaxKind.MethodDeclaration" than "SymbolKind.Method" 
        *  since "SyntaxKind.MethodDeclaration" narrows it down to ordinary methods only. */
		protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var methodNode = (MethodDeclarationSyntax)context.Node;
			var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodNode);

			// we dont need to look at non-static methods
			if (!methodSymbol.IsStatic) return;

			//Rule should be skipped if method name starts with FindBy
			if (methodNode.Identifier.ToString()?.StartsWith("FindBy") ?? true) return;

			var parameters = methodSymbol.Parameters.Where(it => it.IsOptional == false);

			foreach (var item in parameters)
			{
				if (item.Type == methodSymbol.ContainingType)
				{
					if (item.ContainingNamespace == methodSymbol.ContainingNamespace)
					{
						var diagnostic = Diagnostic.Create(Description, methodSymbol.Locations[0], item.MetadataName);
						context.ReportDiagnostic(diagnostic);
						return;
					}
				}
			}
		}
	}
}