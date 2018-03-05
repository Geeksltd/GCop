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
    public class DateMethodsAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        private readonly string DefaultParameterType = "T";
        private InvocationExpressionSyntax Invocation;
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "306",
                Category = Category.Performance,
                Message = "Instead use {0} so it can be converted into a SQL statement and run faster",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            Invocation = (InvocationExpressionSyntax)context.Node;

            var method = GetMethodInfo(Invocation, context.SemanticModel);
            if (method == null || method.Name != "GetList") return;

            Invocation.ArgumentList.Arguments.Select(it => it.Expression).Where(it => it is SimpleLambdaExpressionSyntax).ForEach(expr =>
            {
                if (expr is SimpleLambdaExpressionSyntax lambda)
                {
                    lambda.Body.DescendantNodes().OfType<InvocationExpressionSyntax>().ForEach(invocation =>
                    {
                        method = GetMethodInfo(invocation, context.SemanticModel);
                        if (method == null) return;

                        var @operator = default(string);

                        if (method.Name == "IsBefore" && method.IsExtensionMethod)
                        {
                            @operator = "< operator";
                            ReportDiagnostic(context, invocation.Expression?.GetLocation() ?? Invocation.GetLocation(), @operator);
                        }

                        if (method.Name == "IsBeforeOrEqualTo" && method.IsExtensionMethod)
                        {
                            @operator = "<= operator";
                            ReportDiagnostic(context, invocation.Expression?.GetLocation() ?? Invocation.GetLocation(), @operator);
                        }

                        if (method.Name == "IsAfter" && method.IsExtensionMethod)
                        {
                            @operator = "> operator";
                            ReportDiagnostic(context, invocation.Expression?.GetLocation() ?? Invocation.GetLocation(), @operator);
                        }

                        if (method.Name == "IsAfterOrEqualTo" && method.IsExtensionMethod)
                        {
                            @operator = ">= operator";
                            ReportDiagnostic(context, invocation.Expression?.GetLocation() ?? Invocation.GetLocation(), @operator);
                        }

                        if (method.Name == "IsBetween" && method.IsExtensionMethod)
                        {
                            @operator = "> and < operators";
                            ReportDiagnostic(context, invocation.Expression?.GetLocation() ?? Invocation.GetLocation(), @operator);
                        }
                    });
                }
            });
        }

        private IMethodSymbol GetMethodInfo(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            return invocation == null ? null : semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        }

        private string GetGenericTypeName(IMethodSymbol method) => method.TypeArguments.FirstOrDefault()?.Name ?? DefaultParameterType;
    }
}
