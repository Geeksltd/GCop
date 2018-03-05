namespace GCop.MSharp.FixProvider.Design
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

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DirectoryInfoCodeFixProvider)), Shared]
	public class DirectoryInfoCodeFixProvider : GCopCodeFixProvider
	{
		private string Title => "Use .AsDirectory()";
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop102");

		protected override void RegisterCodeFix()
		{
			try
			{
				var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ObjectCreationExpressionSyntax>().FirstOrDefault();
				if (token == null) return;

				Context.RegisterCodeFix(CodeAction.Create(Title, action => UseAsDirectory(Context.Document, token, action), Title), Diagnostic);
			}
			catch (NullReferenceException)
			{
				//No matter to handle NullReferenceException
			}
		}

		private async Task<Document> UseAsDirectory(Document document, ObjectCreationExpressionSyntax objectCreation, CancellationToken cancellationToken)
		{
			InvocationExpressionSyntax newInvocation = null;
			try
			{
				var argument = objectCreation.ChildNodes().OfType<ArgumentListSyntax>().FirstOrDefault()?.DescendantNodes().OfType<LiteralExpressionSyntax>().FirstOrDefault();

				newInvocation = SyntaxFactory.InvocationExpression(
					SyntaxFactory.ParseExpression(argument + ".AsDirectory"));
			}
			catch
			{
				//No loggin needed
			}

			var root = await document.GetSyntaxRootAsync(cancellationToken);
			var newRoot = root.ReplaceNode(objectCreation, newInvocation);
			return document.WithSyntaxRoot(newRoot);
		}
	}
}