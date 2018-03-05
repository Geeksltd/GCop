namespace GCop.MSharp.Rules.Design
{
	using Core;
    using Core.Attributes;
    using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Diagnostics;

	[MSharpExclusive]
	[ZebbleExclusive]
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class StringEqualsAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "157",
				Category = Category.Design,
				Message = "Use == instead",
                Severity = DiagnosticSeverity.Warning
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var invocation = context.Node as InvocationExpressionSyntax;
			if (invocation == null) return;

			var method = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
			if (method == null) return;

			if (method.Name == "Equals" && method.ContainingType.Name == "String")
			{
				if (invocation.ArgumentList.Arguments.Count == 1)
					ReportDiagnostic(context, invocation);
			}
		}
	}
}
