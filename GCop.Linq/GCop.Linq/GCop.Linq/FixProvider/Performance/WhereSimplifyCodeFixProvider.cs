namespace GCop.Linq.FixProvider.Performance
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(WhereSimplifyCodeFixProvider)), Shared]
    public class WhereSimplifyCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Remove Where";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop314");

        protected override void RegisterCodeFix()
        {
            try
            {
                var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
                if (token == null) return;

                Context.RegisterCodeFix(CodeAction.Create(Title, action => RemoveWhere(Context.Document, token, action), Title), Diagnostic);
            }
            catch (NullReferenceException)
            {
                //No matter to handle NullReferenceException
            }
        }

        private async Task<Document> RemoveWhere(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newInvocation = null;
            var parentInvocation = invocation.GetParent<InvocationExpressionSyntax>() as InvocationExpressionSyntax;
            var memberAccess = parentInvocation.ChildNodes().OfType<MemberAccessExpressionSyntax>().First() as MemberAccessExpressionSyntax;
            var secondMemberAccess = invocation.ChildNodes().OfType<MemberAccessExpressionSyntax>().First();
            try
            {
                newInvocation = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.ParseExpression(secondMemberAccess.ToString().Replace("Where", memberAccess.GetIdentifier())),
                    SyntaxFactory.ParseArgumentList("(" + invocation.ArgumentList.Arguments + ")")
                    );
            }
            catch
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(parentInvocation, newInvocation);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}