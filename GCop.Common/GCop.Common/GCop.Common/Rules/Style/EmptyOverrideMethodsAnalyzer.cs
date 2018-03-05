namespace GCop.Common.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class EmptyOverrideMethodsAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "410",
				Category = Category.Style,
				Severity = DiagnosticSeverity.Warning,
				Message = "This method seems unnecessary as it only calls the base virtual method."
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var method = context.Node as MethodDeclarationSyntax;
			if (method.GetName() == "GetHashCode") return;

			if (method.Body == null || method.Modifiers.None(it => it.Kind() == SyntaxKind.OverrideKeyword)) return;
			if (method.Body.ChildNodes().HasMany() || method.Body.ChildNodes().None()) return;
			var firstStatement = method.Body.ChildNodes().First();
			if (firstStatement == null) return;

			var methodName = method.Identifier.ValueText + method.TypeParameterList?.ToString();

			if (method.Body.DescendantNodes().OfType<InvocationExpressionSyntax>().HasMany()) return;

			InvocationExpressionSyntax invocation = null;
			if (firstStatement.IsKind(SyntaxKind.ReturnStatement))
			{
				invocation = firstStatement.ChildNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault();
			}
			else if (firstStatement.IsKind(SyntaxKind.ExpressionStatement))
			{
				invocation = (firstStatement as ExpressionStatementSyntax)?.Expression as InvocationExpressionSyntax;
			}

			if (invocation == null) return;

			var baseCallExpression = invocation.Expression.As<MemberAccessExpressionSyntax>()?.ChildNodes().OfType<BaseExpressionSyntax>().FirstOrDefault();
			if (baseCallExpression == null) return;

			var baseMethodName = invocation.Expression.GetIdentifier();
			if (baseMethodName.IsEmpty())
			{
				baseMethodName = invocation.Expression.ChildNodes().OfType<GenericNameSyntax>().FirstOrDefault()?.ToString();
			}

			if (baseMethodName != methodName) return;

			if (method.ParameterList.Parameters.TrueForAtLeastOnce(parameter =>
			 {
				 return invocation.ArgumentList.Arguments.None(it => (it.Expression as IdentifierNameSyntax)?.Identifier.ValueText == parameter.Identifier.ValueText);
			 })) return;

			ReportDiagnostic(context, method.Identifier);
		}
	}
}
