namespace GCop.Conditional.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NullableHasValueAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.NotEqualsExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "430",
                Category = Category.Style,
                Severity = DiagnosticSeverity.Warning,
                Message = "Use \" {0}.HasValue \" as negative logic is taxing on the brain."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var notEqualsExpression = context.Node as BinaryExpressionSyntax;
            if (notEqualsExpression.Right == null || notEqualsExpression.Left == null || notEqualsExpression.Right.IsNotKind(SyntaxKind.NullLiteralExpression)) return;

            var symbol = context.SemanticModel.GetSymbolInfo(notEqualsExpression.Left).Symbol;
            if (symbol.IsNullable() == false) return;
            ReportDiagnostic(context, notEqualsExpression, notEqualsExpression.Left.ToString());
        }
    }
}
