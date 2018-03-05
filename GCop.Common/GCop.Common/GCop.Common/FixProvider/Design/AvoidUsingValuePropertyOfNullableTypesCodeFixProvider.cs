namespace GCop.Common.FixProvider.Design
{
    using GCop.Common.Core;
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AvoidUsingValuePropertyOfNullableTypesCodeFixProvider)), Shared]
	public class AvoidUsingValuePropertyOfNullableTypesCodeFixProvider : GCopCodeFixProvider
	{
		private string Title => "Remove .Value";
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop171");

		protected override void RegisterCodeFix()
		{
			try
			{
				var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
				if (token == null) return;

				Context.RegisterCodeFix(CodeAction.Create(Title, action => RemoveValue(Context.Document, token, action), Title), Diagnostic);
			}
			catch (NullReferenceException)
			{
				//No matter to handle NullReferenceException
			}
		}

		private async Task<Document> RemoveValue(Document document, MemberAccessExpressionSyntax memberAccess, CancellationToken cancellationToken)
		{
			MemberAccessExpressionSyntax newMemberAccess = null;
			try
			{
				newMemberAccess = SyntaxFactory.MemberAccessExpression(
					memberAccess.Kind(),
					memberAccess.Expression,
					SyntaxFactory.Token(memberAccess.GetLeadingTrivia(), SyntaxKind.DotToken, "", "", memberAccess.GetTrailingTrivia()),
					SyntaxFactory.IdentifierName(""));
			}
			catch
			{
				//No loggin needed
			}

			var root = await document.GetSyntaxRootAsync(cancellationToken);
			var newRoot = root.ReplaceNode(memberAccess, newMemberAccess);
			return document.WithSyntaxRoot(newRoot);
		}
	}
}