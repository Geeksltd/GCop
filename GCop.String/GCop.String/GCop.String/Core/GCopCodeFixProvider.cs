namespace GCop.String.Core
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Text;
    using System.Linq;
    using System.Threading.Tasks;

    public abstract class GCopCodeFixProvider : CodeFixProvider
    {
        protected SyntaxNode Root { get; private set; }
        protected Diagnostic Diagnostic { get; private set; }
        protected TextSpan DiagnosticSpan { get; private set; }
        protected CodeFixContext Context;
        protected CodeAction NoAction = default(CodeAction);
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Context = context;
            Root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            Diagnostic = context.Diagnostics.First();
            DiagnosticSpan = Diagnostic.Location.SourceSpan;

            RegisterCodeFix();
        }

        protected abstract void RegisterCodeFix();
    }
}
