namespace GCop.Conditional.FixProvider.Design
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FloatVariablesComparisonCodeFixProvider)), Shared]
    public class FloatVariablesComparisonCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use AlmostEquals method";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop180");

        protected override void RegisterCodeFix()
        {
            try
            {
                var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<BinaryExpressionSyntax>().FirstOrDefault();
                if (token == null) return;

                Context.RegisterCodeFix(CodeAction.Create(Title, action => UseAlmostEquals(Context.Document, token, action), Title), Diagnostic);
            }
            catch (NullReferenceException)
            {
                //No matter to handle NullReferenceException
            }
        }

        private async Task<Document> UseAlmostEquals(Document document, BinaryExpressionSyntax binaryExpression, CancellationToken cancellationToken)
        {
            BinaryExpressionSyntax newBinaryExpression = null;
            try
            {
                newBinaryExpression = SyntaxFactory.BinaryExpression(
                    binaryExpression.Kind(),
                    SyntaxFactory.ParseExpression(binaryExpression.Left + ".AlmostEquals(" + binaryExpression.Right + ")"),
                    SyntaxFactory.Token(binaryExpression.GetLeadingTrivia(), SyntaxKind.EqualsEqualsToken, "", "", binaryExpression.GetTrailingTrivia()),
                    SyntaxFactory.ParseExpression(""));
            }
            catch
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(binaryExpression, newBinaryExpression);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}