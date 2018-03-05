namespace GCop.Linq.Rules.Performance
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WhereSimplifyAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        string[] Methods = new string[] { "Count", "First", "FirstOrDefault", "Single", "SingleOrDefault", "Any", "None" };
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "314",
                Category = Category.Performance,
                Message = "You don't need the Where clause. Replace with {0}.{1}({2})",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = context.Node as InvocationExpressionSyntax;
            if (invocation == null || invocation.ArgumentList.Arguments.Any()) return;

            var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null) return;

            var method = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol as IMethodSymbol;
            if (method == null || !method.Name.IsAnyOf(Methods) || method.ContainingNamespace.ToString() != "System.Linq") return;

            var previousInvocation = memberAccess.ChildNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            if (previousInvocation == null) return;

            var previousMethod = context.SemanticModel.GetSymbolInfo(previousInvocation).Symbol as IMethodSymbol;
            if (previousMethod == null || previousMethod.Name != "Where" || previousMethod.ContainingNamespace.ToString() != "System.Linq") return;

            var lambdaParameter = previousInvocation.ArgumentList.Arguments.FirstOrDefault();
            var collectionName = previousInvocation.Expression.ChildNodes().FirstOrDefault()?.ToString();

            var errorLocation = previousInvocation.Expression.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault()?.GetLocation();
            if (errorLocation == null) return;

            ReportDiagnostic(context, errorLocation, collectionName, method.Name, lambdaParameter?.ToString() ?? "condition => your statement here");
        }
    }
}
