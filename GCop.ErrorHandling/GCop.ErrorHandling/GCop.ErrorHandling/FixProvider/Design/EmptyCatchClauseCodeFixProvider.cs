namespace GCop.ErrorHandling.FixProvider.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EmptyCatchClauseCodeFixProvider)), Shared]
    public class EmptyCatchClauseCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "No logging is needed";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop138");


        protected override void RegisterCodeFix()
        {
            try
            {
                var block = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<BlockSyntax>().FirstOrDefault();
                if (block == null) return;

                Context.RegisterCodeFix(CodeAction.Create(Title, action => AddParameterDescriptionToDocumentationAsync(Context.Document, block, action), Title), Diagnostic);
            }
            catch (NullReferenceException)
            {
                //No matter to handle NullReferenceException
            }
        }

        private async Task<Document> AddParameterDescriptionToDocumentationAsync(Document document, BlockSyntax block, CancellationToken cancellationToken)
        {
            BlockSyntax newBlock = null;
            if (block.ChildNodes().Any())
            {
                var firstToken = block.ChildNodes().First().GetFirstToken();
                var newToken = firstToken.WithLeadingTrivia(firstToken.LeadingTrivia.AddRange(CreateNewLogErrorComment(block, withChild: true)));
                newToken = newToken.WithTrailingTrivia(EndOfTheLine());
                newBlock = block.ReplaceToken(firstToken, newToken);
            }
            else
            {
                var newCloseBrace = block.CloseBraceToken.WithLeadingTrivia(SyntaxFactory.TriviaList(CreateNewLogErrorComment(block, withChild: false)));
                newCloseBrace = newCloseBrace.WithTrailingTrivia(EndOfTheLine());
                newBlock = block.ReplaceToken(block.CloseBraceToken, newCloseBrace);
            }


            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(block, newBlock);
            return document.WithSyntaxRoot(newRoot);
        }

        private IEnumerable<SyntaxTrivia> CreateNewLogErrorComment(BlockSyntax block, bool withChild)
        {
            var traviaValue = !withChild ? "                // No logging is needed" : "// No logging is needed";
            return new SyntaxTrivia[]
            {
                SyntaxFactory.SyntaxTrivia( SyntaxKind.SingleLineCommentTrivia, traviaValue ),
                SyntaxFactory.SyntaxTrivia( SyntaxKind.EndOfLineTrivia,Environment.NewLine ),
                SyntaxFactory.SyntaxTrivia( SyntaxKind.WhitespaceTrivia,
                block.CloseBraceToken.LeadingTrivia.First(t=>t.Kind() == SyntaxKind.WhitespaceTrivia).ToFullString())
            };
        }

        private SyntaxTrivia EndOfTheLine() => SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, Environment.NewLine);
    }
}
