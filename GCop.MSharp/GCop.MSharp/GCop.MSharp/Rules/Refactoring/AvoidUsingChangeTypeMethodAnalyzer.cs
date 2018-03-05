namespace GCop.MSharp.Rules.Refactoring
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
    public class AvoidUsingChangeTypeMethodAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "605",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "{0}"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = (InvocationExpressionSyntax)context.Node;
            var memberAccessExpression = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpression == null) return;
            var method = context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol as IMethodSymbol;
            if (method == null || method.Name != "ChangeType" || method.ContainingSymbol?.ToString() != "System.Convert") return;

            if (!invocation.ArgumentList.Arguments.HasMany()) return;

            var firstArgument = invocation.ArgumentList.Arguments.First();
            if (firstArgument == null || firstArgument.Expression == null) return;

            var firstArgumentTypeInfo = context.SemanticModel.GetTypeInfo(firstArgument.Expression).Type as ITypeSymbol;
            if (firstArgumentTypeInfo?.ToString() != "string") return;

            var parameterName = firstArgument.ToString();
            var toType = "Type";
            string message = null;
            var secondParameter = invocation.ArgumentList.Arguments.ElementAt(1);
            if (secondParameter.DescendantNodes().FirstOrDefault(it => it.IsKind(SyntaxKind.PredefinedType)) is PredefinedTypeSyntax typeSyntax)
            {
                toType = typeSyntax.Keyword.ToString();
                message = $"Use {parameterName}.To<{toType}>() instead.";
            }
            else
            {
                var toTypeIdentifier = invocation.ArgumentList.Arguments[1]?.ToString();//;GetToTypeIdentifier(secondParameter);
                if (toTypeIdentifier.HasValue())
                {
                    message = $"Use {parameterName}.To({toTypeIdentifier}) instead.";
                }
            }

            if (message.IsEmpty()) return;

            ReportDiagnostic(context, invocation, message);
        }
    }
}
