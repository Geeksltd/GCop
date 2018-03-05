namespace GCop.Common.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class LongInlineMethodDefinitionAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		public static int MaximumNumberOfCharacters = 100;
		protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "436",
				Category = Category.Style,
				Severity = DiagnosticSeverity.Warning,
				Message = "As the implementation is relatively long, change this into a standard method implementation."
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			var method = context.Node as MethodDeclarationSyntax;
			if (method.ExpressionBody == null || method.ExpressionBody.Expression?.ToString().Length <= MaximumNumberOfCharacters) return;

			ReportDiagnostic(context, method.ExpressionBody.ArrowToken);
		}
	}
}
