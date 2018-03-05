namespace GCop.Conditional.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfConditionParenthesisAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.IfStatement;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "178",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = "Use parenthesis to clarify your boolean logic intention."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;

            var ifStatementSyntax = (IfStatementSyntax)NodeToAnalyze;
            if (ifStatementSyntax == null) return;

            var logicals = ifStatementSyntax.Condition.DescendantNodes().OfType<BinaryExpressionSyntax>()
                 .Where(it => it.IsKind(
                     SyntaxKind.LogicalAndExpression,
                     SyntaxKind.LogicalOrExpression));

            foreach (var item in logicals)
            {
                if (item.Kind() == SyntaxKind.LogicalAndExpression && item.Parent.Kind() == SyntaxKind.LogicalOrExpression)
                {
                    Report(context, ifStatementSyntax);
                    return;
                }

                if (item.Kind() == SyntaxKind.LogicalOrExpression)
                {
                    if (item.DescendantNodes().Any(x => x.IsKind(SyntaxKind.LogicalAndExpression)))
                    {
                        foreach (var orExp in item.DescendantNodes().Where(x => x.IsKind(SyntaxKind.LogicalAndExpression)))
                        {
                            if (orExp.Parent.Kind() == item.Parent.Kind())
                            {
                                Report(context, ifStatementSyntax);
                                return;
                            }
                        }
                    }
                }

                if (item.Kind() == SyntaxKind.LogicalOrExpression && item.Parent.Kind() == SyntaxKind.LogicalAndExpression)
                {
                    Report(context, ifStatementSyntax);
                    return;
                }

                if (item.Kind() == SyntaxKind.LogicalAndExpression)
                {
                    if (item.DescendantNodes().Any(x => x.IsKind(SyntaxKind.LogicalOrExpression)))
                    {
                        foreach (var orExp in item.DescendantNodes().Where(x => x.IsKind(SyntaxKind.LogicalOrExpression)))
                        {
                            if (orExp.Parent.Kind() == item.Parent.Kind())
                            {
                                Report(context, ifStatementSyntax);
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void Report(SyntaxNodeAnalysisContext context, IfStatementSyntax ifStatementSyntax)
        {
            ReportDiagnostic(context, ifStatementSyntax.Condition);
        }
    }
}