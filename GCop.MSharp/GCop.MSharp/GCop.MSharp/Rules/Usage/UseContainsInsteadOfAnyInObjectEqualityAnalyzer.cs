namespace GCop.MSharp.Rules.Usage
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
    public class UseContainsInsteadOfAnyInObjectEqualityAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.EqualsExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "532",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "It should be written as {0}.Contains({1})"
            };
        }
        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var equalsExpression = NodeToAnalyze as BinaryExpressionSyntax;

            var leftSide = equalsExpression.Left as IdentifierNameSyntax;
            if (leftSide == null) return;

            if (!(equalsExpression.Right is MemberAccessExpressionSyntax)) return;

            var invocation = equalsExpression.GetSingleAncestor<InvocationExpressionSyntax>();
            if (invocation == null) return;

            if (!invocation.ArgumentList.Arguments.IsSingle()) return;

            if (equalsExpression.Ancestors().OfType<PrefixUnaryExpressionSyntax>().Any(it => it.Kind() == SyntaxKind.LogicalNotExpression)) return;

            var lambda = invocation.ArgumentList.Arguments.First().Expression as SimpleLambdaExpressionSyntax;
            if (lambda == null) return;

            if (lambda.Parameter.Identifier.ValueText != leftSide.Identifier.ValueText) return;

            var methodInfo = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (methodInfo.Name != "Any") return;

            string variable = null;

            var memberAccessExpression = invocation.Expression?.As<MemberAccessExpressionSyntax>()?.GetIdentifierSyntax();
            if (memberAccessExpression == null) return;

            if (invocation.Expression.DescendantNodes().Any())
                variable = invocation.Expression.DescendantNodesAndSelf().OfType<MemberAccessExpressionSyntax>().FirstOrDefault()?.ToString().Remove(".Any");
            else
                variable = memberAccessExpression.Identifier.ValueText;

            if (variable.IsEmpty()) return;
            ReportDiagnostic(context, invocation, variable, equalsExpression.Right.ToString());
        }
    }
}
