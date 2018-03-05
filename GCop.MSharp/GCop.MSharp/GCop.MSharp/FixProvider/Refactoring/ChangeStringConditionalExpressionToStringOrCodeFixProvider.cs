namespace GCop.MSharp.FixProvider.Refactoring
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ChangeStringConditionalExpressionToStringOrCodeFixProvider)), Shared]
    public class ChangeStringConditionalExpressionToStringOrCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use Or method";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop642");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ConditionalExpressionSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => FixMethodAsync(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> FixMethodAsync(Document document, ConditionalExpressionSyntax conditional, CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newInvocation = null;
            try
            {
                newInvocation = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.ParseExpression(conditional.WhenTrue + ".Or"),
                    SyntaxFactory.ParseArgumentList("(" + conditional.WhenFalse + ")")
                    );
            }
            catch
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(conditional, newInvocation);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}