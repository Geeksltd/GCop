namespace GCop.Linq.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveUnnecessaryBracesAnalyzer : GCopAnalyzer
    {
        private SemanticModel SemanticModel;
        private InvocationExpressionSyntax Invocation;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "144",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = "Since there is only one statement, remove the unnecessary braces and write it as \"Database.Update( myObject, x => x.Abc = value );\""
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(context => AnalyzeSyntax(context), SyntaxKind.InvocationExpression);
        }

        private void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            SemanticModel = context.SemanticModel;
            Invocation = context.Node as InvocationExpressionSyntax;

            var method = GetMethodInfo(Invocation);
            if (method == null || method.Name != "Update") return;

            Invocation.ArgumentList.Arguments.Select(it => it.Expression).Where(it => it is SimpleLambdaExpressionSyntax).ForEach(expr =>
               {
                   var lambda = expr as SimpleLambdaExpressionSyntax;

                   var block = lambda.ChildNodes().OfType<BlockSyntax>().FirstOrDefault();

                   if (block == null) return;

                   if (block.GetCountOfStatements() == Numbers.One)
                   {
                       ReportDiagnostic(context, block.OpenBraceToken);
                   }
               });
        }

        private IMethodSymbol GetMethodInfo(InvocationExpressionSyntax invocation)
        {
            return invocation == null ? null : SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        }
    }
}
