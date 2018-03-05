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
    public class DirectoryInfoAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.ObjectCreationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "102",
                Message = "Use {0}.AsDirectory() instead of {1}",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var objectCreationExpressionSyntax = context.Node as ObjectCreationExpressionSyntax;
            if (objectCreationExpressionSyntax == null) return;

            var identifierNameSyntax = objectCreationExpressionSyntax.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
            if (identifierNameSyntax?.ToString() != "DirectoryInfo") return;
            var argumentListSyntax = objectCreationExpressionSyntax
                .ChildNodes().OfType<ArgumentListSyntax>().FirstOrDefault()
                ?.DescendantNodes().OfType<LiteralExpressionSyntax>().FirstOrDefault();

            if (argumentListSyntax != null)
            {
                context.ReportDiagnostic(Diagnostic.Create(Description, objectCreationExpressionSyntax.GetLocation(), argumentListSyntax, objectCreationExpressionSyntax));
            }
        }
    }
}