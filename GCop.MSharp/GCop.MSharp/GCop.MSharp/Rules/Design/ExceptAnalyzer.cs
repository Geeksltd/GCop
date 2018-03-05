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
    public class ExceptAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.InvocationExpression;

        new SyntaxNodeAnalysisContext Context;
        InvocationExpressionSyntax Invocation;
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "154",
                Category = Category.Design,
                Message = "Should be written simply as {0}.Except({1})",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            Context = context;
            NodeToAnalyze = Context.Node;
            Invocation = Context.Node as InvocationExpressionSyntax;
            if (Invocation == null) return;

            var method = Context.SemanticModel.GetSymbolInfo(Invocation).Symbol as IMethodSymbol;
            if (method == null) return;

            if (method.Name == "Where") AnalyzeTreeExpression(method, "Where", "Lacks");
            else if (method.Name == "Except") AnalyzeTreeExpression(method, "Except", "Contains");
        }

        private void AnalyzeTreeExpression(IMethodSymbol method, string firstMethodName, string secondMethodName)
        {
            var first = Invocation.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
            if (first == null) return;

            var beforeWhere = Context.SemanticModel.GetSymbolInfo(first).Symbol as IMethodSymbol;
            if (beforeWhere == null) return;

            var lambda = Invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression as SimpleLambdaExpressionSyntax;
            if (lambda == null) return;

            var lambdaSymbol = Context.SemanticModel.GetDeclaredSymbol(lambda.Parameter) as IParameterSymbol;
            if (lambdaSymbol == null) return;

            lambda.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Where(invocation => invocation.Name.Identifier.ValueText == secondMethodName).ForEach(invocation =>
         {
             var innerInvoke = Context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;

             if (innerInvoke != null && beforeWhere.ReturnType.ToString() == innerInvoke.ReceiverType.ToString())
             {
                 ReportDiagnostic(Context, Invocation, Invocation.Expression.ToString().Replace("." + firstMethodName, ""), invocation.Expression.ToString());
             }
         });
        }
    }
}
