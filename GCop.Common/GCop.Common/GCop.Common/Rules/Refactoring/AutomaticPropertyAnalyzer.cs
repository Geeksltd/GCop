namespace GCop.Common.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class AutomaticPropertyAnalyzer : GCopAnalyzer
	{
		protected override void Configure()
		{
			RegisterSyntaxNodeAction(Analyze, SyntaxKind.PropertyDeclaration);
		}

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "634",
				Category = Category.Refactoring,
				Severity = DiagnosticSeverity.Warning,
				Message = "Instead of private property, use a class field."
			};
		}

		protected void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;

			var variable = context.SemanticModel.GetDeclaredSymbol(NodeToAnalyze);
			if (variable == null) return;

			var node = NodeToAnalyze as PropertyDeclarationSyntax;
			if (node.ExpressionBody != null) return;

			//When there is a return statement in getaccessor, it's not an "automatic property" , skip it
			var getAccessor = node.AccessorList.ChildNodes().OfKind(SyntaxKind.GetAccessorDeclaration).FirstOrDefault();
			if (getAccessor == null) return;
			if (getAccessor.DescendantNodes().OfKind(SyntaxKind.ReturnStatement).Any()) return;

			//Skip the rule if it's an explicit interface definition
			if (node.ChildNodes().OfType<ExplicitInterfaceSpecifierSyntax>().Any()) return;


			//[get;]: only private  type variable { [get;] set;}
			if (getAccessor.ChildTokens().FirstOrDefault().ValueText == "get" &&
				getAccessor.ChildTokens().FirstOrDefault().GetNextToken().ValueText == ";")
			{
				if (variable.DeclaredAccessibility != Accessibility.Private) return;
				ReportDiagnostic(context, NodeToAnalyze);
			}
		}
	}
}