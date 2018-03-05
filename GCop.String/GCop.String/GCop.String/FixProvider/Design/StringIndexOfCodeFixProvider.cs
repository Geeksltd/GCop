namespace GCop.String.FixProvider.Design
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StringIndexOfCodeFixProvider)), Shared]
    public class StringIndexOfCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use Contains method";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop165");

        protected override void RegisterCodeFix()
        {
            try
            {
                var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<BinaryExpressionSyntax>().FirstOrDefault();
                if (token == null) return;

                Context.RegisterCodeFix(CodeAction.Create(Title, action => RemoveValue(Context.Document, token, action), Title), Diagnostic);
            }
            catch (NullReferenceException)
            {
                //No matter to handle NullReferenceException
            }
        }

        private async Task<Document> RemoveValue(Document document, BinaryExpressionSyntax binaryExpression, CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newInvocation = null;
            try
            {
                var invocation = binaryExpression.Left.IsKind(SyntaxKind.InvocationExpression) ? binaryExpression.Left as InvocationExpressionSyntax : binaryExpression.Right as InvocationExpressionSyntax;

                newInvocation = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.ParseExpression(invocation.Expression.GetIdentifier() + ".Contains"),
                    SyntaxFactory.ParseArgumentList("(" + invocation.ArgumentList.Arguments + ")")
                    );
            }
            catch
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(binaryExpression, newInvocation);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}