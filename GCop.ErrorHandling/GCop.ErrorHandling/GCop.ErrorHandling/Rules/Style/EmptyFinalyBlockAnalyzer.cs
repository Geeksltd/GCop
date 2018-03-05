namespace GCop.ErrorHandling.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EmptyFinalyBlockAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.FinallyClause;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "435",
                Category = Category.Style,
                Severity = DiagnosticSeverity.Warning,
                Message = "Finally block should not be empty"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var node = NodeToAnalyze as FinallyClauseSyntax;
            if (node.Block != null && node.Block.Statements.Any()) return;

            ReportDiagnostic(context, node.FinallyKeyword);
        }
    }
}
