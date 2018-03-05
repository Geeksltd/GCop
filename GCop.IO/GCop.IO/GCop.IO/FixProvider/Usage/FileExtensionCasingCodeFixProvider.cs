namespace GCop.IO.FixProvider.Usage
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FileExtensionCasingCodeFixProvider)), Shared]
    public class FileExtensionCasingCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Add .ToLower() to the left side";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop538");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<BinaryExpressionSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => AddToLower(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> AddToLower(Document document, BinaryExpressionSyntax binary, CancellationToken cancellationToken)
        {
            BinaryExpressionSyntax newBinary = null;
            try
            {
                var newExpression = SyntaxFactory.ParseExpression(binary.Left.ToString() + ".ToLower() ");
                newBinary = SyntaxFactory.BinaryExpression(binary.Kind(), newExpression, binary.OperatorToken, binary.Right);
            }
            catch (Exception ex)
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(binary, newBinary);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}