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
    public class UseToStringJoinAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "131",
                Category = Category.Design,
                Message = "Use {0}.ToString({1}) instead of {2}({1}, {0})",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocationExpressionSyntax = context.Node as InvocationExpressionSyntax;
            if (invocationExpressionSyntax == null) return;

            var invocation = invocationExpressionSyntax.Expression.ToString();

            if (!(invocation.IsAnyOf("string.Join", "String.Join"))) return;

            if (invocationExpressionSyntax.ArgumentList.Arguments.Count != 2) return;

            var argumentListSyntax = invocationExpressionSyntax.ChildNodes().OfType<ArgumentListSyntax>().Single();
            var seperatorName = argumentListSyntax.Arguments
                .FirstOrDefault()
                .ChildNodes()
                .SingleOrDefault(x => x.IsKind(SyntaxKind.StringLiteralExpression) || x.IsKind(SyntaxKind.IdentifierName))
                ?.ToString();

            var collectionName = argumentListSyntax.Arguments.LastOrDefault()?.ToString();

            if (seperatorName.HasValue() && collectionName.HasValue())
            {
                context.ReportDiagnostic(Diagnostic.Create(Description, invocationExpressionSyntax.GetLocation(), collectionName, seperatorName, invocation));
            }
        }
    }
}