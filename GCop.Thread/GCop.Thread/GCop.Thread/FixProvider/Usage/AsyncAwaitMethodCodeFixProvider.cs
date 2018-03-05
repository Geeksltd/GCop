namespace GCop.Thread.FixProvider.Usage
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AsyncAwaitMethodCodeFixProvider)), Shared]
    public class AsyncAwaitMethodCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Remove `async` and `await` keywords and return the task directly.";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop541");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => RemoveAsync(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> RemoveAsync(Document document, MethodDeclarationSyntax method, CancellationToken cancellationToken)
        {
            MethodDeclarationSyntax newMethod = null;
            var statementList = new SyntaxList<StatementSyntax>();
            var firstStatement = method.Body.Statements.First();
            var newStatement = SyntaxFactory.ParseStatement(firstStatement.ToString().Replace("await", "return"));
            newStatement = newStatement.WithLeadingTrivia(firstStatement.GetLeadingTrivia());
            newStatement = newStatement.WithTrailingTrivia(firstStatement.GetTrailingTrivia());
            statementList = statementList.Add(newStatement);
            var newBody = SyntaxFactory.Block(method.Body.OpenBraceToken, statementList, method.Body.CloseBraceToken);
            newBody = newBody.WithLeadingTrivia(method.Body.GetLeadingTrivia());
            newBody = newBody.WithTrailingTrivia(method.Body.GetTrailingTrivia());

            try
            {
                newMethod = SyntaxFactory.MethodDeclaration(
                    method.AttributeLists,
                    SyntaxFactory.TokenList(SyntaxFactory.ParseToken(method.Modifiers.ToString().Replace("async", ""))),
                    method.ReturnType,
                    method.ExplicitInterfaceSpecifier,
                    method.Identifier,
                    method.TypeParameterList,
                    method.ParameterList,
                    method.ConstraintClauses,
                    newBody,
                    method.ExpressionBody,
                    method.SemicolonToken
                    );
                newMethod = newMethod.WithLeadingTrivia(method.GetLeadingTrivia());
                newMethod = newMethod.WithTrailingTrivia(method.GetTrailingTrivia());
            }
            catch (Exception ex)
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(method, newMethod);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}