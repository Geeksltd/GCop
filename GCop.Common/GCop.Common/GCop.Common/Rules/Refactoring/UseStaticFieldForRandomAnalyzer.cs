namespace GCop.Common.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class UseStaticFieldForRandomAnalyzer : GCopAnalyzer
	{
		protected override void Configure()
		{
			RegisterSyntaxNodeAction(Analyze, SyntaxKind.LocalDeclarationStatement, SyntaxKind.FieldDeclaration, SyntaxKind.PropertyDeclaration);
		}

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "614",
				Category = Category.Refactoring,
				Severity = DiagnosticSeverity.Warning,
				Message = "Random instance should be defined and instantiated as a static class field."
			};
		}

		protected void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;

			if (CheckLocalDeclaration())
			{
				ReportDiagnostic(context, NodeToAnalyze);
				return;
			}

			if (CheckFieldDeclaration())
			{
				ReportDiagnostic(context, NodeToAnalyze);
				return;
			}

			if (CheckPropertyDeclaration())
				ReportDiagnostic(context, NodeToAnalyze);
		}

		bool CheckFieldDeclaration()
		{
			var fieldExpression = NodeToAnalyze as FieldDeclarationSyntax;

			if (fieldExpression == null) return false;
			if (fieldExpression.Declaration?.GetIdentifier() != "Random") return false;

			if (fieldExpression.Modifiers.Any(SyntaxKind.StaticKeyword) == false) return true;

			return false;
		}

		bool CheckPropertyDeclaration()
		{
			var propertyExpression = NodeToAnalyze as PropertyDeclarationSyntax;
			if (propertyExpression == null) return false;

			if (propertyExpression.GetIdentifier() != "Random") return false;

			if (propertyExpression.Modifiers.Any(SyntaxKind.StaticKeyword) == false) return true;

			return false;
		}

		bool CheckLocalDeclaration()
		{
			var expression = NodeToAnalyze as LocalDeclarationStatementSyntax;
			if (expression == null) return false;

			var declaration = expression.Declaration;
			if (declaration == null) return false;

			// Random random;
			if (declaration.GetIdentifier() == "Random") return true;

			// var random = new Random();
			var objectCreationExpression =
				declaration.ChildNodes().OfType<VariableDeclaratorSyntax>()?
				.FirstOrDefault()?.ChildNodes()?.OfType<EqualsValueClauseSyntax>()?
				.FirstOrDefault()?.ChildNodes()?.OfType<ObjectCreationExpressionSyntax>()?.FirstOrDefault();

			if (objectCreationExpression == null)
			{
				var invocationExpression =
				declaration?.ChildNodes().OfType<VariableDeclaratorSyntax>()?
				.FirstOrDefault()?.ChildNodes()?.OfType<EqualsValueClauseSyntax>()?
				.FirstOrDefault()?.ChildNodes()?.OfType<InvocationExpressionSyntax>()?.FirstOrDefault();

				if (invocationExpression == null) return false;

				var last = invocationExpression.Expression?.ChildNodes()?.OfType<IdentifierNameSyntax>()?.ToList().LastOrDefault();
				if (last != null)
					if (last.ToString() == "Random") return true;
			}

			if (objectCreationExpression?.GetIdentifier() == "Random") return true;

			return false;
		}
	}
}
