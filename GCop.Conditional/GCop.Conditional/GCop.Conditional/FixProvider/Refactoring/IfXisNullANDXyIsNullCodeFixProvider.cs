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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IfXisNullANDXyIsNullCodeFixProvider)), Shared]
    public class IfXisNullANDXyIsNullCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use null operator";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop639");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => UseNull(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> UseNull(Document document, IfStatementSyntax ifStatement, CancellationToken cancellationToken)
        {
            ExpressionSyntax newExpression = null;
            var binaryExpression = ifStatement.Condition.ChildNodes().OfType<BinaryExpressionSyntax>().First(x => x.Left.IsKind(SyntaxKind.SimpleMemberAccessExpression)
            || x.Right.IsKind(SyntaxKind.SimpleMemberAccessExpression));

            var memberAccess = ((binaryExpression.Left.IsKind(SyntaxKind.SimpleMemberAccessExpression) ? binaryExpression.Left : binaryExpression.Right) as MemberAccessExpressionSyntax).ToString().Split('.');

            newExpression = SyntaxFactory.ParseExpression(memberAccess[0] + "?." + memberAccess[1] + " != null");

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(ifStatement.Condition, newExpression);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}