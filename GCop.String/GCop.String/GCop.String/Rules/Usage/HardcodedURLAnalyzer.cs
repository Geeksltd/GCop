namespace GCop.String.Rules.Usage
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Text.RegularExpressions;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HardcodedURLAnalyzer : GCopAnalyzer
    {
        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "539",
                Category = Category.Usage,
                Severity = DiagnosticSeverity.Warning,
                Message = "Don't hard-code URLs in the code as they might be subject to change. Use Config.Get(...) instead."
            };
        }

        protected override void Configure()
        {
            RegisterSyntaxNodeAction(AnalyzeLiterals, SyntaxKind.StringLiteralExpression);
        }

        void AnalyzeLiterals(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var expr = (LiteralExpressionSyntax)context.Node;

            var regex = new Regex(@"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\u00a1-\uffff\-\.\?\,\'\/\\\\(\)\;+&%\$#=_]*)?$", RegexOptions.IgnoreCase);

            if (regex.IsMatch(expr.ToString().Replace("\"", "")))
                ReportDiagnostic(context, expr.GetLocation());
        }
    }
}
