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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IntersectCodeFixProvider)), Shared]
    public class IntersectCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use Intersect method";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop155");

        protected override void RegisterCodeFix()
        {
            try
            {
                var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
                if (token == null) return;

                Context.RegisterCodeFix(CodeAction.Create(Title, action => UseIntersect(Context.Document, token, action), Title), Diagnostic);
            }
            catch (NullReferenceException)
            {
                //No matter to handle NullReferenceException
            }
        }

        private async Task<Document> UseIntersect(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newInvocation = null;
            var firstInvocation = invocation.GetParent<InvocationExpressionSyntax>() as InvocationExpressionSyntax;
            try
            {
                var first = firstInvocation.Expression.GetIdentifier();
                var second = invocation.Expression.GetIdentifier();

                newInvocation = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.ParseExpression(second + ".Intersect"),
                    SyntaxFactory.ParseArgumentList("(" + first + ")"));
            }
            catch
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(firstInvocation, newInvocation);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}