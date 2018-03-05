namespace GCop.IO.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FileExtensionAnalyzer : GCopAnalyzer
    {
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "537",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "File extensions contain the leading dot character."
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(AnalyzeComparison, SyntaxKind.EqualsExpression);
            RegisterSyntaxNodeAction(AnalyzeExprComparison, SyntaxKind.InvocationExpression);
        }

        void AnalyzeComparison(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var equal = context.Node as BinaryExpressionSyntax;

            if (equal.Left.ToString().Contains("Extension"))
            {
                if (equal.Right is LiteralExpressionSyntax)
                    if (!equal.Right.ToString().Replace("\"", "").StartsWith("."))
                        ReportDiagnostic(context, equal.GetLocation());
            }
        }

        void AnalyzeExprComparison(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocationExpr = (InvocationExpressionSyntax)context.Node;

            if (invocationExpr.Expression.ToString().Contains("Extension"))
            {
                if (invocationExpr.Expression.GetIdentifier() == "Equals" &&
                    invocationExpr.ArgumentList.Arguments.Any(s => s.Expression is LiteralExpressionSyntax))
                {
                    var argument = invocationExpr.ArgumentList.Arguments.First(s => s.Expression is LiteralExpressionSyntax);
                    if (!argument.ToString().Replace("\"", "").StartsWith("."))
                        ReportDiagnostic(context, invocationExpr.GetLocation());
                }
            }
        }
    }
}
