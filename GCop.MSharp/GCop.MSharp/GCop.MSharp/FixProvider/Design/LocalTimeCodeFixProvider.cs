namespace GCop.MSharp.FixProvider.Design
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LocalTimeCodeFixProvider)), Shared]
    public class LocalTimeCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use LocalTime.Now";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop114");

        protected override void RegisterCodeFix()
        {
            try
            {
                var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
                if (token == null) return;

                Context.RegisterCodeFix(CodeAction.Create(Title, action => UseLocalTime(Context.Document, token, action), Title), Diagnostic);
            }
            catch (NullReferenceException)
            {
                //No matter to handle NullReferenceException
            }
        }

        private async Task<Document> UseLocalTime(Document document, MemberAccessExpressionSyntax memberAccess, CancellationToken cancellationToken)
        {
            MemberAccessExpressionSyntax newMemberAccess = null;
            try
            {
                newMemberAccess = SyntaxFactory.MemberAccessExpression(
                    memberAccess.Kind(),
                    SyntaxFactory.ParseExpression("LocalTime"),
                    SyntaxFactory.IdentifierName("Now"));
            }
            catch
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(memberAccess, newMemberAccess);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}