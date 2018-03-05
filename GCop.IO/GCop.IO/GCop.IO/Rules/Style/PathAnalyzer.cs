namespace GCop.IO.Rules.Style
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Text.RegularExpressions;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PathAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.StringLiteralExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "412",
                Category = Category.Style,
                Severity = DiagnosticSeverity.Warning,
                Message = @"Don't hardcode a path. Consider using “AppDomain.CurrentDomain.GetPath() instead”."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;

            var stringLiteral = (LiteralExpressionSyntax)NodeToAnalyze;
            if (stringLiteral == null) return;

            //Sample : c:\\Projects\\SafePlay OR Fileserver\: any address
            if (Regex.IsMatch(NodeToAnalyze.GetFirstToken().ValueText, @"^(?:[a-zA-Z]\:(\\|\/)|file\:\/\/|\\\\|\.(\/|\\))([^\\\/\:\*\?\<\>\\|]+(\\|\/){0,1})+$"))
            {
                ReportDiagnostic(context, NodeToAnalyze);
            }
        }
    }
}