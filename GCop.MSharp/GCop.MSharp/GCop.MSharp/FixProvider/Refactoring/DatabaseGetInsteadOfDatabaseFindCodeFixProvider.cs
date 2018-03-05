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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DatabaseGetInsteadOfDatabaseFindCodeFixProvider)), Shared]
    public class DatabaseGetInsteadOfDatabaseFindCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use Database.Get method";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop613");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => UseDatabaseGet(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> UseDatabaseGet(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newInvocation = null;
            var argument = (invocation.ArgumentList.Arguments[0].Expression as LambdaExpressionSyntax).Body.ChildNodes().ToList()[1].ToString();
            try
            {
                newInvocation = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.ParseExpression(invocation.Expression.ToString().Replace("Find", "Get")),
                    SyntaxFactory.ParseArgumentList("(" + argument + ")")
                    );
            }
            catch
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(invocation, newInvocation);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}