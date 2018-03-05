namespace GCop.MSharp.Rules.Refactoring
{
    using Core;
    using Core.Attributes;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [MSharpExclusive]
	[ZebbleExclusive]
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class AvoidUsingNoneOrAnyOnStringAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "606",
				Category = Category.Refactoring,
				Severity = DiagnosticSeverity.Warning,
				Message = "For string value existence checking use the more readable methods of .HasValue() or .IsEmpty()"
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var invocation = context.Node as InvocationExpressionSyntax;
			var memberAccessExpression = invocation.Expression as MemberAccessExpressionSyntax;

			//var method = memberAccessExpression.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().LastOrDefault();
			//if (method == null || method.Identifier.ValueText.IsNoneOf("Any" , "None")) return;

			//var variable = invocation.GetCallerOf(method.Identifier.ValueText);
			//if (variable == null) return;

			var method = memberAccessExpression?.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
			if (method == null || method.Identifier.ValueText.IsNoneOf("Any", "None")) return;

			if (invocation.ArgumentList.Arguments.Any()) return;

			var variable = memberAccessExpression.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
			if (variable == null) return;

			var symbol = context.SemanticModel.GetSymbolInfo(variable).Symbol;

			if (symbol.Is<string>())
			{
				ReportDiagnostic(context, invocation);
			}
		}
	}
}
