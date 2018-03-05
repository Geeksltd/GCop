namespace GCop.String.FixProvider.Usage
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseStringCollectionIntersectsCodeFixProvider)), Shared]
    public class UseStringCollectionIntersectsCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use Intersects method";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop520");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => RemoveLines(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> RemoveLines(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newInvocation = null;
            try
            {
                var identifier = invocation.Expression.GetIdentifier();
                var lambda = (invocation.ArgumentList.Arguments[0].Expression as SimpleLambdaExpressionSyntax).ChildNodes().OfType<InvocationExpressionSyntax>().First();
                newInvocation = SyntaxFactory.InvocationExpression(SyntaxFactory.ParseExpression(identifier + ".Intersects"), SyntaxFactory.ParseArgumentList("(" + lambda.Expression.GetIdentifier() + ")"));
            }
            catch (Exception ex)
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(invocation, newInvocation);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}