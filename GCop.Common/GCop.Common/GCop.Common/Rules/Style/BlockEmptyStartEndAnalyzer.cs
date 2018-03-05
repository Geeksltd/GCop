namespace GCop.Common.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class BlockEmptyStartEndAnalyzer : GCopAnalyzer
	{
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "438",
				Category = Category.Style,
				Severity = DiagnosticSeverity.Warning,
				Message = "Blocks should not start ​or end ​with empty lines."
			};
		}

		protected override void Configure()
		{
			RegisterSyntaxNodeAction(AnalyzeBlockStart, SyntaxKind.Block);
			RegisterSyntaxNodeAction(AnalyzeBlockEnd, SyntaxKind.Block);
		}

		void AnalyzeBlockStart(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var block = (BlockSyntax)context.Node;
			bool trailFlag = false, leadFlag = false;

			if (block == null) return;
			var openBrace = block.OpenBraceToken;
			if (openBrace == null) return;

			var firstStatement = block.ChildNodes()?.FirstOrDefault(c => c.Kind() != SyntaxKind.OpenBraceToken && c.Kind() != SyntaxKind.CloseBraceToken);
			if (firstStatement == null) return;

			var leadList = firstStatement.GetLeadingTrivia();
			if (leadList.None(l => l.Kind() == SyntaxKind.EndOfLineTrivia)) leadFlag = true;

			if (leadFlag == false)
				if (leadList.Any(x => x.Kind() != SyntaxKind.EndOfLineTrivia && x.Kind() != SyntaxKind.WhitespaceTrivia))
					leadFlag = true;

			var trailList = openBrace.TrailingTrivia;
			if (trailList.Count(t => t.Kind() == SyntaxKind.EndOfLineTrivia) < 2) trailFlag = true;

			if (trailFlag == false)
				if (trailList.Any(x => x.Kind() != SyntaxKind.EndOfLineTrivia && x.Kind() != SyntaxKind.WhitespaceTrivia && x.Kind() != SyntaxKind.SkippedTokensTrivia))
					trailFlag = true;

			if (!trailFlag || !leadFlag)
				ReportDiagnostic(context, openBrace.GetLocation());
		}

		void AnalyzeBlockEnd(SyntaxNodeAnalysisContext context)
		{
			NodeToAnalyze = context.Node;
			var block = (BlockSyntax)context.Node;

			if (block == null) return;

			var closeBrace = block.CloseBraceToken;
			if (closeBrace == null) return;

			var leadList = closeBrace.LeadingTrivia;
			if (leadList.None(t => t.Kind() == SyntaxKind.EndOfLineTrivia)) return;

			if (leadList.Any(x => x.Kind() != SyntaxKind.EndOfLineTrivia && x.Kind() != SyntaxKind.WhitespaceTrivia)) return;

			ReportDiagnostic(context, closeBrace.GetLocation());
		}
	}
}
