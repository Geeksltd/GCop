namespace GCop.MSharp.Rules.Performance
{
	using Core;
    using Core.Attributes;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Diagnostics;
	using Refactoring;
	using System;
	using System.Linq;

	[MSharpExclusive]
	[ZebbleExclusive]
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class DatabaseFindMethodAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		private InvocationExpressionSyntax Invocation;
		protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "305",
				Category = Category.Performance,
				Message = "Replace with Database.Find<{0}>({1})",
                Severity = DiagnosticSeverity.Warning
            };
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			Invocation = context.Node as InvocationExpressionSyntax;

			if (Invocation == null)
				return;

			var methodsToCheck = new[]
			{
				"First",
				"FirstOrDefault",
				"Single",
				"SingleOrDefault"
			};

			var memberAccessExpression = Invocation.ChildNodes().OfType<MemberAccessExpressionSyntax>()?.FirstOrDefault();
			if (memberAccessExpression == null)
				return;

			var getListInvocation = memberAccessExpression?.ChildNodes()?.OfType<InvocationExpressionSyntax>()?.FirstOrDefault();
			if (getListInvocation == null) return;
			var getListInvocationInfo = context.SemanticModel.GetSymbolInfo(getListInvocation).Symbol as IMethodSymbol;
			if (getListInvocationInfo == null) return;
			if (getListInvocationInfo.Name != "GetList") return;

			var identifierSystanx = memberAccessExpression.GetIdentifierSyntax();
			if (identifierSystanx == null) return;

			var methodAfterGetList = context.SemanticModel.GetSymbolInfo(identifierSystanx).Symbol as IMethodSymbol;
			if (methodAfterGetList == null) return;
			if (methodsToCheck.Lacks(methodAfterGetList.Name)) return;

			var lambda = Invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression as SimpleLambdaExpressionSyntax;
			if (lambda == null) return;

			var canRunInDbMode = CriteriaShouldBeConvertedToSqlAnalyzer.CanRunInDbMode(lambda, context.SemanticModel);
			if (canRunInDbMode.IsValid == false) return;

			var arguments = Invocation.ChildNodes().OfType<ArgumentListSyntax>()?.FirstOrDefault()?.ChildNodes()?.OfType<ArgumentSyntax>()?.FirstOrDefault()?.Expression?.ToString();
			var typeArguments = getListInvocationInfo?.TypeArguments;
			if (typeArguments == null) return;
			if (typeArguments.Value.None()) return;
			var getListType = typeArguments.Value[0].Name;
			ReportDiagnostic(context, Invocation, getListType, arguments);
		}
	}
}
