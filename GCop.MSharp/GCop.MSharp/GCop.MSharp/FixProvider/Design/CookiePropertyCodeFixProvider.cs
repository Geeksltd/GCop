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

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CookiePropertyCodeFixProvider)), Shared]
	public class CookiePropertyCodeFixProvider : GCopCodeFixProvider
	{
		private string Title => "Use CookieProperty.Remove() instead";
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop156");

		protected override void RegisterCodeFix()
		{
			try
			{
				var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
				if (token == null) return;

				Context.RegisterCodeFix(CodeAction.Create(Title, action => UseCookieProperty(Context.Document, token, action), Title), Diagnostic);
			}
			catch (NullReferenceException)
			{
				//No matter to handle NullReferenceException
			}
		}

		private async Task<Document> UseCookieProperty(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
		{
			InvocationExpressionSyntax newInvocation = null;
			try
			{
				var key = invocation.ArgumentList.Arguments.FirstOrDefault(x => x.Expression.IsKind(SyntaxKind.StringLiteralExpression))?.ToString() ?? "";

				newInvocation = SyntaxFactory.InvocationExpression(
					SyntaxFactory.ParseExpression("CookieProperty.Remove"),
					SyntaxFactory.ParseArgumentList("(" + key + ")"));
			}
			catch
			{
				//No loggin needed
			}

			var root = await document.GetSyntaxRootAsync(cancellationToken);
			var newRoot = root.ReplaceNode(invocation, newInvocation);
			return document.WithSyntaxRoot(newRoot);
		}
	}
}