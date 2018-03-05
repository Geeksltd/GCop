namespace GCop.MSharp.Rules.Refactoring
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
    public class UseNoneInsteadOfNotAnyAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.LogicalNotExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "615",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "Negative logical comparisons are taxing on the brain. Instead of \"!{0}.Any()\" use \"{0}.None()\"."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var expression = NodeToAnalyze as PrefixUnaryExpressionSyntax;

            if (expression == null)
                return;

            var invocation = expression.ChildNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault();

            if (invocation == null)
                return;

            var identifier = (invocation.Expression as MemberAccessExpressionSyntax)?.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
            var variableName = (invocation.Expression as MemberAccessExpressionSyntax)?.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault()?.Identifier.ValueText;

            if (identifier != null && IsAnyMethodCalled(identifier, context.SemanticModel))
                ReportDiagnostic(context, expression, variableName);
        }

        private bool IsAnyMethodCalled(IdentifierNameSyntax identifier, SemanticModel semanticModel)
        {
            var methodInfo = semanticModel.GetSymbolInfo(identifier).Symbol as IMethodSymbol;

            return methodInfo != null
                && methodInfo.Name == "Any"
                && methodInfo.IsExtensionMethod;
        }
    }
}
