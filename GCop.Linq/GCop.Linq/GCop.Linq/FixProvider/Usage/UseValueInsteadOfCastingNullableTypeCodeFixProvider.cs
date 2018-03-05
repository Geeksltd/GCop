namespace GCop.Linq.FixProvider.Usage
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseValueInsteadOfCastingNullableTypeCodeFixProvider)), Shared]
    public class UseValueInsteadOfCastingNullableTypeCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Replace casting with .Value";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop512");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<CastExpressionSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => UseValue(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> UseValue(Document document, CastExpressionSyntax cast, CancellationToken cancellationToken)
        {
            ExpressionSyntax newExpression = null;
            try
            {
                newExpression = SyntaxFactory.ParseExpression(cast.ChildNodes().OfType<IdentifierNameSyntax>()?.LastOrDefault().ToString() + ".Value");
            }
            catch (Exception ex)
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(cast, newExpression);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}