namespace GCop.Common.FixProvider.Usage
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MathRoundCodeFixProvider)), Shared]
    public class MathRoundCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use Round method differently";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop521");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => UseRound(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> UseRound(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            ExpressionSyntax newExpression = null;
            try
            {
                var arguments = invocation.ArgumentList.Arguments.ToList();
                newExpression = SyntaxFactory.ParseExpression(arguments[0].ToString() + ".Round(" + arguments[1].ToString() + ")");
            }
            catch (Exception ex)
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(invocation, newExpression);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}