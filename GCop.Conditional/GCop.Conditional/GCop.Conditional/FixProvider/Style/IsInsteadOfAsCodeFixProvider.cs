namespace GCop.Conditional.FixProvider.Style
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IsInsteadOfAsCodeFixProvider)), Shared]
    public class IsInsteadOfAsCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use is keyword";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop431");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<BinaryExpressionSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => UseIs(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> UseIs(Document document, BinaryExpressionSyntax binary, CancellationToken cancellationToken)
        {
            ExpressionSyntax newExpression = null;
            BinaryExpressionSyntax parent = null;
            try
            {
                parent = binary.GetParent(typeof(BinaryExpressionSyntax)) as BinaryExpressionSyntax;
                newExpression = SyntaxFactory.ParseExpression(binary.Left.ToString() + " is " + binary.Right.ToString());
            }
            catch (Exception ex)
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(parent, newExpression);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}