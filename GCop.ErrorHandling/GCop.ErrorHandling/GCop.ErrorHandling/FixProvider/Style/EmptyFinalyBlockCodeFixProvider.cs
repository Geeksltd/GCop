namespace GCop.ErrorHandling.FixProvider.Style
{
    using Core;
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EmptyFinalyBlockCodeFixProvider)), Shared]
    public class EmptyFinalyBlockCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Remove finally block";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop435");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<FinallyClauseSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => RemoveFinally(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> RemoveFinally(Document document, FinallyClauseSyntax finallyBlock, CancellationToken cancellationToken)
        {
            SyntaxNode newParent = null;
            try
            {
                var parent = finallyBlock.Parent;
                if (parent != null)
                {
                    newParent = parent.RemoveNode(finallyBlock, SyntaxRemoveOptions.KeepNoTrivia);
                }

                var root = await document.GetSyntaxRootAsync(cancellationToken);
                var newRoot = root.ReplaceNode(parent, newParent);
                return document.WithSyntaxRoot(newRoot);
            }
            catch (Exception ex)
            {
                //No logging needed
            }
            return document;
        }
    }
}