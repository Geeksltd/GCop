namespace GCop.Conditional.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IsInsteadOfAsAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.NotEqualsExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "431",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "Use \" {0} is {1} \" as negative logic is taxing on the brain."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var notEqualsExpression = context.Node as BinaryExpressionSyntax;
            if (notEqualsExpression.Right == null || notEqualsExpression.Left == null || notEqualsExpression.Right.IsNotKind(SyntaxKind.NullLiteralExpression)) return;
            if (notEqualsExpression.Left.Kind() != SyntaxKind.AsExpression) return;

            var asExpression = notEqualsExpression.Left as BinaryExpressionSyntax;

            ReportDiagnostic(context, notEqualsExpression, asExpression.Left.ToString(), asExpression.Right.ToString());
        }
    }
}
