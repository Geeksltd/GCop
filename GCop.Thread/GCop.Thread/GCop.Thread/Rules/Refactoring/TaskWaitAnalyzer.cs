namespace GCop.Thread.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TaskWaitAnalyzer : GCopAnalyzer
    {
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "651",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "Wait() method can cause a deadlock. Use the await keyword on the method call instead."
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(AnalyzeExpression, SyntaxKind.SimpleMemberAccessExpression);
        }

        private void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;

            var memberAccess = context.Node as MemberAccessExpressionSyntax;
            if (memberAccess == null) return;

            var invocation = memberAccess.DescendantNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            if (invocation == null) return;

            var methodType = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (methodType == null) return;
            if (methodType.ReturnType.ToString().Lacks("System.Threading.Tasks.Task")) return;

            if (memberAccess.Parent == null) return;
            if (memberAccess.Parent is InvocationExpressionSyntax && memberAccess.Name?.ToString() == "Wait")
                ReportDiagnostic(context, memberAccess);
        }
    }
}
