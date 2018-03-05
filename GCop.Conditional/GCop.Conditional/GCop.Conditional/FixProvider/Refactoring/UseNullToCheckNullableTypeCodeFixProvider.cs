namespace GCop.Conditional.FixProvider.Refactoring
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseNullToCheckNullableTypeCodeFixProvider)), Shared]
    public class UseNullToCheckNullableTypeCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use null to avoid using negative logic";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop690");

        protected override void RegisterCodeFix()
        {
            var binaryToken = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<BinaryExpressionSyntax>().FirstOrDefault();
            var prefixToken = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PrefixUnaryExpressionSyntax>().FirstOrDefault();

            if (binaryToken == null && prefixToken == null) return;

            if (binaryToken != null)
                Context.RegisterCodeFix(CodeAction.Create(Title, action => BinaryUseNull(Context.Document, binaryToken, action), Title), Diagnostic);

            if (prefixToken != null)
                Context.RegisterCodeFix(CodeAction.Create(Title, action => PrefixUseNull(Context.Document, prefixToken, action), Title), Diagnostic);
        }

        private async Task<Document> BinaryUseNull(Document document, BinaryExpressionSyntax binaryExpression, CancellationToken cancellationToken)
        {
            BinaryExpressionSyntax newBinary = null;
            try
            {
                var memberAccess = binaryExpression.ChildNodes().OfType<MemberAccessExpressionSyntax>().First();
                newBinary = SyntaxFactory.BinaryExpression(
                    SyntaxKind.EqualsExpression,
                    SyntaxFactory.ParseExpression(memberAccess.GetIdentifier()),
                    SyntaxFactory.ParseExpression("null"));
            }
            catch
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(binaryExpression, newBinary);
            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> PrefixUseNull(Document document, PrefixUnaryExpressionSyntax prefixExpression, CancellationToken cancellationToken)
        {
            BinaryExpressionSyntax newBinary = null;
            try
            {
                var memberAccess = prefixExpression.ChildNodes().OfType<MemberAccessExpressionSyntax>().First();
                newBinary = SyntaxFactory.BinaryExpression(
                    SyntaxKind.EqualsExpression,
                    SyntaxFactory.ParseExpression(memberAccess.GetIdentifier()),
                    SyntaxFactory.ParseExpression("null"));
            }
            catch
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(prefixExpression, newBinary);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}