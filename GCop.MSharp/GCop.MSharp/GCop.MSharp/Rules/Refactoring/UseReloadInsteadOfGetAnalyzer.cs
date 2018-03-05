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
    public class UseReloadInsteadOfGetAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "611",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "It should be written as Database.Reload({0})."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = context.Node as InvocationExpressionSyntax;

            var memberAccessExpression = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpression == null) return;

            var methodInfo = context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol as IMethodSymbol;
            if (methodInfo == null || methodInfo.Name != "Get" || methodInfo.ContainingType.Name != "Database" || methodInfo.ContainingAssembly.Name != "MSharp.Framework") return;

            if (invocation.ArgumentList.Arguments.None()) return;

            var parameter = invocation.ArgumentList.Arguments.First().ToString();
            if (!parameter.EndsWith("ID")) return;

            var parameterInfo = context.SemanticModel.GetSymbolInfo(invocation.ArgumentList.Arguments.First().Expression).Symbol;

            if (parameterInfo == null) return;
            //if (!parameterInfo.ContainingType.IsInherited<IEntity>()) return;

            parameter = parameter.Remove("ID");
            if (parameter.IsEmpty())
                parameter = "this";
            if (parameter.EndsWith("."))
                parameter = parameter.Remove(parameter.LastIndexOf("."), 1);

            var method = invocation.GetSingleAncestor<MethodDeclarationSyntax>();
            if (method != null && method.ParameterList.Parameters.Any(it => it.Identifier.ValueText == parameterInfo.Name)) return;

            ReportDiagnostic(context, memberAccessExpression, parameter);
        }
    }
}
