namespace GCop.Common.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class GetValueOrDefaultAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		private readonly string MethodName = "GetValueOrDefault";
		private readonly string NamespaceName = "System";
		protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "108",
				Category = Category.Design,
				Severity = DiagnosticSeverity.Warning,
				Message = "Instead of GetValueOrDefault(defaultValue) method use \" ?? defaultValue\"."
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var invocation = context.Node as InvocationExpressionSyntax;
			var memberAccessExpression = invocation?.Expression as MemberAccessExpressionSyntax;
			if (memberAccessExpression == null) return;

			var symbol = context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol;
			if (symbol == null) return;

			var method = symbol as IMethodSymbol;
			if (method == null) return;

			if (method.Name == MethodName && method.ContainingNamespace.Name == NamespaceName)
				ReportDiagnostic(context, memberAccessExpression.GetLocation());
		}
	}
}
