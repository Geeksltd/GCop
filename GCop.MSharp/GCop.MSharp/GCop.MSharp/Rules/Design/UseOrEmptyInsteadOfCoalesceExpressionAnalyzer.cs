namespace GCop.MSharp.Rules.Design
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
    public class UseOrEmptyInsteadOfCoalesceExpressionAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        private readonly string MethodName = "Empty";
        private readonly string ExpectedConstructedFrom = "System.Linq.Enumerable.Empty<TResult>()";
        protected override SyntaxKind Kind => SyntaxKind.CoalesceExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "130",
                Category = Category.Design,
                Message = @"Instead of ?? Enumerable.Empty<>() use .OrEmpty() for collections that can be null",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var expression = context.Node as BinaryExpressionSyntax;
            if (expression == null) return;

            if (!(expression.Right is InvocationExpressionSyntax)) return;

            if (IsThereAnyConditionalAccessOperator(expression)) return;

            var right = ((InvocationExpressionSyntax)expression.Right).Expression as MemberAccessExpressionSyntax;
            if (right == null) return;

            var method = context.SemanticModel.GetSymbolInfo(right).Symbol as IMethodSymbol;
            if (method == null) return;

            if (method.Name == MethodName && method.ConstructedFrom.ToString() == ExpectedConstructedFrom)
            {
                ReportDiagnostic(context, expression.GetLocation());
            }
        }

        private bool IsThereAnyConditionalAccessOperator(BinaryExpressionSyntax expression)
        {
            return expression.ChildNodes().Any(it => it.IsKind(SyntaxKind.ConditionalAccessExpression));
        }
    }
}
