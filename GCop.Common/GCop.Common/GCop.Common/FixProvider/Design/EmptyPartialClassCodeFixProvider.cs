namespace GCop.Common.FixProvider.Design
{
    using GCop.Common.Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EmptyPartialClassCodeFixProvider)), Shared]
	public class EmptyPartialClassCodeFixProvider : GCopCodeFixProvider
	{
		private string Title => "Remove this partial class";
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop104");

		protected override void RegisterCodeFix()
		{
			try
			{
				var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().FirstOrDefault();
				if (token == null) return;

				Context.RegisterCodeFix(CodeAction.Create(Title, action => RemovePartialClass(Context.Document, token, action), Title), Diagnostic);
			}
			catch (NullReferenceException)
			{
				//No matter to handle NullReferenceException
			}
		}

		private async Task<Document> RemovePartialClass(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken cancellationToken)
		{
			var root = await document.GetSyntaxRootAsync(cancellationToken);
			var newRoot = root.RemoveNode(classDeclaration, SyntaxRemoveOptions.KeepLeadingTrivia & SyntaxRemoveOptions.KeepTrailingTrivia);
			return document.WithSyntaxRoot(newRoot);
		}
	}
}