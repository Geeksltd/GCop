namespace GCop.Conditional.FixProvider.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Rules.Style;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NullableHasValueAnalyzer)), Shared]
    public class NullableHasValueCodeFixProvider : GCopCodeFixProvider
    {
        readonly string Title = "Use HasValue instead";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop430");

        protected override void RegisterCodeFix()
        {
            var diagnostic = Context.Diagnostics.FirstOrDefault();
            if (diagnostic == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, ct => RemoveParenthesisAsync(Context.Document, diagnostic, ct), nameof(NullableHasValueCodeFixProvider)), diagnostic);
        }

        private static async Task<Document> RemoveParenthesisAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var binaryExpression = root.FindNode(diagnosticSpan) as BinaryExpressionSyntax;
            if (binaryExpression?.Kind() != SyntaxKind.NotEqualsExpression) return document;

            var simpleNameSyntax = SyntaxFactory.IdentifierName("HasValue");
            var newCondition = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, binaryExpression.Left, simpleNameSyntax);

            var newRoot = root.ReplaceNode(binaryExpression, newCondition);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
