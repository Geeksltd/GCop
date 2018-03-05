namespace GCop.Common.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using System.Collections.Generic;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class CommentTodoAnalyzer : GCopAnalyzer
	{
		protected string Word => "TODO";
		protected override RuleDescription GetDescription()
		{
			return new RuleDescription
			{
				ID = "437",
				Category = Category.Style,
				Severity = DiagnosticSeverity.Info,
				Message = "Complete the task associated to this \"TODO\" comment."
			};
		}

		protected override void Configure() => RegisterSyntaxTreeAction(Analyze);

		private void Analyze(SyntaxTreeAnalysisContext context)
		{
			var comments = context.Tree.GetCompilationUnitRoot().DescendantTrivia().Where(trivia => IsComment(trivia));

			foreach (var comment in comments)
			{
				var text = comment.ToString();

				foreach (var i in AllCaseInsensitiveIndexesOf(text, Word).Where(i => IsWordAt(text, i, Word.Length)))
				{
					var startLocation = comment.SpanStart + i;
					var location = Location.Create(
						context.Tree,
						TextSpan.FromBounds(startLocation, startLocation + Word.Length));

					context.ReportDiagnostic(Diagnostic.Create(Description, location));
				}
			}
		}

		private bool IsComment(SyntaxTrivia trivia)
		{
			return trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
				trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
				trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
				trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia);
		}

		private IEnumerable<int> AllCaseInsensitiveIndexesOf(string str, string value)
		{
			var count = 0;
			while ((count = str.IndexOf(value, count, str.Length - count, System.StringComparison.CurrentCultureIgnoreCase)) != -1)
			{
				yield return count;
				count += value.Length;
			}
		}

		private bool IsWordAt(string str, int startIndex, int count)
		{
			var leftBoundary = true;
			if (startIndex > 0)
			{
				leftBoundary = !char.IsLetterOrDigit(str[startIndex - 1]);
			}

			var rightBoundary = true;
			var rightOffset = startIndex + count;
			if (rightOffset < str.Length)
			{
				rightBoundary = !char.IsLetterOrDigit(str[rightOffset]);
			}

			return leftBoundary && rightBoundary;
		}
	}
}