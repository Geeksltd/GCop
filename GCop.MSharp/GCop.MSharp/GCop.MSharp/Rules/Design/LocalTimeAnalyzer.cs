namespace GCop.MSharp.Rules.Design
{
    using Core;
    using Core.Attributes;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [MSharpExclusive]
    [ZebbleExclusive]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LocalTimeAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.SimpleMemberAccessExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "114",
                Category = Category.Design,
                Message = "DateTime.Now is not TDD friendly. Use LocalTime.Now instead.",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var memberAccessExpressionSyntax = context.Node as MemberAccessExpressionSyntax;
            if (memberAccessExpressionSyntax == null) return;

            var expression = memberAccessExpressionSyntax.ToString();

            if (expression == "DateTime.Now")
            {
                context.ReportDiagnostic(Diagnostic.Create(Description, memberAccessExpressionSyntax.GetLocation(), "Now", "Now"));
            }
            else if (expression == "DateTime.Today")
            {
                context.ReportDiagnostic(Diagnostic.Create(Description, memberAccessExpressionSyntax.GetLocation(), "Today", "Today"));
            }
        }
    }
}