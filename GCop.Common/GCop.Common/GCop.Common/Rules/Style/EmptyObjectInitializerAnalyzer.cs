namespace GCop.Common.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class EmptyObjectInitializerAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.ObjectCreationExpression;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "400",
				Category = Category.Style,
				Severity = DiagnosticSeverity.Warning,
				Message = "{0}"
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var objectCreation = context.Node as ObjectCreationExpressionSyntax;

			if (objectCreation.Initializer != null && !objectCreation.Initializer.Expressions.Any())
			{
				ReportDiagnostic(context, objectCreation.Initializer.OpenBraceToken.GetLocation(), "Remove empty object initializer.");
			}
		}
	}
}
