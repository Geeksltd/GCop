namespace GCop.MSharp.FixProvider.Performance
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

	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AvoidCallingCountAfterGetListCodeFixProvider)), Shared]
	public class AvoidCallingCountAfterGetListCodeFixProvider : GCopCodeFixProvider
	{
		private string Title => "Use Database.Count method";
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop315");

		protected override void RegisterCodeFix()
		{
			try
			{
				var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
				if (token == null) return;

				Context.RegisterCodeFix(CodeAction.Create(Title, action => UseDatabaseCount(Context.Document, token, action), Title), Diagnostic);
			}
			catch (NullReferenceException)
			{
				//No matter to handle NullReferenceException
			}
		}

		private async Task<Document> UseDatabaseCount(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
		{
			InvocationExpressionSyntax newInvocation = null;
			var parentInvocation = invocation.GetParent<InvocationExpressionSyntax>() as InvocationExpressionSyntax;
			var argument = parentInvocation.ArgumentList.Arguments;
			var memberAccess = parentInvocation.ChildNodes().OfType<MemberAccessExpressionSyntax>().First() as MemberAccessExpressionSyntax;
			var secondInvocation = memberAccess.ChildNodes().OfType<InvocationExpressionSyntax>().First() as InvocationExpressionSyntax;
			var secondMemberAccess = secondInvocation.ChildNodes().OfType<MemberAccessExpressionSyntax>().First() as MemberAccessExpressionSyntax;
			var genericName = secondMemberAccess.ChildNodes().OfType<GenericNameSyntax>().First() as GenericNameSyntax;
			try
			{
				newInvocation = SyntaxFactory.InvocationExpression(
					SyntaxFactory.ParseExpression("Database.Count" + genericName.TypeArgumentList),
					SyntaxFactory.ParseArgumentList("(" + argument + ")")
					);
			}
			catch
			{
				//No logging needed
			}

			var root = await document.GetSyntaxRootAsync(cancellationToken);
			var newRoot = root.ReplaceNode(parentInvocation, newInvocation);
			return document.WithSyntaxRoot(newRoot);
		}
	}
}