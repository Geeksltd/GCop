namespace GCop.Common.Rules.Naming
{
	using Core;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Diagnostics;

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class EndOfMethodOrEnumNamesAnalyzer : GCopAnalyzer
	{
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "202",
				Category = Category.Naming,
				Severity = DiagnosticSeverity.Warning,
				Message = "Don’t end the name of {0} with the same name as the {1}"
			};
		}

		protected override void Configure()
		{
			RegisterSyntaxNodeAction(context => AnalyzeName(context), SyntaxKind.MethodDeclaration
																	, SyntaxKind.EnumMemberDeclaration);
		}

		private void AnalyzeName(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var node = context.Node;

			if (node is MethodDeclarationSyntax method)
			{
				if (method.Modifiers.Any(SyntaxKind.StaticKeyword)) return;

				var classDeclaration = method.Parent as ClassDeclarationSyntax;

				//Maybe method is declared in interface
				if (classDeclaration == null) return;

				if (method.Identifier.ValueText.EndsWith(classDeclaration?.Identifier.ValueText))
					context.ReportDiagnostic(Diagnostic.Create(Description, method.Identifier.GetLocation(), "methods", "class"));
			}
			else if (node is EnumMemberDeclarationSyntax enumMember)
			{
				var enumDeclaration = (EnumDeclarationSyntax)enumMember.Parent;

				if (enumMember.Identifier.ValueText.EndsWith(enumDeclaration.Identifier.ValueText))
					context.ReportDiagnostic(Diagnostic.Create(Description, enumMember.Identifier.GetLocation(), "enum members", "enum"));
			}
		}
	}
}
