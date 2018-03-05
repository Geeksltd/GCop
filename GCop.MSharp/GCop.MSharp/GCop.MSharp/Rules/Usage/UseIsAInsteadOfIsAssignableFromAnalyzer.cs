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
    public class UseIsAInsteadOfIsAssignableFromAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "528",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "Use IsA() or IsA<T>() instead."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var expression = NodeToAnalyze as InvocationExpressionSyntax;

            if (expression == null)
                return;

            var memberAccessExpression = expression.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
            var memberName = memberAccessExpression?.Name as IdentifierNameSyntax;

            if (memberAccessExpression == null || memberName == null || !IsIsAssignableFromMethodCalled(memberName, context))
                return;

            ReportDiagnostic(context, expression);
        }

        private bool IsIsAssignableFromMethodCalled(IdentifierNameSyntax identifier, SyntaxNodeAnalysisContext context)
        {
            var methodInfo = context.SemanticModel.GetSymbolInfo(identifier).Symbol as IMethodSymbol;
            return methodInfo != null && methodInfo.Name == "IsAssignableFrom";
        }
    }
}
