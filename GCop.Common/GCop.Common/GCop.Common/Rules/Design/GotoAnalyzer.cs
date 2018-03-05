namespace GCop.Common.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GotoAnalyzer : GCopAnalyzer
    {
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "141",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = "Avoid using goto command"
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(context => Analyze(context), SyntaxKind.GotoStatement, SyntaxKind.GotoCaseStatement);
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            ReportDiagnostic(context, context.Node.GetLocation());
        }
    }
}
