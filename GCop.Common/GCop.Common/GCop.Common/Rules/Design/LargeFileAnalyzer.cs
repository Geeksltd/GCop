namespace GCop.Common.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LargeFileAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        private readonly int MaximumLineNumber = 1000;
        protected override SyntaxKind Kind => SyntaxKind.ClassDeclaration;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "112",
                Category = Category.Design,
                Severity = DiagnosticSeverity.Warning,
                Message = "This class is too large. Break its responsibilities down into more classes."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var @class = context.Node as ClassDeclarationSyntax;

            var numberOfLines = @class?.NormalizeWhitespace()?.ToFullString()?.NumberOfLines() ?? 0;
            if (numberOfLines > MaximumLineNumber)
            {
                context.ReportDiagnostic(Diagnostic.Create(Description, @class?.Identifier.GetLocation() ?? @class.GetLocation()));
            }
        }
    }
}
