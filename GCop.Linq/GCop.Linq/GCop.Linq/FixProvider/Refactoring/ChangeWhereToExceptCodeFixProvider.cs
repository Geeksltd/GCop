namespace GCop.Linq.FixProvider.Refactoring
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ChangeWhereToExceptCodeFixProvider)), Shared]
    public class ChangeWhereToExceptCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use Except method";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop607");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => UseExcept(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> UseExcept(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newInvocation = null;
            try
            {
                newInvocation = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.ParseExpression(invocation.Expression.ToString().Replace("Where", "Except")),
                    SyntaxFactory.ParseArgumentList("(" + invocation.ArgumentList.Arguments.ToString().Replace("!", "") + ")")
                    );
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