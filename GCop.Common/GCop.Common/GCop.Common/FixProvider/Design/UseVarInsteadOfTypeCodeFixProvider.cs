namespace GCop.Common.FixProvider.Design
{
    using GCop.Common.Core;
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseVarInsteadOfTypeCodeFixProvider)), Shared]
    public class UseVarInsteadOfTypeCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Use var";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop132");

        protected override void RegisterCodeFix()
        {
            try
            {
                var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<VariableDeclarationSyntax>().FirstOrDefault();
                if (token == null) return;

                Context.RegisterCodeFix(CodeAction.Create(Title, action => UseVar(Context.Document, token, action), Title), Diagnostic);
            }
            catch (NullReferenceException)
            {
                //No matter to handle NullReferenceException
            }
        }

        private async Task<Document> UseVar(Document document, VariableDeclarationSyntax variable, CancellationToken cancellationToken)
        {
            VariableDeclarationSyntax newVariable = null;
            try
            {
                newVariable = SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.ParseTypeName("var "),
                    variable.Variables
                    );

                newVariable = newVariable.WithLeadingTrivia(variable.GetLeadingTrivia()).WithTrailingTrivia(variable.GetTrailingTrivia());
            }
            catch
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(variable, newVariable);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}