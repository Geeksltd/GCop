namespace GCop.Common.FixProvider.Style
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BlockEmptyStartEndCodeFixProvider)), Shared]
    public class BlockEmptyStartEndCodeFixProvider : GCopCodeFixProvider
    {
        private string Title => "Remove empty line(s)";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop438");

        protected override void RegisterCodeFix()
        {
            var token = Root.FindToken(DiagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<BlockSyntax>().FirstOrDefault();
            if (token == null) return;
            Context.RegisterCodeFix(CodeAction.Create(Title, action => RemoveLines(Context.Document, token, action), Title), Diagnostic);
        }

        private async Task<Document> RemoveLines(Document document, BlockSyntax block, CancellationToken cancellationToken)
        {
            BlockSyntax newBlock = null;
            var statements = new SyntaxList<StatementSyntax>();
            try
            {
                var blockStatements = block.ChildNodes().OfType<StatementSyntax>();

                foreach (var item in blockStatements)
                {
                    var statementToAdd = item.WithLeadingTrivia(item.GetLeadingTrivia().Except(x => x.IsKind(SyntaxKind.EndOfLineTrivia) && x.SpanStart < blockStatements.First().SpanStart));
                    statements = statements.Add(statementToAdd);
                }
                if (blockStatements.Count() != 0)
                    newBlock = block.Update(block.OpenBraceToken, statements, block.CloseBraceToken.WithLeadingTrivia(block.CloseBraceToken.LeadingTrivia.Except(x => x.IsKind(SyntaxKind.EndOfLineTrivia) && x.SpanStart < block.CloseBraceToken.SpanStart && x.SpanStart > blockStatements.Last().SpanStart)));
                else
                    newBlock = block.Update(block.OpenBraceToken, statements, block.CloseBraceToken.WithLeadingTrivia(block.CloseBraceToken.LeadingTrivia.Except(x => x.IsKind(SyntaxKind.EndOfLineTrivia) && x.SpanStart < block.CloseBraceToken.SpanStart)));
            }
            catch (Exception ex)
            {
                //No logging needed
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(block, newBlock);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}