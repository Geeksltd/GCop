namespace GCop.Common.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class LineLengthAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
	{
		private readonly int Maximum = 200;
		protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "419",
				Category = Category.Style,
				Severity = DiagnosticSeverity.Warning,
				Message = "This statement is too long and hard to read. Press Enter at logical breaking points to split it into multiple lines."
			};
		}

		protected override void Analyze(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var block = NodeToAnalyze.As<MethodDeclarationSyntax>().Body;
			if (block == null) return;

			var strings = block.DescendantNodes().OfType<LiteralExpressionSyntax>().Where(it => it.IsKind(SyntaxKind.StringLiteralExpression)).Select(it => it.Token.ToString());
			var interpolatedStrings = block.DescendantNodes().OfType<InterpolatedStringTextSyntax>().Select(it => it.TextToken.ToString());

			var blockText = block.SyntaxTree.GetText();
			blockText.Lines.ForEach(line =>
			{
				if (line.ToString().Trim().EndsWith(";"))
				{
					var lineWithoutLiteral = line.ToString().Trim();
					strings.Union(interpolatedStrings).ForEach(it => lineWithoutLiteral = lineWithoutLiteral.Remove(it));

					if (lineWithoutLiteral.Length > Maximum)
					{
						var node = block.DescendantNodes(line.Span).FirstOrDefault(it => line.Span.Contains(it.Span));
						if (node != null)
							ReportDiagnostic(context, node.GetLocation());
					}
				}
			});
		}
	}
}
