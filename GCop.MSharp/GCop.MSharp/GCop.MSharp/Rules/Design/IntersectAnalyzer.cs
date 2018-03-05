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
    public class IntersectAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "155",
                Category = Category.Design,
                Message = "Should be written simply as {0}.Intersect({1})",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var invocation = context.Node as InvocationExpressionSyntax;
            if (invocation == null) return;

            var method = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (method == null) return;

            if (method.Name == "Where") AnalyzeTreeExpression(context, invocation, "Where", "Contains");
        }


        private void AnalyzeTreeExpression(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, string firstMethodName, string secondMethodName)
        {
            var first = invocation.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
            if (first == null) return;

            var beforeWhere = context.SemanticModel.GetSymbolInfo(first).Symbol as IMethodSymbol;
            if (beforeWhere == null) return;

            var lambda = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression as SimpleLambdaExpressionSyntax;
            if (lambda == null) return;

            var lambdaSymbol = context.SemanticModel.GetDeclaredSymbol(lambda.Parameter) as IParameterSymbol;
            if (lambdaSymbol == null) return;

            lambda.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Where(inv => inv.Name.Identifier.ValueText == secondMethodName).ForEach(inv =>
             {
                 if (context.SemanticModel.GetSymbolInfo(inv).Symbol is IMethodSymbol innerInvoke && beforeWhere.ReturnType.ToString() == innerInvoke.ReceiverType.ToString())
                 {
                     ReportDiagnostic(context, inv, inv.Expression.ToString().Replace("." + firstMethodName, ""), invocation.Expression.GetIdentifier());
                 }
             });
        }
    }
}
