namespace GCop.String.Rules.Refactoring
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Text.RegularExpressions;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidHardCodeEmailAddresss : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        protected override SyntaxKind Kind => SyntaxKind.StringLiteralExpression;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "646",
                Category = Category.Refactoring,
                Severity = DiagnosticSeverity.Warning,
                Message = "Email addresses should not be hard-coded. Move this to Settings table or Config file."
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var str = NodeToAnalyze as LiteralExpressionSyntax;
            if (str == null) return;

            if (IsEmail(str.Token.ValueText))
                ReportDiagnostic(context, NodeToAnalyze);
        }

        private bool IsEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            var re = new Regex(strRegex);
            return re.IsMatch(inputEmail);
        }

    }
}
