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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertyToExpressionCodeFixProvider)), Shared]
    public class PropertyToExpressionCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Shorten this property by defining it as expression-bodied.";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop647");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => FixPropertyAsync(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> FixPropertyAsync(Document document, PropertyDeclarationSyntax property, CancellationToken cancellationToken)
        {
            var statementToConvert = property.AccessorList.Accessors[0].Body.ChildNodes().OfType<StatementSyntax>().First().ToFullString().Replace("return", "");

            var propertyStatement = SyntaxFactory.ParseExpression(statementToConvert);

            var newLiteral = SyntaxFactory.ArrowExpressionClause(propertyStatement);

            var newNode = SyntaxFactory.PropertyDeclaration(property.AttributeLists,
                                        property.Modifiers,
                                        property.Type,
                                        property.ExplicitInterfaceSpecifier,
                                        property.Identifier,
                                        null,
                                        newLiteral,
                                        property.Initializer,
                                        property.SemicolonToken);

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(property, newNode);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
