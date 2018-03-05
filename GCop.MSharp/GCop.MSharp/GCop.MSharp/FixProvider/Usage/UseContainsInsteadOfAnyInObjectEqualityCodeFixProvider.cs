namespace GCop.MSharp.FixProvider.Usage
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseContainsInsteadOfAnyInObjectEqualityCodeFixProvider)), Shared]
    public class UseContainsInsteadOfAnyInObjectEqualityCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use Contains method";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop532");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => UseContains(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> UseContains(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newInvocation = null;
            try
            {
                var identifier = invocation.Expression.GetIdentifier();
                var memberAccess = (((invocation.ArgumentList.Arguments[0].Expression as SimpleLambdaExpressionSyntax).ChildNodes().ToList()[1] as BinaryExpressionSyntax).Right as MemberAccessExpressionSyntax);
                newInvocation = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.ParseExpression(identifier + ".Contains"),
                    SyntaxFactory.ParseArgumentList("(" + memberAccess + ")")
                    );
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