namespace GCop.Common.FixProvider.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Rules.Style;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UnnecessaryParenthesisAnalyzer)), Shared]
    public class UnnecessaryParenthesisCodeFixProvider : GCopCodeFixProvider
    {
        readonly string Title = "Remove parenthesis";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("GCop402");

        protected override void RegisterCodeFix()
        {
            var diagnostic = Context.Diagnostics.First();
            Context.RegisterCodeFix(CodeAction.Create(Title, ct => RemoveParenthesisAsync(Context.Document, diagnostic, ct), nameof(UnnecessaryParenthesisCodeFixProvider)), diagnostic);
        }

        private static async Task<Document> RemoveParenthesisAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var argumentList = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ArgumentListSyntax>().First();
            var newRoot = root.RemoveNode(argumentList, SyntaxRemoveOptions.KeepTrailingTrivia);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}