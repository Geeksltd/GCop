namespace GCop.IO.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FileExtensionCasingAnalyzer : GCopAnalyzer
    {
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "538",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "File extensions can have different casing. Use case insensitive string comparison."
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(AnalyzeCasing, SyntaxKind.EqualsExpression);
        }

        void AnalyzeCasing(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var equal = context.Node as BinaryExpressionSyntax;
            if (equal == null) return;

            if (equal.Left.ToString().Contains("Extension"))
            {
                if (equal.Left.ToString().Lacks(".ToLower")
                    &&
                    equal.Left.ToString().Lacks("ToUpper"))
                    ReportDiagnostic(context, equal.GetLocation());
            }
        }
    }
}
