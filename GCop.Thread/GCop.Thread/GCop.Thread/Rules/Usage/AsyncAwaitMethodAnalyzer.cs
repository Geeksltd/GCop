namespace GCop.Thread.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsyncAwaitMethodAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "541",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = @"Simply the method by removing the `async` and `await` keywords and just return the task directly."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var method = NodeToAnalyze as MethodDeclarationSyntax;

            if (method.Modifiers.Any(x => x.IsKind(SyntaxKind.VirtualKeyword))) return;
            if (method.Modifiers.None(x => x.IsKind(SyntaxKind.AsyncKeyword))) return;


            if (method.Body.DescendantNodes().OfType<StatementSyntax>().Count() != 1) return;
            if (method.Body.DescendantNodes().OfType<AwaitExpressionSyntax>().Count() != 1) return;

            var awaitStat = method.Body.DescendantNodes().OfType<AwaitExpressionSyntax>().FirstOrDefault();
            if (awaitStat.Ancestors().Any(c => c.IsAnyKind(SyntaxKind.SimpleMemberAccessExpression))) return;

            if (method.Body.DescendantNodes().Any(x => x.GetLineNumberToReport() > awaitStat.GetLineNumberToReport())) return;

            ReportDiagnostic(context, NodeToAnalyze);
        }
    }
}
