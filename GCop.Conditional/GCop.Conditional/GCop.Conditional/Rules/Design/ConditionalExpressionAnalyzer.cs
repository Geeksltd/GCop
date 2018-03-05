namespace GCop.Conditional.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConditionalExpressionAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        int MaximumNumberOfCharacters = 110;
        protected override SyntaxKind Kind => SyntaxKind.ConditionalExpression;

        ConditionalExpressionSyntax Expression;
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "160",
                Category = Category.Design,
                Message = "This is not readable. Either refactor into a method, or use If / else statement.",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            Expression = context.Node as ConditionalExpressionSyntax;
            if (Expression == null) return;

            NodeToAnalyze = Expression;

            if (NumberOfCharacters() > MaximumNumberOfCharacters)
            {
                ReportDiagnostic(context, Expression);
            }
        }

        private int NumberOfCharacters()
        {
            if (Expression.WhenTrue == null || Expression.WhenFalse == null) return Numbers.Zero;

            return Expression.WhenTrue.ToString().Length + Expression.WhenFalse.ToString().Length;
        }
    }
}
