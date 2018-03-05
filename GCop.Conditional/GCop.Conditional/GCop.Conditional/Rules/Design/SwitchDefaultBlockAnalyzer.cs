namespace GCop.Conditional.Rules.Design
{
    using Core;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Linq;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SwitchDefaultBlockAnalyzer : GCopAnalyzer<SyntaxNodeAnalysisContext, SyntaxKind>
    {
        public static readonly string AddDefaultCaseWarning = "Make the 'default' case explicit. If you only expect your specified 'cases', throw a NotSupportedException. Otherwise it can simply break or return.";

        public static readonly string DefaultCaseIsEmptyWarning = "Add descriptive comment if the default block is supposed to be empty or throw an InvalidOperationException if that block is not supposed to be reached";
        protected override SyntaxKind Kind => SyntaxKind.SwitchStatement;

        protected override RuleDescription GetDescription()
        {
            return new RuleDescription
            {
                ID = "135",
                Category = Category.Design,
                Message = "{0}",
                Severity = DiagnosticSeverity.Warning
            };
        }

        protected override void Analyze(SyntaxNodeAnalysisContext context)
        {
            NodeToAnalyze = context.Node;
            var @switch = (SwitchStatementSyntax)context.Node;

            if (!@switch.Sections.TrueForAtLeastOnce(section => section.ChildNodes().OfType<DefaultSwitchLabelSyntax>().Any()))
            {
                ReportDiagnostic(context, @switch.CloseBraceToken, AddDefaultCaseWarning);
                return;
            }

            if (!@switch.Sections.TrueForAtLeastOnce(section =>
            {
                //We are sure there is a default case
                var defaultCase = section.ChildNodes().OfType<DefaultSwitchLabelSyntax>().FirstOrDefault();

                return true;
            }))
            {
                ReportDiagnostic(context, @switch.CloseBraceToken, DefaultCaseIsEmptyWarning);
                return;
            }
        }
    }
}
