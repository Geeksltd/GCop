namespace GCop.Common.FixProvider.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Rules.Usage;
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PrivateKeywordAnalyzer)), Shared]
    public class PrivateKeywordCodeFixProvider : GCopCodeFixProvider
    {
        private string Title = "Remove private keyword";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop524");

        protected override void RegisterCodeFix()
        {
            try
            {
                var token = Root.FindToken(DiagnosticSpan.Start);
                if (!token.IsKind(SyntaxKind.PrivateKeyword)) return;
                Context.RegisterCodeFix(CodeAction.Create(Title, async action => await RemovePrivateKeywordAsync(Context.Document, token, action), Title), Diagnostic);
            }
            catch (NullReferenceException)
            {
                //No matter to handle NullReferenceException
            }
        }

        private async Task<Document> RemovePrivateKeywordAsync(Document document, SyntaxToken token, CancellationToken cancellationToken)
        {
            var oldNode = token.Parent;
            SyntaxNode newNode = null;
            if (token.Parent?.Kind() == SyntaxKind.MethodDeclaration)
            {
                var method = token.Parent.As<MethodDeclarationSyntax>();

                newNode = SyntaxFactory.MethodDeclaration(method.AttributeLists,
                    SyntaxFactory.TokenList(method.Modifiers.Where(it => it.Kind() != SyntaxKind.PrivateKeyword).ToArray()),
                    method.ReturnType,
                    method.ExplicitInterfaceSpecifier,
                    method.Identifier,
                    method.TypeParameterList,
                    method.ParameterList,
                    method.ConstraintClauses,
                    method.Body,
                    method.ExpressionBody,
                    method.SemicolonToken).WithLeadingTrivia(method.GetLeadingTrivia());
            }
            else if (token.Parent?.Kind() == SyntaxKind.PropertyDeclaration)
            {
                var property = token.Parent.As<PropertyDeclarationSyntax>();

                newNode = SyntaxFactory.PropertyDeclaration(property.AttributeLists,
                    SyntaxFactory.TokenList(property.Modifiers.Where(it => it.Kind() != SyntaxKind.PrivateKeyword).ToArray()),
                    property.Type,
                    property.ExplicitInterfaceSpecifier,
                    property.Identifier,
                    property.AccessorList,
                    property.ExpressionBody,
                    property.Initializer,
                    property.SemicolonToken).WithLeadingTrivia(property.GetLeadingTrivia());
            }
            else if (token.Parent?.Kind() == SyntaxKind.FieldDeclaration)
            {
                var field = token.Parent.As<FieldDeclarationSyntax>();

                newNode = SyntaxFactory.FieldDeclaration(field.AttributeLists, SyntaxFactory.TokenList(field.Modifiers.Where(it => it.Kind() != SyntaxKind.PrivateKeyword).ToArray()),
                    field.Declaration, field.SemicolonToken).WithLeadingTrivia(field.GetLeadingTrivia());
            }

            if (newNode == null) return document;

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(oldNode, newNode);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }
    }
}
