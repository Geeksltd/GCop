namespace GCop.MSharp.Rules.Refactoring
{
    using Core;
    using Core.Attributes;
    using Core.Syntax;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [MSharpExclusive]
    [ZebbleExclusive]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CriteriaShouldBeConvertedToSqlAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        string MSharp = "MSharp.Framework";
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "623",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "Either rewrite the criteria to be convertible to SQL, or take the criteria out to a {0} clause after calling the GetList method."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            return;
            /*NodeToAnalyze = context.Node;
            var invocation = NodeToAnalyze as InvocationExpressionSyntax;

            if (invocation == null || invocation.Expression == null || invocation.Expression.GetIdentifier() != "Database") return;

            var method = context.SemanticModel.GetSymbolInfo(invocation.Expression).Symbol as IMethodSymbol;
            if (method == null || method.Name.IsNoneOf("GetList", "Any", "FindWithMax", "FindWithMin", "Find") || method.ContainingAssembly.Name != MSharp) return;
            if (method.TypeArguments.FirstOrDefault()?.GetAttributes().Any(it => it.AttributeClass.Name == "SmallTableAttribute") ?? true) return;

            ValidationResult result = null;
            invocation.ArgumentList.Arguments.Select(it => it.Expression)
            .Where(it => it.Kind() == SyntaxKind.SimpleLambdaExpression)
            .TrueForAtLeastOnce(it =>
            {
                return !(result = CanRunInDbMode(it.As<SimpleLambdaExpressionSyntax>(), context.SemanticModel)).IsValid;
            });

            if (!result?.IsValid ?? false)
            {
                var methodName = method.Name == "Find" ? "FirstOrDefault" : "Where";
                ReportDiagnostic(context, result.Errors.First().ErrorLocation, methodName);
            }*/
        }

        public static ValidationResult CanRunInDbMode(SimpleLambdaExpressionSyntax lambda, SemanticModel semanticModel)
        {
            if (lambda == null) return ValidationResult.Ok;

            var nodes = lambda.DescendantNodes().ToList();
            var binaryExpression = nodes.OfType<BinaryExpressionSyntax>().FirstOrDefault(it => it.IsKind(SyntaxKind.LogicalNotExpression) || it.IsKind(SyntaxKind.LogicalOrExpression));
            if (binaryExpression != null)
                return ShowError(binaryExpression);

            foreach (var invocation in nodes.OfType<InvocationExpressionSyntax>())
            {
                if (IsAnyPartOfBinaryExpression(invocation.ToString(), invocation)) continue;

                foreach (var it in invocation.DescendantNodes().OfType<MemberAccessExpressionSyntax>())
                {
                    var symbol = semanticModel.GetSymbolInfo(it).Symbol;
                    if (symbol == null || symbol.Kind == SymbolKind.Property) continue;
                    //{
                    //    return symbol.GetAttributes().Any(attr => attr.AttributeClass.Name == "Calculated");
                    //}
                    if (symbol.ContainingAssembly.Name.IsNoneOf("System.Core", "mscorlib", "MSharp.Framework"))
                        return ShowError(it.Parent);

                    if (symbol.Kind == SymbolKind.Method && symbol.Name.IsAnyOf("Get", "Contains", "Lacks", "IsEmpty", "HasValue")) continue;
                    if (symbol.Kind == SymbolKind.Method && symbol.Name.IsAnyOf("StartsWith", "EndsWith"))
                    {
                        if (symbol.As<IMethodSymbol>().Parameters.IsSingle()) continue;
                        return ShowError(it.Parent);
                    }
                    return ShowError(it.Parent);
                }
            }
            return ValidationResult.Ok;
        }

        private static bool IsAnyPartOfBinaryExpression(string nodeSignature, InvocationExpressionSyntax invocation)
        {
            return invocation.Ancestors().OfType<BinaryExpressionSyntax>().FirstOrDefault()?.Right?.ToString().Contains(nodeSignature) ?? false;
        }

        private static ValidationResult ShowError(SyntaxNode sy)
        {
            return ValidationResult.Error(new ValidationError { ErrorLocation = sy.GetLocation() });
        }
    }
}
