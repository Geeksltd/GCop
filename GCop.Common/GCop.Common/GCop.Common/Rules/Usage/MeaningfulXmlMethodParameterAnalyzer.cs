namespace GCop.Common.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class MeaningfulXmlMethodParameterAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		protected override SyntaxKind Kind => SyntaxKind.SingleLineDocumentationCommentTrivia;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "511",
				Category = Category.Usage,
				Severity = DiagnosticSeverity.Warning,
				Message = "Either remove the parameter documentation node, or describe it properly."
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var xmlDocumentation = context.Node as DocumentationCommentTriviaSyntax;

			foreach (var element in xmlDocumentation.DescendantNodes().OfType<XmlElementSyntax>())
			{
				var startTag = element.ChildNodes().OfType<XmlElementStartTagSyntax>().First();
				var isParamNode = startTag.Name.LocalName.ValueText == "param";

				if (!isParamNode)
					continue;

				var paramName = startTag.ChildNodes().OfType<XmlNameAttributeSyntax>().FirstOrDefault()?.Identifier.ToString();
				var value = element.ChildNodes().OfType<XmlTextSyntax>().FirstOrDefault()?.TextTokens.ToString().Trim();

				if (paramName.IsEmpty() || value.IsEmpty())
					continue;

				value = Regex.Replace(value, @"\s+", " ");  // Consider using StringBuilder to improve performance.

				if (value.Equals("the " + paramName, StringComparison.OrdinalIgnoreCase) || value.Equals($"the {paramName}.", StringComparison.OrdinalIgnoreCase))
					ReportDiagnostic(context, element);
			}
		}
	}
}