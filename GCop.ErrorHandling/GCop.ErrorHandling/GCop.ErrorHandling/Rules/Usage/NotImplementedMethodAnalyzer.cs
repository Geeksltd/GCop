namespace GCop.ErrorHandling.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NotImplementedMethodAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        private readonly string[] ThrowExpression = new string[] { "throw", "new", "NotImplementedException()" };
        protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "507",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Info,
                Message = "Remember to implement this method."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var method = (MethodDeclarationSyntax)context.Node;

            var bodyImplementation = method.Body?.DescendantNodes();
            if (bodyImplementation?.None() ?? true) return;

            var statement = bodyImplementation.First();

            if (statement.IsKind(SyntaxKind.ThrowStatement) && statement.ToString().ContainsAll(ThrowExpression, caseSensitive: true))
            {
                ReportDiagnostic(context, method.Identifier);
            }
        }
    }
}
