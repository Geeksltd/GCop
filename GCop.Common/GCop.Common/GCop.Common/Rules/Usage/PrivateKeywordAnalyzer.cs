namespace GCop.Common.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class PrivateKeywordAnalyzer : GCopAnalyzer
	{
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "524",
				Category = Category.Usage,
				Severity = DiagnosticSeverity.Hidden,
				Message = "Remove private keyword."
			};
		}

		protected override void Configure()
		{
			RegisterSyntaxNodeAction(AnalyzeFields, SyntaxKind.FieldDeclaration);
			RegisterSyntaxNodeAction(AnalyzeMethods, SyntaxKind.MethodDeclaration);
			RegisterSyntaxNodeAction(AnalyzeProperties, SyntaxKind.PropertyDeclaration);
		}

		void AnalyzeMethods(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var method = (MethodDeclarationSyntax)context.Node;
			var privateKeyword = method.Modifiers.FirstOrDefault(it => it.IsKind(SyntaxKind.PrivateKeyword));
			if (privateKeyword == null) return;
			ReportDiagnostic(context, privateKeyword.GetLocation());
		}

		void AnalyzeProperties(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var property = (PropertyDeclarationSyntax)context.Node;
			var privateKeyword = property.Modifiers.FirstOrDefault(it => it.IsKind(SyntaxKind.PrivateKeyword));
			if (privateKeyword == null) return;
			ReportDiagnostic(context, privateKeyword.GetLocation());
		}

		void AnalyzeFields(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var field = (FieldDeclarationSyntax)context.Node;
			var privateKeyword = field.Modifiers.FirstOrDefault(it => it.IsKind(SyntaxKind.PrivateKeyword));
			if (privateKeyword == null) return;
			ReportDiagnostic(context, privateKeyword.GetLocation());
		}
	}
}
