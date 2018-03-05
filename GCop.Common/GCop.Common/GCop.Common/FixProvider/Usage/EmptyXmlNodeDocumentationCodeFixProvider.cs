namespace GCop.Common.FixProvider.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EmptyXmlNodeDocumentationCodeFixProvider)), Shared]
    public class EmptyXmlNodeDocumentationCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Remove empty documentation";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop536");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).LeadingTrivia.ToList().First(x => x.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => RemoveDocumentation(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> RemoveDocumentation(Document document, SyntaxTrivia documentation, CancellationToken cancellationToken)
        {
            SyntaxNode newParent = null;
            SyntaxNode parent = null;

            try
            {
                parent = documentation.Token.Parent;
                newParent = parent.WithoutLeadingTrivia();
                var whiteSpaceCount = parent.GetLeadingTrivia().Where(x => x.IsKind(SyntaxKind.WhitespaceTrivia)).Count();
                newParent = newParent.WithLeadingTrivia(parent.GetLeadingTrivia().Except(x => x.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)).Take(whiteSpaceCount - 1));
            }
            catch (Exception ex)
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(parent, newParent);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}