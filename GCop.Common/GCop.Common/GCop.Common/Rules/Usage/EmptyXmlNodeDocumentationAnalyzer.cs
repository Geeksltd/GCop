namespace GCop.Common.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class EmptyXmlNodeDocumentationAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.SingleLineDocumentationCommentTrivia;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "536",
				Category = Category.Usage,
				Severity = DiagnosticSeverity.Warning,
				Message = "Remove empty xml node documentation"
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var xmlDocumentation = context.Node as DocumentationCommentTriviaSyntax;

			xmlDocumentation.ChildNodes().OfType<XmlElementSyntax>().ForEach(element =>
			{
				var xmlTexts = element.ChildNodes().OfType<XmlTextSyntax>();

				if (xmlTexts.None())
				{
					ReportDiagnostic(context, element);
					return;
				}


				var text = xmlTexts.FirstOrDefault();
				if (text == null) return;

				if (text.TextTokens.Where(it => it.IsKind(SyntaxKind.XmlTextLiteralToken)).Any(it =>
				 {
					 return it.ValueText.Trim() != "";
				 })) return;

				ReportDiagnostic(context, element);
			});
		}
	}
}
