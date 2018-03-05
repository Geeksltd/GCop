namespace GCop.Linq.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ChangeWhereToExceptAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "607",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "It should be written as {0}"
            };
        }
        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = context.Node as InvocationExpressionSyntax;

            var memberAccessExpression = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpression == null) return;

            var methodInfo = context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol as IMethodSymbol;
            if (methodInfo == null || methodInfo.Name != "Where" || methodInfo.ContainingNamespace.ToString() != "System.Linq") return;

            var lambda = invocation.ArgumentList.Arguments.FirstOrDefault(it => it?.Expression.Kind() == SyntaxKind.SimpleLambdaExpression)?.Expression as SimpleLambdaExpressionSyntax;
            if (lambda == null) return;

            if (lambda.ChildNodes().OfType<PrefixUnaryExpressionSyntax>().Any(it => it.IsKind(SyntaxKind.LogicalNotExpression)))
            {
                var message = invocation.ToString().ReplaceWholeWord("Where", "Except").Replace("!", "");
                ReportDiagnostic(context, memberAccessExpression.GetIdentifierSyntax(), message);
            }
        }
    }
}
