namespace GCop.Thread.FixProvider.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TaskWaitCodeFixProvider)), Shared]
    public class TaskWaitCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Add await keyword";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop651");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => AddAwait(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> AddAwait(Document document, MemberAccessExpressionSyntax memberAccess, CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newInvocation = null;
            var invocation = memberAccess.GetParent<InvocationExpressionSyntax>() as InvocationExpressionSyntax;
            try
            {
                newInvocation = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.ParseExpression("await " + invocation.Expression.ToString().Remove("().Wait"))
                    );

                newInvocation = newInvocation.WithLeadingTrivia(invocation.GetLeadingTrivia());
            }
            catch
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(invocation, newInvocation);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}