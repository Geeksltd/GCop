namespace GCop.MSharp.FixProvider.Refactoring
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseReloadInsteadOfGetCodeFixProvider)), Shared]
    public class UseReloadInsteadOfGetCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use Database.Reload method";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop611");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => UseReload(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> UseReload(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newInvocation = null;

            try
            {
                var memberAccess = invocation.ChildNodes().OfType<MemberAccessExpressionSyntax>().First();
                newInvocation = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.ParseExpression(memberAccess.ToString().Replace("Get", "Reload")),
                    SyntaxFactory.ParseArgumentList("(" + invocation.ArgumentList.Arguments.ToString().Replace("ID", "") + ")"));
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