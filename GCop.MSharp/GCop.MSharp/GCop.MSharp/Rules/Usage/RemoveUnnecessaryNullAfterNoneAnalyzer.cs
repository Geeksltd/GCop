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
    public class RemoveUnnecessaryNullAfterNoneAnalyzer : GCopAnalyzer
    {
        protected override void Configure()
        {
            RegisterSyntaxNodeAction(Analyze, SyntaxKind.LogicalOrExpression, SyntaxKind.LogicalAndExpression);
        }

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "510",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "None() already handles the null scenario. Remove unnecessary part: {0} == null."
            };
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var expression = NodeToAnalyze as BinaryExpressionSyntax;

            var invocations = expression?.DescendantNodes()?.Where(i => i.IsKind(SyntaxKind.InvocationExpression));
            var nullExpressions = expression?.DescendantNodes()?.OfType<BinaryExpressionSyntax>()?.Where(i => i.ChildNodes().LastOrDefault()?.IsKind(SyntaxKind.NullLiteralExpression) ?? false);

            if (invocations.None() || nullExpressions.None())
                return;

            // Null checks.
            var nullVariableNames = nullExpressions
                .Select(i => i.ChildNodes().FirstOrDefault()?.ChildTokens()?.OfType<SyntaxToken>()?.FirstOrDefault().ValueText)
                .ToList();

            // None() invocation.
            foreach (var invocation in invocations)
            {
                var memberAccessExpression = (invocation as InvocationExpressionSyntax)?.Expression as MemberAccessExpressionSyntax;

                if (memberAccessExpression == null)
                    continue;

                var variableName = memberAccessExpression.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault()?.Identifier.ValueText;
                var noneIdentifier = memberAccessExpression.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();

                if (nullVariableNames.Lacks(variableName) || noneIdentifier == null)
                    continue;

                if (!IsMSharpNoneMethodCalledOn(noneIdentifier, context))
                    continue;

                ReportDiagnostic(context, expression, variableName);
            }
        }

        private bool IsMSharpNoneMethodCalledOn(IdentifierNameSyntax noneIdentifier, SyntaxNodeAnalysisContext context)
        {
            var methodInfo = context.SemanticModel.GetSymbolInfo(noneIdentifier).Symbol as IMethodSymbol;

            return methodInfo != null
                && methodInfo.Name == "None"
                && methodInfo.IsExtensionMethod
                && methodInfo.ContainingAssembly.ToString().StartsWith("MSharp.Framework");
        }
    }
}
