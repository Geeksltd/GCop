namespace GCop.Common.FixProvider.Refactoring
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConvertToLambdaExpressionCodeFixProvider)), Shared]
    public class ConvertToLambdaExpressionCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Shorten this method by defining it as expression-bodied.";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop638");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => FixMethodAsync(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> FixMethodAsync(Document document, MethodDeclarationSyntax method, CancellationToken cancellationToken)
        {
            var statementToConvert = method.Body.ChildNodes().OfType<StatementSyntax>().First().ToFullString().Replace("return", "");

            var methodStatement = SyntaxFactory.ParseExpression(statementToConvert);

            var newLiteral = SyntaxFactory.ArrowExpressionClause(methodStatement);

            var newNode = SyntaxFactory.MethodDeclaration(method.AttributeLists,
                                        method.Modifiers,
                                        method.ReturnType,
                                        method.ExplicitInterfaceSpecifier,
                                        method.Identifier,
                                        method.TypeParameterList,
                                        method.ParameterList,
                                        method.ConstraintClauses,
                                        null,
                                        newLiteral,
                                        method.SemicolonToken);

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(method, newNode);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
