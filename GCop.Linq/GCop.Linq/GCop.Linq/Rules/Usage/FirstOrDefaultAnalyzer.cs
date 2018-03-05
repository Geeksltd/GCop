namespace GCop.Linq.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FirstOrDefaultAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.ConditionalExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "513",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "It should be written as {0}.FirstOfDefault({1}) ?? {2}."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var conditionalExpression = NodeToAnalyze as ConditionalExpressionSyntax;

            var leftInvocation = conditionalExpression.Condition as InvocationExpressionSyntax;
            if (leftInvocation == null) return;

            var anyMethod = leftInvocation.Expression.As<MemberAccessExpressionSyntax>()?.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
            if (anyMethod == null || anyMethod.Identifier.ValueText != "Any") return;

            var whenTrueInvocation = conditionalExpression.WhenTrue as InvocationExpressionSyntax;
            if (whenTrueInvocation == null) return;

            var firstMethod = whenTrueInvocation.Expression.As<MemberAccessExpressionSyntax>()?.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
            if (firstMethod == null || firstMethod.Identifier.ValueText != "First") return;

            var collection = leftInvocation.DescendantNodes().OfType<InvocationExpressionSyntax>().LastOrDefault()?.Expression.As<MemberAccessExpressionSyntax>()?.GetIdentifier();
            var argument = leftInvocation.ArgumentList.Arguments.FirstOrDefault()?.ToString();
            ReportDiagnostic(context, conditionalExpression, collection, argument, conditionalExpression.WhenFalse.ToString());
        }
    }
}
