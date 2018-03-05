namespace GCop.Common.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class MathRoundAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "521",
				Category = Category.Usage,
				Severity = DiagnosticSeverity.Warning,
				Message = "Change it to {0}.Round(digits)."
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;

			var memberAccess = NodeToAnalyze.ChildNodes().FirstOrDefault() as MemberAccessExpressionSyntax;
			if (memberAccess == null) return;

			if (memberAccess.GetIdentifier() != "Math") return;
			if (memberAccess.ChildNodes().None()) return;

			if (memberAccess.ChildNodes().LastOrDefault()?.GetIdentifier() != "Round") return;

			var argumentsList = NodeToAnalyze.As<InvocationExpressionSyntax>()?.ArgumentList;
			if (argumentsList.IsNone() || argumentsList.Arguments.Count > 2) return;

			var varibaleName = argumentsList.Arguments.FirstOrDefault()?.GetIdentifierSyntax();
			if (varibaleName == null) return;

			var symbol = context.SemanticModel.GetSymbolInfo(varibaleName).Symbol;
			if (symbol == null) return;

			if (symbol.Is("Double"))
				ReportDiagnostic(context, memberAccess, varibaleName.Identifier.ValueText);
		}
	}
}