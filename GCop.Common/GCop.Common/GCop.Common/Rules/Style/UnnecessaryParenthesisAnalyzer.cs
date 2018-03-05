namespace GCop.Common.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class UnnecessaryParenthesisAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.ObjectCreationExpression;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "402",
				Category = Category.Style,
				Severity = DiagnosticSeverity.Warning,
				Message = "Remove unnecessary parenthesis."
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var objectCreation = context.Node as ObjectCreationExpressionSyntax;
			if (objectCreation.Initializer != null && objectCreation.ArgumentList != null && !objectCreation.ArgumentList.Arguments.Any())
			{
				ReportDiagnostic(context, objectCreation.ArgumentList.OpenParenToken.GetLocation());
			}
		}
	}
}
