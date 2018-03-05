namespace GCop.MSharp.FixProvider.Style
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RedundantDatabaseGetListCodeFixProvider)), Shared]
    public class RedundantDatabaseGetListCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Remove unnecessary method";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop425");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => RemoveMethod(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> RemoveMethod(Document document, MethodDeclarationSyntax method, CancellationToken cancellationToken)
        {
            SyntaxNode newParent = null;
            ClassDeclarationSyntax parent = null;
            try
            {
                parent = method.GetParent(typeof(ClassDeclarationSyntax)) as ClassDeclarationSyntax;
                if (parent != null)
                    newParent = parent.RemoveNode(method, SyntaxRemoveOptions.KeepNoTrivia);
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