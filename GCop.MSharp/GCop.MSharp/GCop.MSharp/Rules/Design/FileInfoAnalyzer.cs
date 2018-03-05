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
    public class FileInfoAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.ObjectCreationExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "106",
                Message = "Use {0}.AsFile() instead of {1}",
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
            if (identifierNameSyntax?.ToString() != "FileInfo") return;
            var argumentListSyntax = objectCreationExpressionSyntax.ChildNodes().OfType<ArgumentListSyntax>().SingleOrDefault();

            context.ReportDiagnostic(Diagnostic.Create(Description, objectCreationExpressionSyntax.GetLocation(), argumentListSyntax, objectCreationExpressionSyntax));
        }
    }
}