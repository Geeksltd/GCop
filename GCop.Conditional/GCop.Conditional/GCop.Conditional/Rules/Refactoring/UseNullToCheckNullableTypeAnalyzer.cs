namespace GCop.Conditional.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseNullToCheckNullableTypeAnalyzer : GCopAnalyzer
    {
        protected override void Configure()
        {
            RegisterSyntaxNodeAction(Analyze, SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression, SyntaxKind.LogicalNotExpression);
        }

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "690",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "Negative logic is taxing on the brain. Use \"{0} == null\" instead."
            };
        }

        protected void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;

            if (NodeToAnalyze is BinaryExpressionSyntax equalsExpression)
            {
                var memberAccessExpression1 = equalsExpression.ChildNodes().OfType<MemberAccessExpressionSyntax>()?.FirstOrDefault();

                LiteralExpressionSyntax falseExpression;

                if (equalsExpression.IsKind(SyntaxKind.EqualsExpression))
                {
                    falseExpression = equalsExpression.ChildNodes().OfType<LiteralExpressionSyntax>()?.FirstOrDefault(i => i.IsKind(SyntaxKind.FalseLiteralExpression));
                }
                else if (equalsExpression.IsKind(SyntaxKind.NotEqualsExpression))
                {
                    falseExpression = equalsExpression.ChildNodes().OfType<LiteralExpressionSyntax>()?.FirstOrDefault(i => i.IsKind(SyntaxKind.TrueLiteralExpression));
                }
                else
                {
                    return;
                }

                if (memberAccessExpression1 == null || falseExpression == null)
                    return;

                var identifiers1 = memberAccessExpression1.ChildNodes().OfType<IdentifierNameSyntax>();

                if (identifiers1?.Count() == 2)
                {
                    var variableName1 = identifiers1.First().Identifier.ValueText;
                    var hasValue1 = identifiers1.Last().Identifier.ValueText == "HasValue";

                    if (hasValue1)
                        ReportDiagnostic(context, equalsExpression, variableName1);
                }
            }

            var logicalNotExpression = NodeToAnalyze as PrefixUnaryExpressionSyntax;

            if (logicalNotExpression == null) return;

            var memberAccessExpression = logicalNotExpression.ChildNodes().OfType<MemberAccessExpressionSyntax>()?.FirstOrDefault();

            if (memberAccessExpression == null)
                return;

            var identifiers = memberAccessExpression.ChildNodes().OfType<IdentifierNameSyntax>();
            if (identifiers == null) return;
            if (identifiers.None()) return;
            if (identifiers.Count() != 2) return;

            var variableName = identifiers.First().Identifier.ValueText;
            var hasValue = identifiers.Last().Identifier.ValueText == "HasValue";

            if (hasValue)
                ReportDiagnostic(context, logicalNotExpression, variableName);
        }
    }
}
