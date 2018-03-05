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
    public class CookiePropertyAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "156",
                Category = Category.Design,
                Message = "Should be written as CookieProperty.Remove({0})",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocation = context.Node as InvocationExpressionSyntax;
            if (invocation == null) return;
            NodeToAnalyze = invocation;

            var method = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (method == null) return;

            if (method.Name == "Set" && method.ContainingType.Name == "CookieProperty" && method.ContainingNamespace.ToString().Contains("MSharp.Framework"))
            {
                if (invocation.ArgumentList.Arguments.Any(it => it.Expression.IsKind(SyntaxKind.NullLiteralExpression)))
                {
                    var key = invocation.ArgumentList.Arguments.FirstOrDefault(it => it.Expression.IsKind(SyntaxKind.StringLiteralExpression));

                    ReportDiagnostic(context, invocation.Expression, key?.ToString() ?? "key");
                }
            }
        }
    }
}
