namespace GCop.ErrorHandling.Rules.Naming
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ValidateMethodWithoutThrowStatementAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "217",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "Rename the method to Is{0}Valid"
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;

            var methodDeclaration = NodeToAnalyze as MethodDeclarationSyntax;
            if (methodDeclaration.ReturnType.ToString().IsNoneOf("bool", "Boolean")) return;

            var methodName = methodDeclaration.GetName();
            if (methodName.IsEmpty() || !methodName.StartsWith("Validate", caseSensitive: false)) return;

            var restOfName = methodName.Remove("Validate");
            if (restOfName.IsEmpty()) return;

            //18418
            if (restOfName.Substring(0, 1).IsPascalCase() == false) return;

            var throwStatements = methodDeclaration.Body.DescendantNodes().OfType<ThrowStatementSyntax>().Where(it => it.Expression.ToString().Contains("ValidationException"));
            if (throwStatements.None())
            {
                ReportDiagnostic(context, methodDeclaration.Identifier, restOfName);
                return;
            }
        }
    }
}
