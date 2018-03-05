namespace GCop.Linq.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LongWhereClauseAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        const int NodeCountLimitation = 10;

        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "635",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "The condition of the where clause is very long and should be turned into a method."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = context.Node as InvocationExpressionSyntax;
            if (invocation == null) return;

            var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null) return;


            var methodIdentifier = memberAccess.GetIdentifierSyntax();
            if (methodIdentifier == null) return;


            //var methodName = memberAccess.GetIdentifier();
            if (methodIdentifier.Identifier.ValueText != "Where") return;

            var lambdaParameter = invocation.ArgumentList.Arguments.FirstOrDefault();
            if (lambdaParameter == null) return;

            var internalNodes = lambdaParameter.DescendantNodes().OfKind(SyntaxKind.IdentifierName).Distinct();
            var nodes = internalNodes.GroupBy(x => x.ToString()).Count();
            if (nodes > NodeCountLimitation)
                ReportDiagnostic(context, lambdaParameter);
        }
    }
}