namespace GCop.Common.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EmptyPartialClassAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.ClassDeclaration;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "104",
                Category = Category.Design,
                Message = "Remove empty partial class",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = context.Node as ClassDeclarationSyntax;
            NodeToAnalyze = classDeclaration;
            // if it has no modifiers, ignore
            // if it has any attributes, ignore
            // if it has any bases, ignore
            // if it has any type parameters, ignore
            if (classDeclaration.Modifiers.Count == 0 ||
                classDeclaration.AttributeLists.Count != 0 ||
                (classDeclaration.BaseList?.Types.Count ?? 0) != 0 ||
                (classDeclaration.TypeParameterList?.Parameters.Count ?? 0) != 0)
                return;

            // if its not partial, ignore
            if (classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)) == false)
                return;

            // if has no child nodes (statements), report error
            if (classDeclaration.ChildNodes().Any() == false)
            {
                var diagnostic = Diagnostic.Create(Description, classDeclaration.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
