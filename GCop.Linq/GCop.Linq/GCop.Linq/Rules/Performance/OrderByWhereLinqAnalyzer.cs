namespace GCop.Linq.Rules.Performance
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OrderByWhereLinqAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "313",
                Category = Category.Performance,
                Message = "Where should be called first, so it is not doing unnecessary ordering of objects that will be thrown away.",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = (InvocationExpressionSyntax)NodeToAnalyze;

            var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null) return;

            var method = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol as IMethodSymbol;
            if (method == null || method.Name != "Where" || method.ContainingNamespace.ToString() != "System.Linq") return;

            var previousInvocation = memberAccess.ChildNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            if (previousInvocation == null) return;

            var previousMethod = context.SemanticModel.GetSymbolInfo(previousInvocation).Symbol as IMethodSymbol;
            if (previousMethod == null || previousMethod.Name != "OrderBy" || previousMethod.ContainingNamespace.ToString() != "System.Linq") return;

            ReportDiagnostic(context, previousInvocation.Expression?.GetIdentifierSyntax()?.GetLocation() ?? previousInvocation.GetLocation());
        }
    }
}
