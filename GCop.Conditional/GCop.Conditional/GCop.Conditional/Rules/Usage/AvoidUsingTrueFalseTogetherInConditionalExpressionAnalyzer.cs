namespace GCop.Conditional.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidUsingTrueFalseTogetherInConditionalExpressionAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.ConditionalExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "508",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "Since the condition is already a boolean it can be returned directly."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var expression = context.Node as ConditionalExpressionSyntax;

            if (expression.WhenTrue == null || expression.WhenFalse == null) return;

            if (expression.WhenTrue.IsAnyKind(SyntaxKind.TrueLiteralExpression, SyntaxKind.FalseLiteralExpression))
            {
                if (expression.WhenFalse.IsAnyKind(SyntaxKind.TrueLiteralExpression, SyntaxKind.FalseLiteralExpression))
                {
                    ReportDiagnostic(context, expression);
                }
            }
        }
    }
}
