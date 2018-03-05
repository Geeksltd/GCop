namespace GCop.Conditional.FixProvider.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EqualsTrueCodeFixProvider)), Shared]
	public class EqualsTrueCodeFixProvider : GCopCodeFixProvider
	{
		private string Title => "Remove true keyword";
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop161");

		protected override void RegisterCodeFix()
		{
			try
			{
				var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<BinaryExpressionSyntax>().FirstOrDefault();
				if (token == null) return;

				Context.RegisterCodeFix(CodeAction.Create(Title, action => RemoveTrue(Context.Document, token, action), Title), Diagnostic);
			}
			catch (NullReferenceException)
			{
				//No matter to handle NullReferenceException
			}
		}

		private async Task<Document> RemoveTrue(Document document, BinaryExpressionSyntax binaryExpression, CancellationToken cancellationToken)
		{
			BinaryExpressionSyntax newBinaryExpression = null;
			try
			{
				var variablePart = binaryExpression.Left.IsKind(SyntaxKind.TrueLiteralExpression) ? binaryExpression.Right : binaryExpression.Left;
				newBinaryExpression = SyntaxFactory.BinaryExpression(
					binaryExpression.Kind(),
					variablePart,
					SyntaxFactory.Token(binaryExpression.GetLeadingTrivia(), SyntaxKind.EqualsEqualsToken, "", "", binaryExpression.GetTrailingTrivia()),
					SyntaxFactory.ParseExpression(""));
			}
			catch
			{
				//No logging needed
			}

			var root = await document.GetSyntaxRootAsync(cancellationToken);
			var newRoot = root.ReplaceNode(binaryExpression, newBinaryExpression.WithoutTrailingTrivia());
			return document.WithSyntaxRoot(newRoot);
		}
	}
}