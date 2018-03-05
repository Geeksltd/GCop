namespace GCop.Common.FixProvider.Usage
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AbstractClassPublicCtorCodeFixProvider)), Shared]
    public class AbstractClassPublicCtorCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Make constructor protected";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop540");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ConstructorDeclarationSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => MakeProtected(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> MakeProtected(Document document, ConstructorDeclarationSyntax constructor, CancellationToken cancellationToken)
        {
            ConstructorDeclarationSyntax newConstructor = null;
            try
            {
                newConstructor = SyntaxFactory.ConstructorDeclaration(
                    constructor.AttributeLists,
                    constructor.Modifiers.Replace(constructor.Modifiers.First(x => x.IsKind(SyntaxKind.PublicKeyword)), SyntaxFactory.ParseToken("protected ")),
                    constructor.Identifier,
                    constructor.ParameterList,
                    constructor.Initializer,
                    constructor.Body,
                    constructor.SemicolonToken
                    );
                newConstructor = newConstructor.WithLeadingTrivia(constructor.GetLeadingTrivia());
            }
            catch (Exception ex)
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(constructor, newConstructor);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}