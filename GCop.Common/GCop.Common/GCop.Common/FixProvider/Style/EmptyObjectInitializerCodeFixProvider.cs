namespace GCop.Common.FixProvider.Style
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EmptyObjectInitializerCodeFixProvider)), Shared]
    public class EmptyObjectInitializerCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Remove object initializer";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop400");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ObjectCreationExpressionSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => RemoveInitializer(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> RemoveInitializer(Document document, ObjectCreationExpressionSyntax objectCreation, CancellationToken cancellationToken)
        {
            ObjectCreationExpressionSyntax newObjectCreation = null;
            try
            {
                newObjectCreation = SyntaxFactory.ObjectCreationExpression(
                    objectCreation.NewKeyword,
                    objectCreation.Type,
                    objectCreation.ArgumentList,
                    null
                    );
            }
            catch (Exception ex)
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(objectCreation, newObjectCreation);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}