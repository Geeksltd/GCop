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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StaticMethodsCallsAnotherStaticMethodCodeFixProvider)), Shared]
    public class StaticMethodsCallsAnotherStaticMethodCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Remove class name";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop433");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IdentifierNameSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => RemoveClassName(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> RemoveClassName(Document document, IdentifierNameSyntax identifier, CancellationToken cancellationToken)
        {
            ExpressionSyntax newExpression = null;
            try
            {
                newExpression = SyntaxFactory.ParseExpression(identifier.Parent.ToString().Replace(identifier.ToString() + ".", ""));
            }
            catch (Exception ex)
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(identifier.Parent, newExpression);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}