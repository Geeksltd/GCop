namespace GCop.Linq.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MultipleReturnInWhereClauseAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "422",
                Category = Category.Style,
                Severity = DiagnosticSeverity.Warning,
                Message = "Change it to a method."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;

            var invocation = NodeToAnalyze as InvocationExpressionSyntax;

            var memberAccess = invocation.ChildNodes().OfKind(SyntaxKind.SimpleMemberAccessExpression).FirstOrDefault();

            if (memberAccess == null) return;
            if (memberAccess.ChildNodes().None()) return;
            if (memberAccess.ChildNodes().OfType<IdentifierNameSyntax>().None()) return;
            if (memberAccess.ChildNodes()?.OfType<IdentifierNameSyntax>().LastOrDefault()?.Identifier.ValueText != "Where") return;

            var arguments = invocation.ArgumentList;
            if (arguments == null) return;
            if (arguments.Arguments == null) return;
            if (arguments.Arguments.None()) return;

            var lambda = arguments.Arguments.FirstOrDefault().Expression as LambdaExpressionSyntax;
            if (lambda == null) return;

            var body = lambda.Body as BlockSyntax;
            if (body == null) return;
            if (body.DescendantNodesAndSelf().OfType<ReturnStatementSyntax>().HasMany())
            {
                ReportDiagnostic(context, lambda);
            }
        }
    }
}

