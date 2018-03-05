namespace GCop.MSharp.Rules.Refactoring
{
    using Core;
    using Core.Attributes;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    [MSharpExclusive]
    [ZebbleExclusive]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DatabaseGetInsteadOfDatabaseFindAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        ExpressionSyntax OtherSideExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "613",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "It should be written as Database.Get<{0}>({1})."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = context.Node as InvocationExpressionSyntax;

            var memberAccessExpression = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpression == null) return;

            var methodInfo = context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol as IMethodSymbol;
            if (methodInfo == null || methodInfo.Name != "Find" || methodInfo.ContainingType.Name != "Database" || methodInfo.ContainingAssembly.Name != "MSharp.Framework") return;

            if (invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression?.Kind() != SyntaxKind.SimpleLambdaExpression) return;

            var lambda = invocation.ArgumentList.Arguments.First().Expression as SimpleLambdaExpressionSyntax;
            if (lambda == null) return;

            var equalsExpression = lambda.Body as BinaryExpressionSyntax;
            if (!IsValidEqualsExpression(equalsExpression, lambda.Parameter.Identifier.ToString())) return;

            var genericName = memberAccessExpression.ChildNodes().OfType<GenericNameSyntax>().FirstOrDefault();
            ReportDiagnostic(context, invocation, genericName?.TypeArgumentList?.Arguments.FirstOrDefault()?.ToString() ?? "TYPE", OtherSideExpression.ToString());
        }

        private bool IsValidEqualsExpression(BinaryExpressionSyntax equalsExpression, string expectedLambdaParameter)
        {
            if (equalsExpression == null || equalsExpression.Kind() != SyntaxKind.EqualsExpression) return false;

            var idExpression = equalsExpression.Left as MemberAccessExpressionSyntax;
            if (idExpression == null)
            {
                idExpression = equalsExpression.Right as MemberAccessExpressionSyntax;
                OtherSideExpression = equalsExpression.Left;
            }
            else
                OtherSideExpression = equalsExpression.Right;

            if (idExpression == null) return false;

            var idProperty = CalculateIdExpression(idExpression).LastOrDefault();
            if (idProperty?.Identifier.ValueText != "ID")
            {
                idExpression = equalsExpression.Right as MemberAccessExpressionSyntax;
                OtherSideExpression = equalsExpression.Left;
                if (idExpression == null) return false;

                idProperty = CalculateIdExpression(idExpression).LastOrDefault();
                if (idProperty?.Identifier.ValueText != "ID") return false;
            }

            var actualLambdaParameter = CalculateIdExpression(idExpression).FirstOrDefault();
            if (actualLambdaParameter?.Identifier.ValueText != expectedLambdaParameter) return false;

            return true;
        }

        private IEnumerable<IdentifierNameSyntax> CalculateIdExpression(MemberAccessExpressionSyntax input)
        {
            return input.ChildNodes().OfType<IdentifierNameSyntax>();
        }
    }
}
