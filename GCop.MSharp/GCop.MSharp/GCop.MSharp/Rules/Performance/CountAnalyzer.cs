namespace GCop.MSharp.Rules.Performance
{
    using Core;
    using Core.Attributes;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [MSharpExclusive]
    [ZebbleExclusive]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CountAnalyzer : GCopAnalyzer
    {
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "300",
                Category = Category.Performance,
                Message = "Replace with {0}()",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(context => Analyze(context), SyntaxKind.EqualsExpression
                                                                , SyntaxKind.GreaterThanExpression
                                                                , SyntaxKind.LessThanExpression
                                                                , SyntaxKind.GreaterThanOrEqualExpression);
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var binaryExpression = context.Node as BinaryExpressionSyntax;

            var invocation = binaryExpression.DescendantNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            if (invocation == null) return;

            var memberAccessExpression = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpression == null) return;

            var method = context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol as IMethodSymbol;
            if (method == null || method.Name != "Count") return;

            var rightHand = binaryExpression.DescendantNodes().OfType<LiteralExpressionSyntax>().FirstOrDefault(it => it.Token.ValueText.IsAnyOf("0", "1"));
            if (rightHand == null) return;

            if (binaryExpression.OperatorToken.IsKind(SyntaxKind.EqualsEqualsToken) && invocation.Parent?.Kind() == SyntaxKind.EqualsExpression)
            {
                if (rightHand.Token.ValueText == "1")
                {
                    ReportDiagnostic(context, binaryExpression.GetLocation(), "IsSingle");
                }
                else if (rightHand.Token.ValueText == "0")
                    ReportDiagnostic(context, binaryExpression.GetLocation(), "None");
            }
            else if (binaryExpression.OperatorToken.IsKind(SyntaxKind.GreaterThanToken) && invocation.Parent?.Kind() == SyntaxKind.GreaterThanExpression)
            {
                if (rightHand.Token.ValueText != "0") return;
                ReportAny(context, binaryExpression);
            }
            else if (binaryExpression.OperatorToken.IsKind(SyntaxKind.GreaterThanEqualsToken) && invocation.Parent?.Kind() == SyntaxKind.GreaterThanOrEqualExpression)
            {
                if (rightHand.Token.ValueText != "1") return;
                ReportAny(context, binaryExpression);
            }
            else if (binaryExpression.OperatorToken.IsKind(SyntaxKind.LessThanToken) && invocation.Parent?.Kind() == SyntaxKind.LessThanExpression)
            {
                var leftHand = binaryExpression.DescendantNodes().OfType<LiteralExpressionSyntax>().FirstOrDefault();

                if (leftHand != null && leftHand.Token.ValueText == Numbers.Zero.ToString())
                    ReportAny(context, binaryExpression);
            }
        }

        private void ReportAny(SyntaxNodeAnalysisContext context, BinaryExpressionSyntax input)
        {
            ReportDiagnostic(context, input.GetLocation(), "Any");
        }
    }
}
