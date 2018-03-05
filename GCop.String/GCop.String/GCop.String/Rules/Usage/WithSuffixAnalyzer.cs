namespace GCop.String.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WithSuffixAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.ConditionalExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "530",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "Write this as {0}.WithSuffix({1})."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var conditionalExpression = context.Node as ConditionalExpressionSyntax;
            if (conditionalExpression.Condition == null || conditionalExpression.WhenFalse == null || conditionalExpression.WhenTrue == null) return;

            var invocation = conditionalExpression.Condition as InvocationExpressionSyntax;
            if (invocation == null) return;

            var identifierName = invocation.Expression?.As<MemberAccessExpressionSyntax>()?.ChildNodes()?.OfType<IdentifierNameSyntax>()?.LastOrDefault();
            if (identifierName == null) return;

            var variableName = invocation.Expression.As<MemberAccessExpressionSyntax>().GetIdentifier();
            if (variableName.IsEmpty()) return;
            if (identifierName.Identifier == null) return;

            if (identifierName.Identifier.ValueText == "IsEmpty")
            {
                if (conditionalExpression.WhenTrue.ToString().IsNoneOf("String.Empty", "string.Empty", "null", "\"\"")) return;
                if (conditionalExpression.WhenFalse.IsNotKind(SyntaxKind.AddExpression)) return;

                var addExpression = conditionalExpression.WhenFalse;

                var variable = addExpression.ChildNodes().FirstOrDefault() as IdentifierNameSyntax;
                if (variable == null) return;

                if (variable.As<IdentifierNameSyntax>()?.Identifier.ValueText != variableName) return;

                var something = addExpression.ChildNodes().LastOrDefault()?.ToString();
                if (something.IsEmpty()) return;

                ReportDiagnostic(context, conditionalExpression, variableName, something);
            }
            else if (identifierName.Identifier.ValueText == "HasValue")
            {
                if (conditionalExpression.WhenFalse.ToString().IsNoneOf("String.Empty", "string.Empty", "null", "\"\"")) return;
                if (conditionalExpression.WhenTrue.IsNotKind(SyntaxKind.AddExpression)) return;

                var addExpression = conditionalExpression.WhenTrue;
                var variable = addExpression.ChildNodes().FirstOrDefault() as IdentifierNameSyntax;
                if (variable == null) return;

                if (variable.As<IdentifierNameSyntax>()?.Identifier.ValueText != variableName) return;

                var something = addExpression.ChildNodes().LastOrDefault()?.ToString();
                if (something.IsEmpty()) return;

                ReportDiagnostic(context, conditionalExpression, variableName, something);
            }
        }
    }
}
