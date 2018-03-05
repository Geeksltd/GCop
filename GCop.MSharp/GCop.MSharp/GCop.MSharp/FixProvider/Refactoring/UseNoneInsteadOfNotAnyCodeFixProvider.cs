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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseNoneInsteadOfNotAnyCodeFixProvider)), Shared]
    public class UseNoneInsteadOfNotAnyCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use None method";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop615");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PrefixUnaryExpressionSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => UseNone(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> UseNone(Document document, PrefixUnaryExpressionSyntax prefix, CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newInvocation = null;

            try
            {
                var invocation = prefix.ChildNodes().OfType<InvocationExpressionSyntax>().First();
                newInvocation = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.ParseExpression(invocation.Expression.GetIdentifier() + ".None"),
                    SyntaxFactory.ParseArgumentList("(" + invocation.ArgumentList.Arguments + ")")
                    );
            }
            catch
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(prefix, newInvocation);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}