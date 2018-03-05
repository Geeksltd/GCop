namespace GCop.Common.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class PropertyToExpressionAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		readonly int MaxLength = 80;
		protected override SyntaxKind Kind => SyntaxKind.PropertyDeclaration;
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "647",
				Category = Category.Refactoring,
				Severity = DiagnosticSeverity.Warning,
				Message = "Shorten this property by defining it as expression-bodied."
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var property = (PropertyDeclarationSyntax)context.Node;
			
			if (property.AccessorList == null) return;
			if (property.AccessorList.Accessors.Count != 1) return;
			if (property.AccessorList.Accessors.First().Keyword.ToString() != "get") return;

			var body = property.AccessorList.Accessors.First().Body;
			if (body == null) return;
			if (body.GetText().ToString().Length >= MaxLength) return;

			var firstChild = body.ChildNodes().OfType<StatementSyntax>().First().ToString();
			if (firstChild.StartsWith("yield")) return;
			if (firstChild.StartsWith("throw")) return;
			if (firstChild.StartsWith("if")) return;
			if (!firstChild.StartsWith("return")) return;

			ReportDiagnostic(context, property.Identifier.GetLocation());
		}
	}
}
