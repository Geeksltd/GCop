namespace GCop.Common.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class DelegatesComplexCodeAnalyzer : GCopAnalyzer
	{
		private readonly string[] MethodNames = new string[] { "OnSaving", "OnDeleting" };

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "176",
				Category = Category.Design,
				Severity = DiagnosticSeverity.Warning,
				Message = "This anonymous method should not contain complex code, Instead call other focused methods to perform the complex logic"
			};
		}

		protected override void Configure()
		{
			RegisterSyntaxNodeAction(context => AnalyzeSyntax(context), SyntaxKind.AnonymousMethodExpression
																	  , SyntaxKind.ParenthesizedLambdaExpression);
		}

		private void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
		{
			var node = context.Node;
			NodeToAnalyze = node;

			BlockSyntax block = null;
			Location errorLocation = null;

			if (node is ParenthesizedLambdaExpressionSyntax inlineMethod)
			{
				block = inlineMethod.Body as BlockSyntax;
				errorLocation = inlineMethod.ParameterList.GetLocation();
			}
			else if (node is AnonymousMethodExpressionSyntax anonymousMethod)
			{
				block = anonymousMethod.Body as BlockSyntax;
				errorLocation = anonymousMethod.GetFirstToken().GetLocation();
			}

			//if (block?.IsTooLong(Numbers.Ten) ?? false)
			if (block == null) return;
			if (block?.IsTooLong(Numbers.Ten) == false) return;

			var containingMethod = NodeToAnalyze.GetSingleAncestor<MethodDeclarationSyntax>();
			if (containingMethod != null)
			{
				if (containingMethod.Body.Statements.Except(NodeToAnalyze).Count() <= Numbers.Six) return;
			}
			ReportDiagnostic(context, errorLocation);
		}
	}
}
