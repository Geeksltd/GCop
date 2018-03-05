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
    public class IsEquivalentToAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        InvocationExpressionSyntax Invocation;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "151",
                Category = Category.Design,
                Message = "Equals or == should be used",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            Invocation = context.Node as InvocationExpressionSyntax;
            if (Invocation == null) return;

            var memberAccessExpression = Invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpression == null) return;

            var method = context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol as IMethodSymbol;

            if (method == null || method.Name != "IsEquivalentTo" || method.ContainingAssembly.Name != "MSharp.Framework") return;

            if (Invocation.ArgumentList.Arguments.Any(it => it.Expression.IsKind(SyntaxKind.StringLiteralExpression)))
            {
                ReportDiagnostic(context, Invocation.Expression);
            }
        }
    }
}
