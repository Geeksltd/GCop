namespace GCop.Common.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ImplicitGenericMethodAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        private readonly string[] SpecificClassess = new string[] { "Database" };
        private readonly string[] SpecificMethods = new string[] { "Save" };
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "111",
                Category = Category.Design,
                Message = "Use implicit generic method typing",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocationExpression = context.Node as InvocationExpressionSyntax;
            var memberAccessExpression = invocationExpression?.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpression == null) return;

            var method = context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol as IMethodSymbol;
            if (method == null) return;

            if (!method.IsGenericMethod || SpecificMethods.Lacks(method.Name)) return;

            var typeArgument = method.TypeArguments.FirstOrDefault()?.ToString();
            var parameterType = method.Parameters.FirstOrDefault()?.Type.ToString();

            if (parameterType != typeArgument) return;

            if (memberAccessExpression.DescendantNodes().OfType<GenericNameSyntax>().Any())
                ReportDiagnostic(context, memberAccessExpression);
        }
    }
}
