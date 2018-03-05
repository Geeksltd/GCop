namespace GCop.String.FixProvider.Style
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RedundantToStringCodeFixProvider)), Shared]
    public class RedundantToStringCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Remove .ToString()";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop414");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ExpressionSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => RemoveLines(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> RemoveLines(Document document, ExpressionSyntax expression, CancellationToken cancellationToken)
        {
            ExpressionSyntax newExpression = null;
            try
            {
                newExpression = SyntaxFactory.ParseExpression(expression.Parent.ToString().Replace(".ToString()", ""));
            }
            catch (Exception ex)
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(expression.Parent, newExpression);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}