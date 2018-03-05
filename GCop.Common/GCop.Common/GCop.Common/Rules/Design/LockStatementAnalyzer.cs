namespace GCop.Common.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LockStatementAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.LockStatement;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "137",
                Category = Category.Design,
                Message = "Avoid locking on a Type or on the current object instance.",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var @lock = (LockStatementSyntax)context.Node;
            if (@lock.Expression is TypeOfExpressionSyntax || @lock.Expression is ThisExpressionSyntax)
            {
                ReportDiagnostic(context, @lock.GetFirstToken());
            }
        }
    }
}
