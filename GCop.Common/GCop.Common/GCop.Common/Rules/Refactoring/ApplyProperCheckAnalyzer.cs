namespace GCop.Common.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ApplyProperCheckAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		static readonly string MessageWhenExpressionIsTrue = "A secure file should be made available only to authorised users, not just any user. Apply proper checks.";
		readonly string MessageWhenExpressionIsNotEquals = MessageWhenExpressionIsTrue + ". Also make the rule more flexible. Sometimes the parameter is not exactly IUser but another type which implements IUser. In that case the rule should still be shown.";

		protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "605",
				Category = Category.Refactoring,
				Severity = DiagnosticSeverity.Warning,
				Message = "{0}"
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var method = NodeToAnalyze as MethodDeclarationSyntax;

			if (method.Modifiers.None(it => it.ValueText == "public")) return;

			if (method.ReturnType.ToString() != "bool") return;

			var methodName = method.Identifier.Text;

			if (!(methodName.StartsWith("Is") && methodName.EndsWith("VisibleTo"))) return;
			//if (methodName.StartsWith("Is") && methodName.EndsWith("VisibleTo"))

			if (!method.ParameterList.Parameters.IsSingle()) return;

			var parameter = method.ParameterList.Parameters.First();
			if (parameter.Type?.ToString() != "IUser") return;

			var returnTrue = method.Body?.ChildNodes().FirstOrDefault() as ReturnStatementSyntax;
			if (returnTrue == null)
			{
				var @true = method.ExpressionBody?.ChildNodes().FirstOrDefault() as LiteralExpressionSyntax;
				if (@true?.Kind() == SyntaxKind.TrueLiteralExpression)
				{
					ReportDiagnostic(context, method.Identifier, MessageWhenExpressionIsTrue);
				}
				else if (method.ExpressionBody?.Expression?.Kind() == SyntaxKind.NotEqualsExpression)
				{
					var leftSide = method.ExpressionBody.Expression.As<BinaryExpressionSyntax>().Left as IdentifierNameSyntax;
					if (leftSide?.Identifier.ValueText != method.ParameterList.Parameters.FirstOrDefault()?.Identifier.ValueText) return;

					var rightSide = method.ExpressionBody.Expression.As<BinaryExpressionSyntax>().Right as LiteralExpressionSyntax;
					if (rightSide?.Kind() != SyntaxKind.NullLiteralExpression) return;

					ReportDiagnostic(context, method.Identifier, MessageWhenExpressionIsNotEquals);
				}
				return;
			}
			if (returnTrue.Expression == null) return;

			if (returnTrue.Expression.Kind() == SyntaxKind.TrueLiteralExpression) ReportDiagnostic(context, method.Identifier, MessageWhenExpressionIsTrue);

			else if (returnTrue.Expression.Kind() == SyntaxKind.NotEqualsExpression)
			{
				var leftSide = returnTrue.Expression.As<BinaryExpressionSyntax>().Left as IdentifierNameSyntax;
				if (leftSide?.Identifier.ValueText != method.ParameterList.Parameters.FirstOrDefault()?.Identifier.ValueText) return;

				var rightSide = returnTrue.Expression.As<BinaryExpressionSyntax>().Right as LiteralExpressionSyntax;
				if (rightSide?.Kind() != SyntaxKind.NullLiteralExpression) return;

				ReportDiagnostic(context, method.Identifier, MessageWhenExpressionIsNotEquals);
			}
		}
	}
}
