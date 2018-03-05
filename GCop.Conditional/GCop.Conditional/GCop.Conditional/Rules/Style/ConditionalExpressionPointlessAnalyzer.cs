namespace GCop.Conditional.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConditionalExpressionPointlessAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.ConditionalExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "421",
                Category = Category.Style,
                Severity = DiagnosticSeverity.Warning,
                Message = "The conditions seems redundant. Either remove the ternary operator, or fix the values."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var expression = (ConditionalExpressionSyntax)NodeToAnalyze;

            if (RemoveParentheses(expression.WhenTrue).IsEquivalent(RemoveParentheses(expression.WhenFalse)))
            {
                ReportDiagnostic(context, expression.GetLocation());
            }
        }

        private ExpressionSyntax RemoveParentheses(ExpressionSyntax expression)
        {
            var currentExpression = expression;
            var parentheses = expression as ParenthesizedExpressionSyntax;
            while (parentheses != null)
            {
                currentExpression = parentheses.Expression;
                parentheses = currentExpression as ParenthesizedExpressionSyntax;
            }
            return currentExpression;
        }
    }
}
