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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LacksCodeFixProvider)), Shared]
    public class LacksCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use Lacks method";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop153");

        protected override void RegisterCodeFix()
        {
            try
            {
                var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
                if (token == null) return;

                Context.RegisterCodeFix(CodeAction.Create(Title, action => RemoveValue(Context.Document, token, action), Title), Diagnostic);
            }
            catch (NullReferenceException)
            {
                //No matter to handle NullReferenceException
            }
        }

        private async Task<Document> RemoveValue(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            PrefixUnaryExpressionSyntax newPrefix = null;
            var parent = invocation.GetParent<PrefixUnaryExpressionSyntax>();
            try
            {
                newPrefix = SyntaxFactory.PrefixUnaryExpression(
                    parent.Kind(),
                    SyntaxFactory.Token(parent.GetLeadingTrivia(), SyntaxKind.ExclamationToken, "", "", parent.GetTrailingTrivia()),
                    SyntaxFactory.ParseExpression(invocation.Expression.GetIdentifier() + ".Lacks(" + invocation.ArgumentList.Arguments + ")"));
            }
            catch
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(parent, newPrefix);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}